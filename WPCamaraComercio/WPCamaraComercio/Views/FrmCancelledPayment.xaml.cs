using System;
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
        //BackgroundViewModel backgroundViewModel;
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
            //backgroundViewModel = new BackgroundViewModel(Utilities.Operation);
            lblValue.Content = string.Format("{0:C0}", Utilities.ValueReturn);
            this.pay = pay;
            // this.DataContext = backgroundViewModel;
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
                Task.Run(() =>
                {
                    Canceltransactions();
                });

                pay.callbackReturn = valueDispenser =>
                {
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
                    WFCPayPadService.WCFPayPad.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                });
            }
            catch (Exception ex)
            {
                navigationService.NavigatorModal(ex.Message);
            }
        }

        /// <summary>
        /// Metodo para realizar la impresion del comprobante de pago con su respectivo estado y reinicio de la aplicacion.
        /// </summary>
        private void FinishTransaction()
        {
            camaraComercio.ImprimirComprobante("Cancelada");
            Utilities.GoToInicial();
        }
        #endregion
    }
}