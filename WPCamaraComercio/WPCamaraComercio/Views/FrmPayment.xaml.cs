using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
using WPCamaraComercio.ViewModels;
//using WPCamaraComercio.WCFPayPad;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPagoEfectivo.xaml
    /// </summary>
    public partial class FrmPayment : Window
    {
        #region References

        private PaymentViewModel PaymentViewModel;//Modelo para el pago(Valores e imagenes)


        private FrmLoading frmLoading;

        private Utilities utilities;

        WCFPayPadService payPadService;

        private Classes.LogErrorGeneral logError;

        private Record recorder;//Instancia

        private Api api;

        private TransactionDetails transactionDetails;

        private int count;//Contador utilizado para los reintentos en UpdateTrans

        private int tries;//Contador utilizado para los reintentos en SavePay

        private bool stateUpdate;

        private bool payState;

        CamaraComercio camaraComercio;

        NavigationService navigationService;

        bool isCancel = false;

        #endregion

        #region LoadMethods

        public FrmPayment()
        {
            InitializeComponent();

            try
            {
                OrganizeValues();
                frmLoading = new FrmLoading();
                payPadService = new WCFPayPadService();
                recorder = new Record();
                api = new Api();
                transactionDetails = new TransactionDetails();
                camaraComercio = new CamaraComercio();
                utilities = new Utilities();
                navigationService = new NavigationService(this);
                logError = new Classes.LogErrorGeneral
                {
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    IDCorresponsal = Utilities.CorrespondentId,
                    IdTransaction = Utilities.IDTransactionDB,
                    ValuePay = Utilities.ValueToPay,
                };

                //FinishPayment();
                count = 0;
                tries = 0;
                Utilities.control.StartValues();
            }
            catch (Exception ex)
            {
                Classes.LogService.CreateLogsPeticionRespuestaDispositivos("FrmPayment: ", "Error: " + ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //recorder.Grabar(Utilities.IDTransactionDB);
                Task.Run(() =>
                {
                    Utilities.control.StartValues();
                    ActivateWallet();
                });
            }
            catch (Exception ex)
            {
                Classes.LogService.CreateLogsPeticionRespuestaDispositivos("Window_Loaded: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método encargado de dar el estado inicial de todas las imagenes/botones de la vista
        /// </summary>
        private void VisibilityImage()
        {
            try
            {
                PaymentViewModel.ImgCancel = Visibility.Visible;
                PaymentViewModel.ImgIngreseBillete = Visibility.Visible;
                PaymentViewModel.ImgEspereCambio = Visibility.Hidden;
                PaymentViewModel.ImgLeyendoBillete = Visibility.Hidden;
                PaymentViewModel.ImgRecibo = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Classes.LogService.CreateLogsPeticionRespuestaDispositivos("VisibilityImage: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método encargado de organizar todos los valores de la transacción en la vista
        /// </summary>
        private void OrganizeValues()
        {
            try
            {
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
            catch (Exception ex)
            {
                Classes.LogService.CreateLogsPeticionRespuestaDispositivos("OrganizeValues: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region Events

        private void BtnCancel_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                try
                {
                    Classes.LogService.CreateLogsPeticionRespuestaDispositivos("BtnCancel_StylusDown: ", "Ingresé");
                }
                catch { }

                Dispatcher.BeginInvoke((Action)delegate
                {
                    Task.Run(() =>
                    {
                        Utilities.control.StopAceptance();

                        Classes.LogService.CreateLogsPeticionRespuestaDispositivos("BtnCancel_StylusDown: ", "Apague billetero");
                    });

                    this.Opacity = 0.5;
                    FrmModalConfirmation modal = new FrmModalConfirmation("¿Está seguro de cancelar la transacción?");
                    modal.ShowDialog();
                    this.Opacity = 1;

                    try
                    {
                        Classes.LogService.CreateLogsPeticionRespuestaDispositivos("BtnCancel_StylusDown: ", "Cerré la modal");
                    }
                    catch { }

                    if (modal.DialogResult.Value)
                    {

                        if (PaymentViewModel.ValorIngresado > 0)
                        {
                            Classes.LogService.CreateLogsPeticionRespuestaDispositivos("BtnCancel_StylusDown: ", "Abri formulario de cancelación");
                            FrmCancelledPayment cancel = new FrmCancelledPayment(PaymentViewModel.ValorIngresado);
                            cancel.Show();
                            this.Close();
                        }
                        else
                        {
                            try
                            {
                                Classes.LogService.CreateLogsPeticionRespuestaDispositivos("BtnCancel_StylusDown: ", "GoToInitial");
                            }
                            catch { }
                            Utilities.GoToInicial();
                        }
                    }
                    else
                    {
                        Utilities.control.StartAceptance(PaymentViewModel.PayValue);
                        try
                        {
                            Classes.LogService.CreateLogsPeticionRespuestaDispositivos("BtnCancel_StylusDown: ", "Prendi billetero");
                        }
                        catch { }
                    }
                });
            }
            catch (Exception ex)
            {
                Classes.LogService.CreateLogsPeticionRespuestaDispositivos("BtnCancel_StylusDown: ", "Error: " + ex.ToString());
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


                Utilities.log.Add(new LogTransactional
                {
                    Fecha = DateTime.Now,
                    IDTrsansaccion = Utilities.IDTransactionDB,
                    Operacion = "Orden Aceptar Dinero",
                    ValorDevolver = 0,
                    ValorDevuelto = "0",
                    ValorPago = Utilities.ValueToPay,
                    ValorIngresado = 0,
                    CantidadDevolucion = 0,
                    EstadoTransaccion = "En Proceso"
                });

             
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
                
                Utilities.control.callbackTotalOut = totalOut =>
                {
                    EndDispenserMoney(totalOut, 2);
                };

                Utilities.control.callbackOut = quiantityOut =>
                {
                    EndDispenserMoney(quiantityOut, 2);
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                };

                Utilities.log.Add(new LogTransactional
                {
                    Fecha = DateTime.Now,
                    IDTrsansaccion = Utilities.IDTransactionDB,
                    Operacion = "Orden Devolver Dinero",
                    ValorDevolver = PaymentViewModel.ValorSobrante,
                    ValorDevuelto = "0",
                    ValorPago = Utilities.ValueToPay,
                    ValorIngresado = PaymentViewModel.ValorIngresado,
                    CantidadDevolucion = 0,
                    EstadoTransaccion = "En Proceso"
                });

                
                Utilities.control.StartDispenser(returnValue);

            }
            catch (Exception ex)
            {
                try
                {
                    Classes.LogService.CreateLogsPeticionRespuestaDispositivos("ReturnMoney: ", "Error: " + ex.ToString());
                }
                catch { }
            }
        }

        private async void EndDispenserMoney(decimal quiantity, int stateTrans = 2, bool state = true)
        {
            try
            {
                Utilities.ValueDelivery = (long)quiantity;
                await Task.Run(() =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                });

                transactionDetails.Description = Utilities.control.LogMessage;
                RequestApi requestApi = new RequestApi
                {
                    Data = transactionDetails
                };
                var response = await api.GetResponse(requestApi, "SaveTransactionDetail");

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    Utilities.Loading(frmLoading, false, this);
                }));

                if (state)
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        BtnCancel.IsEnabled = false;
                    }));

                    FinishPayment();
                }
                else
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        BtnCancel.IsEnabled = false;
                    }));

                    FinishPayment();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Classes.LogService.CreateLogsPeticionRespuestaDispositivos("EndDispenserMoney: ", "Error: " + ex.ToString());
                }
                catch { }
            }

        }


        /// <summary>
        /// Método que se encarga de llenar un log de error general, esto cuando se produce una excepción
        /// </summary>
        /// <param name="ex">excepción a ser mostrada</param>
        private void ErroUpdateTrans(string message)
        {
            try
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
            catch (Exception ex)
            {
                try
                {
                    Classes.LogService.CreateLogsPeticionRespuestaDispositivos("ErroUpdateTrans: ", "Error: " + ex.ToString());
                }
                catch { }
            }
        }

        private async void ApproveTrans()
        {
            try
            {
                if (stateUpdate)
                {
                    var state = await AdminPaypad.UpdateTransaction(Utilities.IDTransactionDB, PaymentViewModel.ValorIngresado, 2, PaymentViewModel.ValorSobrante);
                    if (!state)
                    {
                        if (count < 2)
                        {
                            count++;
                            ApproveTrans();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stateUpdate = false;
            }
        }


        /// <summary>
        /// Método encargado de finalizar el pago y realizar las tareas pertinentes como actualizar la transacción e imprimir los recibos
        /// </summary>
        private async Task FinishPayment()
        {
            try
            {
                if (!isCancel)
                {
                    Utilities.BuyID = await camaraComercio.ConfirmarCompra();

                    if (!Utilities.BuyID.Equals("0"))
                    {

                        await Dispatcher.BeginInvoke((Action)delegate
                        {
                            Utilities.Loading(frmLoading, false, this);
                        });
                        await Dispatcher.BeginInvoke((Action)delegate
                        {
                            FinishPayment frmInformationCompany = new FinishPayment(PaymentViewModel.ValorIngresado, PaymentViewModel.ValorSobrante);
                            frmInformationCompany.Show();
                            this.Close();
                        });
                    }
                    else
                    {
                        await Dispatcher.BeginInvoke((Action)delegate
                        {
                            Utilities.Loading(frmLoading, false, this);
                        });
                        await Dispatcher.BeginInvoke((Action)delegate
                        {
                            FrmModal modal = new FrmModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                                "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                            "Comuniquese con servicio al cliente o diríjase a las taquillas."), this);
                            modal.ShowDialog();
                            if (modal.DialogResult.Value)
                            {
                                decimal value = PaymentViewModel.ValorIngresado - PaymentViewModel.ValorSobrante;
                                FrmCancelledPayment cancel = new FrmCancelledPayment(value);
                                cancel.Show();
                                this.Close();
                            }
                        });
                    }
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
    }
}
