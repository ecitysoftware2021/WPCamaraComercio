using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        NavigationService navigationService;
        WCFPayPadService WFCPayPadService;
        WCFServices wCFService;
        PaymentController pay;
        PayViewModel payModel;
        CamaraComercio camaraComercio;
        Utilities utilities;
        decimal amount = 0;
        List<Log> log;
        #endregion

        #region InitialMethods
        public FrmPayment()
        {
            try
            {
                InitializeComponent();
                log = new List<Log>();
                utilities = new Utilities();
                WFCPayPadService = new WCFPayPadService();
                navigationService = new NavigationService(this);
                wCFService = new WCFServices();
                camaraComercio = new CamaraComercio();
                InitPay();
                //SendFinish();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("FrmPayment", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }
        #endregion

        #region Methods
        private void InitPay()
        {
            try
            {
                amount = Math.Floor(Utilities.ValueToPay);
                lblValorPagar.Content = string.Format("{0:C0}", amount);
                pay = new PaymentController();
                TimerTime();
                payModel = new PayViewModel
                {
                    ValorFaltante = string.Format("{0:C0}", amount),
                    ValorIngresado = string.Format("{0:C0}", 0),
                    ValorSobrante = string.Format("{0:C0}", 0),
                    ImgCancel = Visibility.Visible,
                    ImgEspereCambio = Visibility.Hidden,
                    ImgIngreseBillete = Visibility.Visible,
                    ImgLeyendoBillete = Visibility.Hidden,
                    ImgRecibo = Visibility.Hidden,
                };

                PaymentGrid.DataContext = payModel;
                pay.callback = value =>
                {
                    FinishPayment(value);
                };

                pay.callbackValue = newValue =>
                {
                    SetMountInsert(newValue.ToString());
                };

                pay.Start(amount);
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("InitPay", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void FillLog(string menssage)
        {
            log.Add(new Log
            {
                Fecha = DateTime.Now,
                IDTrsansaccion = Utilities.IDTransactionDB,
                Operacion = menssage,
                ValorDevolver = 0,
                ValorDevuelto = "0",
                ValorPago = Utilities.ValueToPay,
                ValorIngresado = Utilities.ValueEnter,
                CantidadDevolucion = 0,
                EstadoTransaccion = "En proceso"
            });
        }

        private void FinishPayment(decimal valueInto)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ValidatePayment(valueInto);

                    payModel.ImgCancel = Visibility.Hidden;
                    payModel.ImgEspereCambio = Visibility.Hidden;
                    payModel.ImgIngreseBillete = Visibility.Hidden;
                    payModel.ImgLeyendoBillete = Visibility.Hidden;
                    payModel.ImgRecibo = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("FinishPayment", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void ValidatePayment(decimal intoValue)
        {
            try
            {
                Utilities.ValueEnter = intoValue;
                if (intoValue > amount)
                {
                    log.Add(new Log
                    {
                        Fecha = DateTime.Now,
                        IDTrsansaccion = Utilities.IDTransactionDB,
                        Operacion = "Orden Devolucion Billetero",
                        ValorDevolver = decimal.Parse(payModel.ValorSobrante.Replace("$", "")),
                        ValorDevuelto = "0",
                        ValorPago = Utilities.ValueToPay,
                        ValorIngresado = decimal.Parse(payModel.ValorIngresado.Replace("$", "")),
                        CantidadDevolucion = 0,
                        EstadoTransaccion = "En proceso"
                    });

                    decimal value = intoValue - amount;
                    Utilities.ValueReturn = value;
                    pay.callbackReturn = valueDispenser =>
                    {
                        SendFinish();
                    };

                    pay.StartReturn(value);
                }
                else
                {
                    SendFinish();
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("ValidatePayment", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        public async void SendFinish()
        {
            try
            {
                var valueInto = decimal.Parse(payModel.ValorIngresado.Replace("$", ""));
                FillLog("Llamando el servicio para extraer el certificado");
                Utilities.BuyID = await camaraComercio.ConfirmarCompra();
                FillLog("El servicio respondió: " + Utilities.BuyID);
                //camaraComercio.Print("h");
                if (!Utilities.BuyID.Equals("0") && !string.IsNullOrEmpty(Utilities.BuyID))
                {
                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        FinishPayment frmInformationCompany = new FinishPayment(pay,valueInto);
                        frmInformationCompany.Show();
                        this.Close();
                    });
                    FillLog("Llamó al formulario frmFinalizar");
                }
                else
                {
                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        FrmModal modal = new FrmModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                            "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                        "Comuniquese con servicio al cliente o diríjase a las taquillas."), this);
                        modal.ShowDialog();
                        if (modal.DialogResult.Value)
                        {
                            Utilities.ValueEnter = valueInto;
                            Utilities.ValueReturn = Math.Floor(Utilities.ValueToPay);
                            GotoCancel();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("SendFinish", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void TimerTime()
        {
            try
            {
                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Tick += (s, a) =>
                {
                    ChangeView(pay.GetStatus());
                };

                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("TimerTime", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void ChangeView(string statusPay)
        {
            try
            {
                if (statusPay == null)
                {
                    return;
                }

                if (statusPay.Contains("LEYENDO"))
                {
                    payModel.ImgRecibo = Visibility.Hidden;
                    payModel.ImgLeyendoBillete = Visibility.Visible;
                    payModel.ImgEspereCambio = Visibility.Hidden;
                    payModel.ImgIngreseBillete = Visibility.Hidden;
                    payModel.ImgCancel = Visibility.Hidden;
                }
                else if (statusPay.Contains("ALMACENADO") || statusPay.Contains("BODEGA"))
                {
                    if (double.Parse(payModel.ValorFaltante.Replace("$", "")) > 0)
                    {
                        payModel.ImgRecibo = Visibility.Hidden;
                        payModel.ImgLeyendoBillete = Visibility.Hidden;
                        payModel.ImgEspereCambio = Visibility.Hidden;
                        payModel.ImgIngreseBillete = Visibility.Visible;
                        payModel.ImgCancel = Visibility.Visible;
                    }
                    else if (double.Parse(payModel.ValorSobrante.Replace("$", "")) > 0)
                    {
                        payModel.ImgRecibo = Visibility.Hidden;
                        payModel.ImgLeyendoBillete = Visibility.Hidden;
                        payModel.ImgEspereCambio = Visibility.Visible;
                        payModel.ImgIngreseBillete = Visibility.Hidden;
                        payModel.ImgCancel = Visibility.Hidden;
                    }
                    else
                    {
                        payModel.ImgRecibo = Visibility.Visible;
                        payModel.ImgLeyendoBillete = Visibility.Hidden;
                        payModel.ImgEspereCambio = Visibility.Hidden;
                        payModel.ImgIngreseBillete = Visibility.Hidden;
                        payModel.ImgCancel = Visibility.Hidden;

                    }
                }
                else if (statusPay.Contains("INGRESANDO"))
                {
                    payModel.ImgRecibo = Visibility.Hidden;
                    payModel.ImgLeyendoBillete = Visibility.Visible;
                    payModel.ImgEspereCambio = Visibility.Hidden;
                    payModel.ImgIngreseBillete = Visibility.Hidden;
                    payModel.ImgCancel = Visibility.Hidden;
                }
                else if (statusPay.Contains("NO SE PUDO CONECTAR"))
                {
                    HideImages();
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("ChangeView", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void HideImages()
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    payModel.ImgRecibo = Visibility.Hidden;
                    payModel.ImgLeyendoBillete = Visibility.Hidden;
                    payModel.ImgEspereCambio = Visibility.Hidden;
                    payModel.ImgIngreseBillete = Visibility.Visible;
                    payModel.ImgCancel = Visibility.Visible;
                }));

            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("HideImages", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void SetMountInsert(string value) => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                payModel.ValorIngresado = string.Format("{0:C0}", decimal.Parse(value));
                decimal faltante = amount - decimal.Parse(value);
                decimal restante = 0;
                if (faltante < 0)
                {
                    restante = faltante * (-1);
                    faltante = 0;
                }

                payModel.ValorFaltante = string.Format("{0:C0}", faltante);
                payModel.ValorSobrante = string.Format("{0:C0}", restante);
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("SetMountInsert", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        });

        private void BtnCancel_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                this.Opacity = 0.5;
                FrmModalConfirmation frmConfirmation = new FrmModalConfirmation("¿Está seguro que desea cancelar la transacción?");
                frmConfirmation.ShowDialog();
                this.Opacity = 1;
                if (frmConfirmation.DialogResult.Value && frmConfirmation.DialogResult.HasValue)
                {
                    var valueInto = decimal.Parse(payModel.ValorIngresado.Replace("$", ""));
                    Utilities.ValueEnter = valueInto;
                    if (valueInto != 0)
                    {
                        Utilities.ValueReturn = valueInto;
                        GotoCancel();
                    }
                    else
                    {
                        pay.Finish();
                        //Utilities.GoToInicial();
                        Utilities.RestartApp();
                    }
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("BtnCancel_StylusDown", "FrmPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
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
