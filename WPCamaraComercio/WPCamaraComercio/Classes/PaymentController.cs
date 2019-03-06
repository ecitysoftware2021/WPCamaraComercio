using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPCamaraComercio.Classes.Smart;
using WPCamaraComercio.Service;

namespace WPCamaraComercio.Classes
{
    public class PaymentController
    {
        #region Referencias

        private int idCorrespo;

        private bool statePay = false;

        private decimal intoValue = 0;

        private decimal paymentValue = 0;

        private decimal returnValue = 0;

        private decimal dispenserValue = 0;

        public Action<decimal> callback;

        public Action<decimal> callbackValue;

        public Action<decimal> callbackReturn;

        public Action<string> callbackError;

        private Payout payout;

        WCFServices wCFService;

        private SmartCoins smartCoins;

        public ReturnObject returnObject;

        WCFPayPadService WCFPayPadService;

        Utilities utilities;

        #endregion

        public PaymentController()
        {
            try
            {
                this.idCorrespo = Utilities.CorrespondentId;
                this.payout = new Payout();
                this.smartCoins = new SmartCoins();
                wCFService = new WCFServices();
                WCFPayPadService = new WCFPayPadService();
                utilities = new Utilities();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("PaymentController", "PaymentController", ex.ToString());
            }
        }

        public void Start(decimal paymentValue)
        {
            try
            {
                this.paymentValue = paymentValue;

                returnObject = new ReturnObject
                {
                    state = false,
                    amount = 0,
                    tryPay = 0
                };

                Utilities.log.Add(new Log
                {
                    Fecha = DateTime.Now,
                    IDTrsansaccion = Utilities.IDTransactionDB,
                    Operacion = "Opereción Aceptar Dinero",
                    ValorDevolver = 0,
                    ValorDevuelto = "0",
                    ValorPago = Utilities.ValueToPay,
                    ValorIngresado = 0,
                    CantidadDevolucion = 0,
                    EstadoTransaccion = "En proceso"
                });

                InitSmartPayout();

                InitAcceptanceCoins();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("Start", "PaymentController", ex.ToString());
            }
        }

        private void InitAcceptanceCoins()
        {
            try
            {
                Task.Run(() =>
                    {
                        smartCoins.callbackValue = value =>
                        {
                            Utilities.log.Add(new Log
                            {
                                Fecha = DateTime.Now,
                                IDTrsansaccion = Utilities.IDTransactionDB,
                                Operacion = "Aceptando Monedas",
                                ValorDevolver = 0,
                                ValorDevuelto = "0",
                                ValorPago = Utilities.ValueToPay,
                                ValorIngresado = value,
                                CantidadDevolucion = 0,
                                EstadoTransaccion = "En proceso"
                            });

                            OperationAcceptance(value);
                        };
                        smartCoins.Start();
                    });
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("InitAcceptanceCoins", "PaymentController", ex.ToString());
            }
        }

        public void StartReturn(decimal returnValue)
        {
            try
            {
                this.returnValue = returnValue;

                if (!statePay)
                {
                    InitSmartPayout();
                }
                InitDispenser(returnValue);
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("StartReturn", "PaymentController", ex.ToString());
            }
        }

        public void InitSmartPayout()
        {
            try
            {
                Task.Run(() =>
                {
                    this.payout.callbackValue = value =>
                    {
                        if (!returnObject.state)
                        {
                            Utilities.log.Add(new Log
                            {
                                Fecha = DateTime.Now,
                                IDTrsansaccion = Utilities.IDTransactionDB,
                                Operacion = "Aceptando Billetes",
                                ValorDevolver = 0,
                                ValorDevuelto = "0",
                                ValorPago = Utilities.ValueToPay,
                                ValorIngresado = value,
                                CantidadDevolucion = 0,
                                EstadoTransaccion = "En proceso"
                            });

                            OperationAcceptance(value);
                        }
                    };
                    this.payout.callbackError = error =>
                    {
                        if (GetStatus() == "DISPENSER")
                        {
                            decimal value = payout.GetReturnMoney();
                            returnObject.amount = returnObject.amount - value;
                            dispenserValue = dispenserValue + value;
                            returnObject.state = true;
                        }
                    };
                    statePay = payout.IsStart();
                    if (statePay)
                    {
                        while (statePay)
                        {
                            // if the poll fails, try to reconnect
                            if (!payout.DoPoll())
                            {
                                if (!payout.IsStart())
                                {
                                    Thread.Sleep(1000);
                                    //statePay = false;
                                    //InitSmartPayout();
                                }
                            }
                            else
                            {
                                if (returnObject.state)
                                {
                                    returnObject.state = false;

                                    decimal dispenValue = payout.PayOut(returnObject.amount);

                                    if (dispenValue == returnObject.amount && returnObject.tryPay < 3)
                                    {
                                        returnObject.tryPay++;
                                        returnObject.state = true;
                                    }
                                    else if (dispenValue > 0)
                                    {
                                        returnObject.amount = returnObject.amount - dispenValue;
                                        ReturnMoney(dispenValue, 2);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        InitSmartPayout();
                    }
                });
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("InitSmartPayout", "PaymentController", ex.ToString());
            }
        }

        private void InitDispenser(decimal valueReturn)
        {
            try
            {
                int amountCoint = Convert.ToInt32(valueReturn % 1000);
                if (valueReturn >= 1000)
                {
                    var valueInBills = valueReturn - amountCoint;
                    if (amountCoint > 0)
                    {
                        ReturnMoney(amountCoint, 2);
                    }

                    ReturnMoney(valueInBills, 1);
                }
                else
                {
                    if (amountCoint > 0)
                    {
                        ReturnMoney(amountCoint, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("InitDispenser", "PaymentController", ex.ToString());
            }
        }

        private void ReturnMoney(decimal value, int type)
        {
            try
            {
                if (type == 1)
                {
                    payout.callback = valueDispenser =>
                    {
                        returnObject.state = false;
                        //returnObject.amount = 0;
                        Utilities.log.Add(new Log
                        {
                            Fecha = DateTime.Now,
                            IDTrsansaccion = Utilities.IDTransactionDB,
                            Operacion = "Orden Devolver Billetero",
                            ValorDevolver = valueDispenser,
                            ValorDevuelto = "0",
                            ValorPago = Utilities.ValueToPay,
                            ValorIngresado = Utilities.ValueEnter,
                            EstadoTransaccion = "En proceso"
                        });
                        OperationDispenser(returnObject.amount);
                    };


                    returnObject.state = true;
                    returnObject.amount = value;
                    returnObject.tryPay = 0;

                }
                else
                {
                    smartCoins.callbackTotal = total =>
                    {
                        Utilities.log.Add(new Log
                        {
                            Fecha = DateTime.Now,
                            IDTrsansaccion = Utilities.IDTransactionDB,
                            Operacion = "Orden Devolver Monedero",
                            ValorDevolver = total,
                            ValorDevuelto = "0",
                            ValorPago = Utilities.ValueToPay,
                            ValorIngresado = Utilities.ValueEnter,
                            EstadoTransaccion = "En proceso"
                        });
                        OperationDispenser(total);
                    };

                    smartCoins.ReturnCoins(value);
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("ReturnMoney", "PaymentController", ex.ToString());
            }
        }

        private void OperationDispenser(decimal devolucion)
        {
            try
            {
                dispenserValue = dispenserValue + devolucion;
                InsertDetails(devolucion, false);
                if (dispenserValue >= this.returnValue)
                {
                    //Finish();
                    this.callbackReturn?.Invoke(dispenserValue);
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("OperationDispenser", "PaymentController", ex.ToString());
            }
        }

        public string GetStatus()
        {
            try
            {
                return payout.GetStatus();
            }
            catch (Exception ex)
            {
                return "Error";
                utilities.SaveLogErrorMethods("GetStatus", "PaymentController", ex.ToString());
            }
        }

        private void OperationAcceptance(decimal value)
        {
            try
            {
                intoValue += value;
                InsertDetails(value, true);
                this.callbackValue?.Invoke(intoValue);

                if (intoValue >= paymentValue)
                {
                    if (intoValue == paymentValue)
                    {
                        //Finish();
                    }
                    this.callback?.Invoke(intoValue);
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("OperationAcceptance", "PaymentController", ex.ToString());
            }
        }

        public void Finish()
        {
            try
            {
                statePay = false;
                payout.Close();
                smartCoins.Stop();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("Finish", "PaymentController", ex.ToString());
            }
        }

        public List<Bills> ValidateBills()
        {
            try
            {
                statePay = payout.IsStart();
                if (statePay)
                {
                    List<Bills> listBills = payout.GetBills();
                    Finish();
                    if (listBills.Count > 0)
                    {
                        return listBills;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
                utilities.SaveLogErrorMethods("ValidateBills", "PaymentController", ex.ToString());
            }
        }

        private void InsertDetails(decimal value, bool IsPay)
        {
            try
            {
                Task.Run(() =>
                {
                    var data = (IsPay) ? WCFPayPad.CLSEstadoEstadoDetalle.Ingresando :
                                        WCFPayPad.CLSEstadoEstadoDetalle.Devolviendo;
                    WCFPayPadService.InsertarDetalleTransaccion(Utilities.IDTransactionDB, data, value);
                });
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("InsertDetails", "PaymentController", ex.ToString());
            }
        }
    }

    public class ReturnObject
    {
        public bool state { get; set; }
        public decimal amount { get; set; }
        public int tryPay { get; set; }
    }

}
