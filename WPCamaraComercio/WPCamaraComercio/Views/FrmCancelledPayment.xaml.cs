using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Service;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPagoCancelado.xaml
    /// </summary>
    public partial class FrmCancelledPayment : Window
    {
        #region Reference
        NavigationService navigationService;
        WCFServices wcfService;
        WCFPayPadService WFCPayPadService;
        CamaraComercio camaraComercio;
        private LogErrorGeneral log;
        private Utilities utilities;
        PaymentController pay;
        #endregion

        #region MethodsInitials
        /// <summary>
        /// Contructor del formulario donde inicializamos todas las variables y componentes.
        /// </summary>
        /// <param name="pay">variable de objeto PaymentController para poder llevar el valor a devolver y no perderlo. </param>
        public FrmCancelledPayment(PaymentController pay)
        {
            InitializeComponent();
            navigationService = new NavigationService(this);
            wcfService = new WCFServices();
            WFCPayPadService = new WCFPayPadService();
            camaraComercio = new CamaraComercio();
            log = new LogErrorGeneral();
            utilities = new Utilities();
            lblValue.Content = string.Format("{0:C0}", Utilities.ValueReturn);
            this.pay = pay;
        }

        /// <summary>
        /// Metodo para cargar las diferentes acciones a realizar al momento de finalizar la carga de la ventana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Canceltransactions();

                pay.callbackReturn = valueDispenser =>
                {
                    Thread.Sleep(2000);
                    FinishTransaction();
                };
                pay.StartReturn(Utilities.ValueReturn);

            }
            catch (Exception ex)
            {
                navigationService.NavigatorModal(ex.Message);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Metodo para actualizar el estado de la transaccion en el servidor.
        /// </summary>
        private void Canceltransactions()
        {
            try
            {
                Task.Run(() =>
                {
                    WFCPayPadService.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                });
            }
            catch (Exception ex)
            {
                navigationService.NavigatorModal(ex.Message);
            }
        }

        private void CreateLog()
        {
            try
            {
                log.IdTransaction = Utilities.IDTransactionDB;
                log.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                log.Description = "Se actualiza la transacción y se deja en estado Cancelada";
                log.ValuePay = Utilities.ValueToPay;
                log.IDCorresponsal = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));
                log.State = "Cancelada";
                Utilities.SaveLogTransactions(log, "LogTransacciones\\Canceladas");
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CreateLog", "FrmCancelledPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        /// <summary>
        /// Metodo para realizar la impresion del comprobante de pago con su respectivo estado y reinicio de la aplicacion.
        /// </summary>
        private void FinishTransaction()
        {
            pay.Finish();
            Utilities.CrearLogTransactional(Utilities.log);
            CreateLog();
            Task.Run(() =>
            {
                camaraComercio.ImprimirComprobante("Cancelada");
            });
            Utilities.PayerData = null;
            Thread.Sleep(1000);
            Utilities.RestartApp();
        }
        #endregion
    }
}