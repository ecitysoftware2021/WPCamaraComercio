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
using WPCamaraComercio.Service;
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
        NavigationService navigationService;
        private int count = 0;
        private LogErrorGeneral logError;
        #endregion

        public FinishPayment()
        {
            InitializeComponent();
            logError = new LogErrorGeneral();
            navigationService = new NavigationService(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PDFBYTE();
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
                        ApproveTrans(CLSEstadoEstadoTransaction.Aprobada);
                        camaraComercio.ImprimirComprobante("Aprobada");
                        utilities.RestartApplication();
                    }
                    else
                    {
                        ApproveTrans(CLSEstadoEstadoTransaction.Cancelada);
                        navigationService.NavigatorModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                            "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                        "Comuniquese con servicio al cliente o diríjase a las taquillas."));

                        navigationService.NavigationTo("FrmCancelledPayment");
                    }
                }
                else if (t.Status == TaskStatus.Faulted)
                {
                    //objUtil.Exception(t.Exception.GetBaseException().Message);
                }
            });
            return t;
        }
        private void ApproveTrans(CLSEstadoEstadoTransaction estate)
        {
            try
            {
                var state = utilities.UpdateTransaction(estate);
                if (!state)
                {
                    if (count < 2)
                    {
                        count++;
                        ApproveTrans(estate);
                    }
                    else
                    {
                        string json = Utilities.CreateJSON();
                        logError.Description = json + "\nNo fue posible actualizar esta transacción a aprobada";
                        logError.State = "Iniciada";
                        Utilities.SaveLogTransactions(logError, "LogTransacciones\\Iniciadas");
                    }
                }
                else
                {
                    string json = Utilities.CreateJSON();
                    logError.Description = json + "\nTransacción Exitosa";
                    logError.State = "Aprobada";
                    Utilities.SaveLogTransactions(logError, "LogTransacciones\\Aprobadas");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
