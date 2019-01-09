using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPCamaraComercio.Classes;
using WPCamaraComercio.WCFPayPad;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FinishPayment.xaml
    /// </summary>
    public partial class FinishPayment : Window
    {
        #region Referencias
        Utilities utilities = new Utilities();
        CamaraComercio camaraComercio = new CamaraComercio();
        ServicePayPadClient WCFPayPadInsert = new ServicePayPadClient();
        #endregion

        public FinishPayment()
        {
            InitializeComponent();
        }

        public Task PDFBYTE()
        {
            var t = Task.Run(() =>
            {
                return camaraComercio.ListCertificadosiMPORT();
            });

            var c = t.ContinueWith((antecedent) =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (antecedent.Result)
                    {
                        WCFPayPadInsert.ActualizarEstadoTransaccion(Utilities.IDTransaccionDB, WCFPayPad.CLSEstadoEstadoTransaction.Aprobada);
                        camaraComercio.ImprimirComprobante("Aprobada");
                        utilities.RestartApplication();
                    }
                    else
                    {
                        WCFPayPadInsert.ActualizarEstadoTransaccion(Utilities.IDTransaccionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                        utilities.OpenModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                            "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                        "Comuniquese con servicio al cliente o diríjase a las taquillas."));
                        //this.BeginInvoke((Action)delegate
                        //{
                        //    frmRechazarPago objForm = new frmRechazarPago(CLSUtil.ValorPagar);
                        //    objForm.Show();
                        //    this.Dispose();
                        //});
                        //GC.Collect();
                    }
                }
                else if (t.Status == TaskStatus.Faulted)
                {
                    //objUtil.Exception(t.Exception.GetBaseException().Message);
                }
            });
            return t;
        }
    }
}
