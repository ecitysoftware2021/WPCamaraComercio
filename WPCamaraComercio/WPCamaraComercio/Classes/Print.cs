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
        public string Telefono { get; set; }
        public string Nit = GetConfiguration("NitEntidad");

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
            string RutaIMG =Logo;

            int increment = SPACE;
            g.DrawImage(Image.FromFile(Logo), 2, 2);
            g.DrawString("NIT " + Nit, fBody, sb, 70, 125);
            g.DrawString("Trámite:", fBody, sb, 10, increment);
            g.DrawString(Tramite, fBody1, sb, 120, increment);
            increment += 30;
            g.DrawString("Estado:", fBody, sb, 10, increment);
            g.DrawString(Estado, fBody1, sb, 120, increment);
            increment += 30;
            g.DrawString("Fecha de pago:", fBody, sb, 10, increment);
            g.DrawString(FechaPago.ToString(), fBody1, sb, 120, increment);
            increment += 30;
            g.DrawString("Referencia:", fBody, sb, 10, increment);
            g.DrawString(Referencia, fBody1, sb, 120, increment);
            increment += 30;
            g.DrawString("ID Compra:", fBody, sb, 10, increment);
            g.DrawString(IDCompra, fBody1, sb, 120, increment);
            increment += 30;
            g.DrawString("Cédula:", fBody, sb, 10, increment);
            g.DrawString(Cedula, fBody1, sb, 120, increment);
            increment += 30;
            g.DrawString("Nombre:", fBody, sb, 10, increment);
            g.DrawString(Nombre, fBody1, sb, 120, increment);
            increment += 30;
            g.DrawString("Teléfono:", fBody, sb, 10, increment);
            g.DrawString(Telefono, fBody1, sb, 120, increment);
            increment += 30;
            //g.DrawString("Correo:", fBody, sb, 10, increment);
            //g.DrawString(Correo, fBody1, sb, 120, increment);
            //increment += 30;
            if (Estado != "Rechazada")
            {
                g.DrawString("Total:", fBody, sb, 10, increment);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);
                increment += 30;
            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, increment);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);
                increment += 40;
            }


            g.DrawString("Su transacción se ha realizado exitosamente", fBody1, sb, 50, increment);
            increment += 30;
            g.DrawString("E-city software", fBody1, sb, 112, increment);
        }

        public static string GetConfiguration(string key)
        {
            AppSettingsReader reader = new AppSettingsReader();
            return reader.GetValue(key, typeof(String)).ToString();
        }

    }
}
