using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;

namespace WPCamaraComercio.Classes
{
    public class Print
    {
        SolidBrush sb = new SolidBrush(Color.Black);

        Font fBody = new Font("Arial", 8, FontStyle.Bold);

        Font fBody1 = new Font("Arial", 8, FontStyle.Regular);

        Font rs = new Font("Stencil", 25, FontStyle.Bold);

        Font fTType = new Font("", 150, FontStyle.Bold);

        public string Nit = GetConfiguration("NitEntidad");

        public string Tramite { get; set; }

        public DateTime FechaPago { get; set; }

        public string Referencia { get; set; }

        public string Cedula { get; set; }

        public string Nombre { get; set; }

        public string Estado { get; set; }

        public decimal Valor { get; set; }

        public string Logo { get; set; }

        public int IDTramite { get; set; }

        public decimal ValorDevuelto { get; set; }

        public int SPACE { get { return 145; } }

        public string IDCompra { get; set; }

        public void ImprimirComprobante()
        {
            try
            {

                PrintController printcc = new StandardPrintController();

                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                pd.PrinterSettings.PrinterName = "MS-D347";
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);
                pd.PrintPage += new PrintPageEventHandler(PrintPage);
                pd.Print();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            string RutaIMG = Path.Combine(Directory.GetCurrentDirectory(), @"LogoComprobante\LCamaraComercio.png");

            g.DrawImage(Image.FromFile(RutaIMG), 2, 2);
            g.DrawString("NIT " + Nit, fBody, sb, 70, 125);
            g.DrawString("Trámite:", fBody, sb, 10, SPACE);
            g.DrawString(Tramite, fBody1, sb, 120, SPACE);
            g.DrawString("Estado:", fBody, sb, 10, SPACE + 30);
            g.DrawString(Estado, fBody1, sb, 120, SPACE + 30);
            g.DrawString("Fecha de pago:", fBody, sb, 10, SPACE + 60);
            g.DrawString(FechaPago.ToString(), fBody1, sb, 120, SPACE + 60);
            g.DrawString("Referencia:", fBody, sb, 10, SPACE + 90);
            g.DrawString(Referencia, fBody1, sb, 120, SPACE + 90);

            g.DrawString("ID Compra:", fBody, sb, 10, SPACE + 120);
            g.DrawString(IDCompra, fBody1, sb, 120, SPACE + 120);

            g.DrawString("Cédula:", fBody, sb, 10, SPACE + 150);
            g.DrawString(Cedula, fBody1, sb, 120, SPACE + 150);
            g.DrawString("Nombre:", fBody, sb, 10, SPACE + 170);
            g.DrawString(Nombre, fBody1, sb, 120, SPACE + 170);
            if (Estado != "Rechazada")
            {
                g.DrawString("Total:", fBody, sb, 10, SPACE + 190);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + 190);
            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, SPACE + 190);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + 190);
            }


            g.DrawString("Su transacción se ha realizado exitosamente", fBody1, sb, 50, SPACE + 230);
            g.DrawString("E-city software", fBody1, sb, 112, SPACE + 250);
        }

        public static string GetConfiguration(string key)
        {
            AppSettingsReader reader = new AppSettingsReader();
            return reader.GetValue(key, typeof(String)).ToString();
        }

    }
}
