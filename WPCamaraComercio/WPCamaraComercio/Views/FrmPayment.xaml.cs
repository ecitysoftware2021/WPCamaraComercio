using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
using WPCamaraComercio.ViewModels;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPagoEfectivo.xaml
    /// </summary>
    public partial class FrmPayment : Window
    {
        #region References

        private PaymentViewModel PaymentViewModel;//Modelo para el pago(Valores e imagenes)

        private WCFServices services;//Instancia para invocar los servicios web

        private FrmLoading frmLoading;

        private Utilities utilities;

        WCFPayPadService payPadService;

        private LogErrorGeneral logError;

        private Record recorder;//Instancia

        private int count;//Contador utilizado para los reintentos en UpdateTrans

        private int tries;//Contador utilizado para los reintentos en SavePay
        CamaraComercio camaraComercio;
        NavigationService navigationService;

        #endregion

        #region LoadMethods

        public FrmPayment()
        {
            InitializeComponent();
            OrganizeValues();
            services = new WCFServices();
            frmLoading = new FrmLoading();
            payPadService = new WCFPayPadService();
            recorder = new Record();
            camaraComercio = new CamaraComercio();
            utilities = new Utilities();
            navigationService = new NavigationService(this);
            logError = new LogErrorGeneral
            {
                Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                IDCorresponsal = Utilities.CorrespondentId,
                IdTransaction = Utilities.IDTransactionDB,
                ValuePay = Utilities.ValueToPay,
            };
            count = 0;
            tries = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //recorder.Grabar(Utilities.IDTransactionDB);
            Task.Run(() =>
            {
                ActivateWallet();
            });
        }

        #endregion

        #region Events

        private void btnCancelar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Utilities.control.StopAceptance();
                //recorder.FinalizarGrabacion();
                Task.Run(() =>
                {
                    utilities.UpdateTransaction(WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                });
                if (PaymentViewModel.ValorIngresado > 0)
                {
                    Utilities.DispenserVal = PaymentViewModel.ValorIngresado;
                    Utilities.Loading(frmLoading, true, this);
                    ReturnMoney(Utilities.DispenserVal);
                }
                else
                {
                    Utilities.GoToInicial();
                }
            }
            catch (Exception ex)
            {
                ErroUpdateTrans(ex.Message);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Método encargado de activar el billetero aceptance, seguido de esto crea un callback esperando a que este le indique que puede finalizar la transacción
        /// </summary>
        private void ActivateWallet()
        {
            try
            {
                Utilities.control.callbackValueIn = enterValue =>
                {
                    if (enterValue > 0)
                    {
                        PaymentViewModel.ValorIngresado += enterValue;
                    }
                };

                Utilities.control.callbackTotalIn = enterTotal =>
                {
                    Dispatcher.BeginInvoke((Action)delegate { Utilities.Loading(frmLoading, true, this); });
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    Utilities.EnterTotal = enterTotal;
                    if (enterTotal > 0 && PaymentViewModel.ValorSobrante > 0)
                    {
                        ReturnMoney(PaymentViewModel.ValorSobrante);
                    }
                    else
                    {
                        FinishPayment().Wait();
                    }
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                };

                Utilities.control.StartAceptance(PaymentViewModel.PayValue);
            }
            catch (Exception ex)
            {
                ErroUpdateTrans(ex.Message);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    FrmModal modal = new FrmModal(string.Concat("Lo sentimos,", Environment.NewLine, "Ha ocurrido un error."), this);
                    modal.ShowDialog();
                    if (modal.DialogResult.HasValue)
                    {
                        Utilities.GoToInicial();
                    }
                }));
            }
        }

        /// <summary>
        /// Método que se encarga de devolver el dinero ya sea por que se canceló la transacción o por que hay valor sobrante
        /// </summary>
        /// <param name="returnValue">valor a devolver</param>
        private void ReturnMoney(decimal returnValue)
        {
            try
            {
                Utilities.control.callbackValueOut = valueOut =>
                {
                    if (valueOut > 0)
                    {

                    }
                };

                Utilities.control.callbackTotalOut = totalOut =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    FinishPayment().Wait();
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                };

                Utilities.control.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método encargado de dar el estado inicial de todas las imagenes/botones de la vista
        /// </summary>
        private void VisibilityImage()
        {
            PaymentViewModel.ImgCancel = Visibility.Visible;
            PaymentViewModel.ImgIngreseBillete = Visibility.Visible;
            PaymentViewModel.ImgEspereCambio = Visibility.Hidden;
            PaymentViewModel.ImgLeyendoBillete = Visibility.Hidden;
            PaymentViewModel.ImgRecibo = Visibility.Hidden;
        }

        /// <summary>
        /// Método encargado de organizar todos los valores de la transacción en la vista
        /// </summary>
        private void OrganizeValues()
        {
            Utilities.ValueToPay = 12000;
            lblValorPagar.Content = string.Format("{0:C0}", Utilities.ValueToPay);
            PaymentViewModel = new PaymentViewModel
            {
                PayValue = Utilities.ValueToPay,
                ValorFaltante = Utilities.ValueToPay,
                ValorSobrante = 0,
                ValorIngresado = 0
            };

            VisibilityImage();
            this.DataContext = PaymentViewModel;
        }

        /// <summary>
        /// Método que se encarga de llenar un log de error general, esto cuando se produce una excepción
        /// </summary>
        /// <param name="ex">excepción a ser mostrada</param>
        private void ErroUpdateTrans(string message)
        {
            var json = Utilities.CreateJSON();
            logError.Description = string.Concat(json, Environment.NewLine, "Ocurrió un error en la transación IdTransaccion:",
                Utilities.IDTransactionDB,
                "\n error: ", message,
                "\n Total Ingresado: " + PaymentViewModel.ValorIngresado);
            logError.State = "Cancelada";
            Utilities.SaveLogTransactions(logError, "LogTransacciones\\Canceladas");
            //recorder.FinalizarGrabacion();
        }

        /// <summary>
        /// Método encargado de finalizar el pago y realizar las tareas pertinentes como actualizar la transacción e imprimir los recibos
        /// </summary>
        private async Task FinishPayment()
        {
            try
            {
                //ApproveTrans();

                var confirm = await camaraComercio.ConfirmarCompra();
                if (!confirm.Equals("0"))
                {
                    Dispatcher.BeginInvoke((Action)delegate { Utilities.Loading(frmLoading, false, this); });
                    navigationService.NavigationTo("FinishPayment");
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate { Utilities.Loading(frmLoading, false, this); });
                    utilities.UpdateTransaction(WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                    Dispatcher.BeginInvoke((Action)delegate {
                        FrmModal modal = new FrmModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                            "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                        "Comuniquese con servicio al cliente o diríjase a las taquillas."),this);
                        modal.ShowDialog();
                        if (modal.DialogResult.Value)
                        {
                            navigationService.NavigationTo("FrmCancelledPayment");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ErroUpdateTrans(ex.Message);
            }
        }

        /// <summary>
        /// Método encargado de actualizar la transacción a aprobada, se llama en finalizar pago En caso de fallo se reintenta dos veces más actualizar el estado de la transacción, si el error persiste se guarda en un log local y en el servidor, seguido de esto se continua con la transacción normal
        /// </summary>

        #endregion

        private void BtnCancel_StylusDown(object sender, StylusDownEventArgs e)
        {
            FrmModal modal = new FrmModal("Esta seguro de cancelar la transaccion?", this);
            if (modal.DialogResult.Value)
            {
                navigationService.NavigationTo("FrmCancelledPayment");
            }
        }
    }
}
