﻿using System;
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

        private WCFPayPadService payPadService;

        private FrmLoading frmLoading;

        private Utilities utilities;

        private LogErrorGeneral logError;

        private Record recorder;//Instancia

        private int count;//Contador utilizado para los reintentos en UpdateTrans

        private int tries;//Contador utilizado para los reintentos en SavePay
        private bool stateUpdate;

        #endregion

        #region LoadMethods

        //public FrmPay(SaveViewModel update)
        //{
        //    InitializeComponent();
        //    services = new WCFServices();
        //    frmLoading = new FrmLoading();
        //    recorder = new CLSGrabador();
        //    utilities = new Utilities();
        //    logError = new LogErrorGeneral
        //    {
        //        Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
        //        IDCorresponsal = Utilities.CorrespondentId,
        //        IdTransaction = Utilities.IDTransactionDB,
        //        UserId = InfoUserPass.Identification,
        //        ValuePay = Utilities.PayVal,
        //    };
        //    OrganizeValues();
        //    this.update = update;
        //    count = 0;
        //    tries = 0;
        //    stateUpdate = true;
        //    InfoUserPass.PayWay = "E";
        ////}

        public FrmPayment()
        {
            InitializeComponent();
            //Utilities.ValueToPay = value;
            OrganizeValues();
            services = new WCFServices();
            payPadService = new WCFPayPadService();
            frmLoading = new FrmLoading();
            recorder = new Record();
            utilities = new Utilities();
            logError = new LogErrorGeneral
            {
                Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                IDCorresponsal = Utilities.CorrespondentId,
                IdTransaction = Utilities.IDTransactionDB,
                //UserId = InfoUserPass.Identification,
                ValuePay = Utilities.ValueToPay,
            };
            count = 0;
            tries = 0;
            stateUpdate = true;
            //InfoUserPass.PayWay = "E";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //recorder.Grabar(Utilities.IDTransactionDB);
            ActivateWallet();
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
                    UpdateTransaction(4);
                });
                if (PaymentViewModel.ValorIngresado > 0)
                {
                    //Utilities.DispenserVal = PaymentViewModel.ValorIngresado;
                    Utilities.Loading(frmLoading, true, this);
                    //ReturnMoney(Utilities.DispenserVal);
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
           // lblValorPagar.Content = string.Format("{0:C0}", Utilities.ValueToPay);
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
            UpdateTransaction(4);
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
                ApproveTrans();
                //recorder.FinalizarGrabacion();
                //InfoUserPass.CollectionVal = Convert.ToInt16(Utilities.PayVal);
                //await SavePay();
                Dispatcher.BeginInvoke((Action)delegate { Utilities.Loading(frmLoading, false, this); });
                string message = string.Concat("Reserva realizada con éxito", Environment.NewLine, "gracias por utilizar nuestro servicio,", Environment.NewLine, "espere mientras se imprimen los boleto.");
                Utilities.OpenModal(message, this, true);
                //await Task.Run(() =>
                //{
                //    Utilities.PrintC(new PrintGenericViewModel
                //    {
                //        Cod_Transaction = Utilities.IDTransactionDB.ToString(),
                //        DateTramite = DateTime.Now.ToString(),
                //        Identification = "123414124",
                //        ReturnedVal = Utilities.ValueDelivery.ToString(),
                //        State = "Aprobada",
                //        Value = Utilities.PayVal.ToString(),
                //        Tramite = Utilities.GetTramite(Utilities.Operation)
                //    });//print tickets    
                //    //PrintTickets(answer.facturas);
                //});
                Thread.Sleep(3000);
                Utilities.ResetTimer();
                await Dispatcher.BeginInvoke((Action)delegate
                {
                    FinishPayment finishPayment = new FinishPayment();
                    finishPayment.Show();
                    Close();
                });
            }
            catch (Exception ex)
            {
                ErroUpdateTrans(ex.Message);
                await FinishPayment();
            }
        }

        /// <summary>
        /// Método encargado de actualizar la transacción a aprobada, se llama en finalizar pago En caso de fallo se reintenta dos veces más actualizar el estado de la transacción, si el error persiste se guarda en un log local y en el servidor, seguido de esto se continua con la transacción normal
        /// </summary>
        private void ApproveTrans()
        {
            try
            {
                if (stateUpdate)
                {
                    var state = UpdateTransaction(2);
                    if (!state)
                    {
                        if (count < 2)
                        {
                            count++;
                            ApproveTrans();
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
            }
            catch (Exception ex)
            {
                stateUpdate = false;
                throw ex;
            }
        }

        /// <summary>
        /// Método encargado de actualizar la transacción a cualquiera de los estados posibles
        /// </summary>
        /// <param name="state">Id del estado al cual queremos actualizar la transacción</param>
        /// <returns></returns>
        private bool UpdateTransaction(int state)
        {
            try
            {
                bool response = false;
                if (stateUpdate)
                {
                    switch (state)
                    {
                        case 1://Iniciada
                            response = payPadService.WCFPayPad.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Iniciada);
                            break;
                        case 2://Aprobada
                            response = payPadService.WCFPayPad.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Aprobada);
                            break;
                        case 4://Cancelada
                            response = payPadService.WCFPayPad.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                            break;
                        default://Cancelada
                            response = payPadService.WCFPayPad.ActualizarEstadoTransaccion(Utilities.IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                            break;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///// <summary>
        ///// Guarda el pago
        ///// </summary>
        //private async Task<bool> SavePay()
        //{
        //    string json;
        //    var task = services.SavePay(update);
        //    if (await Task.WhenAny(task, Task.Delay(10000)) == task)
        //    {
        //        var response = task.Result;
        //        var answer = (respuestaConfirmacionPago)response.Result;
        //        if (answer != null)
        //        {
        //            if (!string.IsNullOrEmpty(answer.error))
        //            {
        //                Utilities.OpenModal(string.Concat("Lo sentimos, ",
        //                    Environment.NewLine,
        //                    "Ha ocurrido un error.",
        //                    Environment.NewLine,
        //                    "Error: ", answer.error), this);
        //                json = Utilities.CreateJSON();
        //                logError.Description = json + "\nOcurrio un error: " + answer.error;
        //                logError.State = "Aprobada";
        //                Utilities.SaveLogTransactions(logError, "LogTransacciones\\AprobadasError");
        //                return false;
        //            }

        //            if (response.IsSuccess)
        //            {
        //                this.answer = answer;
        //                json = Utilities.CreateJSON();
        //                logError.Description = json + "\nTransacción Exitosa";
        //                logError.State = "Aprobada";
        //                Utilities.SaveLogTransactions(logError, "LogTransacciones\\Aprobadas");
        //                return true;
        //            }
        //        }

        //        json = Utilities.CreateJSON();
        //        logError.Description = json + "\nOcurrio un error al guardar el pago en Confa";
        //        logError.State = "Aprobada";
        //        Utilities.SaveLogTransactions(logError, "LogTransacciones\\AprobadasError");
        //        return false;
        //    }
        //    else
        //    {
        //        if (tries < 1)
        //        {
        //            tries++;
        //            await SavePay();
        //        }

        //        json = Utilities.CreateJSON();
        //        logError.Description = json + "\nOcurrió un error al guardar el pago en Confa";
        //        logError.State = "Aprobada";
        //        Utilities.SaveLogTransactions(logError, "LogTransacciones\\Aprobadas");
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Método encargado de mandar a imprimir cada factura
        ///// </summary>
        ///// <param name="Invoices">Lista de facturas a imprimir</param>
        //private void PrintTickets(personaFactura[] Invoices)
        //{
        //    foreach (var invoice in Invoices)
        //    {
        //        PrintInvoiceViewModel print = new PrintInvoiceViewModel
        //        {
        //            Category = invoice.categoria,
        //            ConceptoValue = invoice.valorConcepto,
        //            CountableDocument = invoice.numeroDocumentoContable,
        //            DateEnter = invoice.fechaIngreso,
        //            Document = invoice.documento,
        //            DocumentType = invoice.tipoDocumento,
        //            Name = invoice.nombrePersona,
        //            NameConcept = invoice.nombreConcepto,
        //            NameIndustry = invoice.nombreEmpresa,
        //            NameSede = invoice.nombreSede,
        //            NIT = invoice.nit,
        //            Paydesk = invoice.caja,
        //            Politic = invoice.politica,
        //            Rank = invoice.rango,
        //            Resolution = invoice.resolucion,
        //            Text1 = invoice.texto1,
        //            Text2 = invoice.texto2,
        //            Text3 = invoice.texto3,
        //            TicketType = invoice.tipoEntrada,
        //            Value = invoice.valorTotal
        //        };

        //        Utilities.PrintTickets(print);
        //    }
        //}

        #endregion
    }
}
