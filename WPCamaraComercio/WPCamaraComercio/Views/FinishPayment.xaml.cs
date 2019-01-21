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
        public FinishPayment(PaymentController pay,decimal valueInto)
        {
            InitializeComponent();
            payPadService = new WCFPayPadService();
            pay = new PaymentController();
            this.returnValue = valueInto;
            logError = new LogErrorGeneral();
            navigationService = new NavigationService(this);
            this.pay = pay;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PDFBYTE();
        }
        #endregion

        #region Methods
        public Task PDFBYTE()
        {
            try
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
                            //utilities.UpdateTransaction(enterValue, 2, Utilities.BuyID, returnValue);
                            payPadService.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Aprobada);
                            camaraComercio.ImprimirComprobante("Aprobada");
                            Utilities.GoToInicial();
                        }
                        else
                        {
                            //utilities.UpdateTransaction(enterValue, 3, Utilities.BuyID, returnValue);
                            payPadService.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
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
                                        Utilities.ValueReturn = returnValue;
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
                    else if (t.Status == TaskStatus.Faulted)
                    {
                        //objUtil.Exception(t.Exception.GetBaseException().Message);
                    }
                });
                return t;
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("PDFBYTE", "FinishPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
                return null;
            }
        }

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
