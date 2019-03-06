using System;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Service;
//using WPCamaraComercio.WCFPayPad;

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
        //ServicePayPadClient WCFPayPadInsert = new ServicePayPadClient();
        WCFPayPadService payPadService;
        NavigationService navigationService;
        PaymentController pay;
        private int count = 0;
        private LogErrorGeneral logError;
        decimal enterValue = 0;
        decimal returnValue = 0;
        #endregion

        #region LoadMethods
        public FinishPayment(PaymentController pay, decimal valueInto)
        {
            InitializeComponent();
            payPadService = new WCFPayPadService();
            this.returnValue = valueInto;
            logError = new LogErrorGeneral();
            navigationService = new NavigationService(this);
            this.pay = pay;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitPrinter();
        }

        private void InitPrinter()
        {
            try
            {
                var taskPrint = Task.Run(() =>
                {
                    return camaraComercio.ListCertificadosiMPORT();
                    //camaraComercio.Print("2");
                });

                if (taskPrint != null)
                {
                    var responseTask = taskPrint.ContinueWith((antecedent) =>
                    {
                        if (taskPrint.Status == TaskStatus.RanToCompletion)
                        {
                            if (antecedent.Result)
                            {
                                FinishPrint(WCFPayPad.CLSEstadoEstadoTransaction.Aprobada);
                            }
                            else
                            {
                                FinishPrint(WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                            }
                        }
                        else if (taskPrint.Status == TaskStatus.Faulted)
                        {
                            FinishPrint(WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("InitPrinter", "FinishPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void FinishPrint(WCFPayPad.CLSEstadoEstadoTransaction state)
        {
            try
            {
                Task.Run(() =>
                {
                    payPadService.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, state);
                });

                if (state == WCFPayPad.CLSEstadoEstadoTransaction.Aprobada)
                {
                    pay.Finish();
                    camaraComercio.ImprimirComprobante("Aprobada");
                    Utilities.CrearLogTransactional(Utilities.log);

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Utilities.PayerData = null;
                        Utilities.RestartApp();
                       
                    });
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        FrmModal modal = new FrmModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                                "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                                "Comuniquese con servicio al cliente o diríjase a las taquillas."), this);
                        modal.ShowDialog();
                        if (modal.DialogResult.Value)
                        {
                            if (returnValue != 0)
                            {
                                Utilities.ValueReturn = Utilities.ValueToPay;
                                GotoCancel();
                            }
                            else
                            {
                                pay.Finish();
                                Utilities.GoToInicial();
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("FinishPrint", "FinishPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        #endregion

        #region Methods
        private void GotoCancel()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                FrmCancelledPayment frmCancelPay = new FrmCancelledPayment(pay);
                frmCancelPay.Show();
                Close();
            }));
        }
        #endregion
    }
}
