using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;

namespace WPCamaraComercio.Classes
{
    public class CLSPrint
    {
        private Utilities utilities = new Utilities();
        private SolidBrush sb = new SolidBrush(Color.Black);
        private Font fTitles = new Font("Arial", 8, FontStyle.Bold);
        private Font fContent = new Font("Arial", 8, FontStyle.Regular);

        #region "Referencias"
        public string ReferenciaDePago { get; set; }

        public DateTime FechaPago { get; set; }

        public string Identificacion { get; set; }

        public string NumeroDeContrato { get; set; }

        public string IdentificacionClienteEMP { get; set; }

        public string SaldoAnteriorVencido { get; set; }

        public string SaldoAnteriorVigente { get; set; }

        public string SaldoAnteriorFactura { get; set; }

        public string ValorPagadoPorCliente { get; set; }

        public string NuevoSaldoFactura { get; set; }

        public string SaldoAfavor { get; set; }

        public string ValorTotalDeFactura { get; set; }

        public string RestantePagosFraccionados { get; set; }

        public DateTime FechaVencimientoFactura { get; set; }

        public string ProductosAsuspender { get; set; }

        public string PuntoDeVenta { get; set; }
        #endregion

        #region "Métodos"
        public void ImprimirComprobante()
        {
            try
            {
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);
                pd.PrintPage += new PrintPageEventHandler(PrintPage);
                pd.Print();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("ImprimirComprobante", "CLSPrint", ex.ToString());
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                int y = 0;
                int sum = 30;
                int x = 150;

                string RutaIMG = GetConfiguration("LogoComprobante");
                g.DrawImage(Image.FromFile(RutaIMG), y += sum + 20, 0);

                g.DrawString("Empresas Públicas De Medellín", fContent, sb, 75, y += sum);

                g.DrawString("Nit 890.904.996-1", fContent, sb, 95, y += sum);

                g.DrawString("Nombre del servicio:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Referencia de pago:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Fecha de pago:", fTitles, sb, 10, y += sum);
                g.DrawString(FechaPago.ToString(), fContent, sb, x, y);

                g.DrawString("Número de contrato:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Identificación cliente EPM:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Saldo anterior vencido:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Saldo anterior vigente:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Saldo anterior factura:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Saldo a favor:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Valor total de la factura:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("# Restante pagos fraccionados:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Fecha de vencimietno de factura:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Productos a suspenderse:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Canal de venta:", fTitles, sb, 10, y += sum);
                g.DrawString(Identificacion, fContent, sb, x, y);

                g.DrawString("Apreciado Cliente, usted solo cuenta con una", fContent, sb, 10, y += sum);
                g.DrawString("oportunidad para cancelar el saldo de la factura.", fContent, sb, 10, y += 20);

                g.DrawString("Recuerde siempre esperar la tirilla de soporte de su", fContent, sb, 10, y += sum);
                g.DrawString("pago, es el único documento que lo respalda.", fContent, sb, 10, y += 20);


                //g.DrawString("Usuario:", fTitles, sb, 10, y += sum);
                //g.DrawString(string.Concat(FormatName(Usuario, true),
                //            Environment.NewLine, FormatName(Usuario, false)), fContent, sb, x, y);


            }
            catch (Exception ex)
            {
                utilities.saveLogError("PrintPage", "CLSPrint", ex.ToString());
            }
        }

        private string FormatName(string value, bool flag)
        {
            try
            {
                var names = value.Split('-');

                if (names.Length <= 1)
                {
                    names = value.Split(' ');
                }

                if (flag && names.Length > 1)
                {
                    return string.Format("{0} {1}", names[0], names[1]);
                }

                string name = string.Empty;
                if (names.Length > 2)
                {
                    for (int i = 2; i < names.Length; i++)
                    {
                        name += names[i] + " ";
                    }
                }

                return name;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion

        #region "Eventos"
        public static string GetConfiguration(string key)
        {
            try
            {
                AppSettingsReader reader = new AppSettingsReader();
                return reader.GetValue(key, typeof(String)).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
