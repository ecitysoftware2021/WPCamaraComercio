using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPCamaraComercio.Service;

namespace WPCamaraComercio.Classes
{
    public class ControlPeripherals
    {
        #region References

        #region SerialPorts

        private SerialPort _serialPortBills;//Puerto billeteros

        private SerialPort _serialPortCoins;//Puerto Monederos

        #endregion

        #region CommandsPorts

        private string _StartBills = "OR:START";//Iniciar los billeteros

        private string _AceptanceBillOn = "OR:ON:AP";//Operar billetero Aceptance

        private string _DispenserBillOn = "OR:ON:DP";//Operar billetero Dispenser

        private string _AceptanceBillOFF = "OR:OFF:AP";//Cerrar billetero Aceptance

        private string _AceptanceCoinOn = "OR:ON:MA";//Operar Monedero Aceptance

        private string _DispenserCoinOn = "OR:ON:MD:";//Operar Monedero Dispenser

        private string _AceptanceCoinOff = "OR:OFF:MA";//Cerrar Monedero Aceptance

        #endregion

        #region Callbacks

        public Action<decimal> callbackValueIn;//Calback para cuando ingresan un billete

        public Action<decimal> callbackValueOut;//Calback para cuando sale un billete

        public Action<decimal> callbackTotalIn;//Calback para cuando se ingresa la totalidad del dinero

        public Action<decimal> callbackTotalOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackOut;//Calback para cuando sale cieerta cantidad del dinero

        public Action<string> callbackError;//Calback de error

        public Action<string> callbackMessage;//Calback de mensaje

        public Action<bool> callbackToken;//Calback de mensaje


        #endregion

        #region EvaluationValues

        private static int _mil = 1000;
        private static int _hundred = 100;

        #endregion

        #region Variables
        //private WCFPayPadService payPadService;

        private decimal payValue;//Valor a pagar

        private decimal enterValue;//Valor ingresado

        private decimal deliveryValue;//Valor entregado

        private decimal dispenserValue;//Valor a dispensar

        private bool stateError = false;//Si sucede algun error en un periférico, se pone en true

        private static string TOKEN;//Llave que retorna el dispenser

        public string LogMessage;//Mensaje para el log

        public static LogDispenser log;//Log del dispenser

        #endregion

        #endregion

        #region LoadMethods

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ControlPeripherals()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ControlPeripherals: ", "Ingresé");
                _serialPortBills = new SerialPort();
                _serialPortCoins = new SerialPort();
                log = new LogDispenser();
                InitPortBills();
                InitPortPurses();
                //payPadService = new WCFPayPadService();
                LogService.CreateLogsPeticionRespuestaDispositivos("ControlPeripherals: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ControlPeripherals: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método que inicializa los billeteros
        /// </summary>
        public void Start()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("Start: ", "Ingresé");
                SendMessageBills(_StartBills);
                LogService.CreateLogsPeticionRespuestaDispositivos("Start: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("Start: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método usado para iniciar los valores de la clase
        /// </summary>
        public void StartValues()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StartValues: ", "Ingresé");
                deliveryValue = 0;//valor entregado TEST
                enterValue = 0;//valor ingresado
                deliveryVal = 0;//valor entregado
                LogMessage = string.Empty;//Mensaje del log del dispenser
                LogService.CreateLogsPeticionRespuestaDispositivos("StartValues: ", "Ingresé");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StartValues: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("InitPortBills: ", "Ingresé");
                if (!_serialPortBills.IsOpen)
                {
                    _serialPortBills.PortName = Utilities.GetConfiguration("PortBills");
                    _serialPortBills.ReadTimeout = 3000;
                    _serialPortBills.WriteTimeout = 500;
                    _serialPortBills.BaudRate = 57600;
                    _serialPortBills.Open();
                }

                _serialPortBills.DataReceived += new SerialDataReceivedEventHandler(_serialPortBillsDataReceived);
                LogService.CreateLogsPeticionRespuestaDispositivos("InitPortBills: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("InitPortBills: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        ///  Método para inciar el puerto de los monederos
        /// </summary>
        private void InitPortPurses()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("InitPortPurses: ", "Ingresé");
                if (!_serialPortCoins.IsOpen)
                {
                    _serialPortCoins.PortName = Utilities.GetConfiguration("PortCoins");
                    _serialPortCoins.ReadTimeout = 3000;
                    _serialPortCoins.WriteTimeout = 500;
                    _serialPortCoins.BaudRate = 57600;
                    _serialPortCoins.Open();
                }

                _serialPortCoins.DataReceived += new SerialDataReceivedEventHandler(_serialPortCoinsDataReceived);
                LogService.CreateLogsPeticionRespuestaDispositivos("InitPortPurses: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("InitPortPurses: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region SendMessage

        /// <summary>
        /// Método para enviar orden al puerto de los billeteros
        /// </summary>
        /// <param name="message">mensaje a enviar</param>
        private void SendMessageBills(string message)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("SendMessageBills: ", "Ingresé");
                if (_serialPortBills.IsOpen)
                {
                    Thread.Sleep(2000);
                    _serialPortBills.Write(message);
                    log.SendMessage += string.Format("Billetero: {0}\n", message);
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("SendMessageBills: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("SendMessageBills: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método para enviar orden al puerto de los monederos
        /// </summary>
        /// <param name="message">mensaje a enviar</param>
        private void SendMessageCoins(string message)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("SendMessageCoins: ", "Ingresé");
                if (_serialPortCoins.IsOpen)
                {
                    Thread.Sleep(2000);
                    _serialPortCoins.Write(message);
                    log.SendMessage += string.Format("Monedero: {0}\n", message);
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("SendMessageCoins: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("SendMessageCoins: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region Listeners

        /// <summary>
        /// Método que escucha la respuesta del puerto del billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPortBillsDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("_serialPortBillsDataReceived: ", "Ingresé");
                string response = _serialPortBills.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    log.ResponseMessage += string.Format("Respuesta Billetero:{0}\n", response);
                    ProcessResponseBills(response);
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("_serialPortBillsDataReceived: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("_serialPortBillsDataReceived: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método que escucha la respuesta del puerto del billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPortCoinsDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("_serialPortCoinsDataReceived: ", "Ingresé");
                string response = _serialPortCoins.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    log.ResponseMessage += string.Format("Respuesta Monedero: {0}\n", response);
                    ProcessResponseCoins(response);
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("_serialPortCoinsDataReceived: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("_serialPortCoinsDataReceived: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region ProcessResponse

        /// <summary>
        /// Método que procesa la respuesta del puerto de los billeteros
        /// </summary>
        /// <param name="message">respuesta del puerto de los billeteros</param>
        private void ProcessResponseBills(string message)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessResponseBills: ", "Ingresé");
                string[] response = message.Split(':');
                switch (response[0])
                {
                    case "RC":
                        ProcessRC(response);
                        break;
                    case "ER":
                        try
                        {
                            LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: Respuesta del billetero: ", message);
                        }
                        catch { }
                        ProcessER(response);
                        break;
                    case "UN":
                        ProcessUN(response);
                        break;
                    case "TO":
                        ProcessTO(response);
                        break;
                    default:
                        break;
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessResponseBills: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessResponseBills: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Método que procesa la respuesta del puerto de los monederos
        /// </summary>
        /// <param name="message">respuesta del puerto de los monederos</param>
        private void ProcessResponseCoins(string message)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessResponseCoins: ", "Ingresé");
                string[] response = message.Split(':');
                switch (response[0])
                {
                    case "RC":
                        ProcessRC(response);
                        break;
                    case "ER":
                        try
                        {
                            LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: Respuesta del billetero: ", message);
                        }
                        catch { }
                        ProcessER(response);
                        break;
                    case "UN":
                        ProcessUN(response);
                        break;
                    case "TO":
                        ProcessTO(response);
                        break;
                    default:
                        break;
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessResponseCoins: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessResponseCoins: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region ProcessResponseCases

        /// <summary>
        /// Respuesta para el caso de Recepción de un mensaje enviado
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessRC(string[] response)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessRC: ", "Ingresé");
                if (response[1] == "OK")
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessRC: ", "Ingresé al primer if");
                    switch (response[2])
                    {
                        case "AP":

                            break;
                        case "DP":
                            if (response[3] == "HD" && !string.IsNullOrEmpty(response[4]))
                            {
                                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessRC: ", "Ingresé al segungo if");
                                TOKEN = response[4].Replace("\r", string.Empty);
                                callbackToken?.Invoke(true);
                            }
                            break;
                        default:
                            break;
                    }
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessRC: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessRC: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Respuesta para el caso de error
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessER(string[] response)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessER: ", "Ingresé");
                if (response[1] == "DP" || response[1] == "MD")
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessER: ", "Ingresé al primer if");
                    //stateError = true;
                    callbackError?.Invoke(string.Concat("Error, se alcanzó a entregar:", deliveryValue));
                }
                if (response[1] == "AP")
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessER: ", "Ingresé al segundo if");
                   // stateError = true;
                    callbackError?.Invoke("Error, en el billetero Aceptance");
                }
                else if (response[1] == "FATAL")
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessER: ", "Ingresé al else if");
                    Utilities.GoToInicial();
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessER: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessER: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Respuesta para el caso de ingreso o salida de un billete/moneda
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessUN(string[] response)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Ingresé");
                if (response[1] == "DP")
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Ingresé al if DP");
                    deliveryValue += decimal.Parse(response[2]) * _mil;
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]) * _mil);
                    int idDenominacion = Utilities.getDescriptionEnum(response[2].Replace("\r", string.Empty));
                    Utilities.log.Add(new LogTransactional
                    {
                        Fecha = DateTime.Now,
                        IDTrsansaccion = Utilities.IDTransactionDB,
                        Operacion = "Devolviendo Billetes",
                        ValorDevolver = dispenserValue,
                        ValorDevuelto = (decimal.Parse(response[2]) * _mil).ToString(),
                        ValorPago = Utilities.ValueToPay,
                        ValorIngresado = Utilities.EnterTotal,
                        CantidadDevolucion = 1,
                        EstadoTransaccion = "En Proceso"
                    });

                    //payPadService.InsertarControlDispenser(idDenominacion, Utilities.CorrespondentId2, 0, int.Parse(response[2]));
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Salí del if DP");

                }
                else if (response[1] == "MD")
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Ingresé al else if MD");
                    decimal moneda = decimal.Parse(response[2]);
                    deliveryValue += decimal.Parse(response[2]) * _hundred;
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]) * _hundred);
                    string value = response[2].Replace("\r", string.Empty);
                    if (moneda == 100)
                    {
                        value = response[2].Replace("\r", string.Empty) + "M";
                    }
                    int idDenominacion = Utilities.getDescriptionEnum(value);
                    Utilities.log.Add(new LogTransactional
                    {
                        Fecha = DateTime.Now,
                        IDTrsansaccion = Utilities.IDTransactionDB,
                        Operacion = "Devolviendo Monedas",
                        ValorDevolver = dispenserValue,
                        ValorDevuelto = (decimal.Parse(response[2]) * _hundred).ToString(),
                        ValorPago = Utilities.ValueToPay,
                        ValorIngresado = Utilities.EnterTotal,
                        CantidadDevolucion = 1,
                        EstadoTransaccion = "En Proceso"
                    });


                    //payPadService.InsertarControlMonedas(idDenominacion, Utilities.CorrespondentId2, 0, int.Parse(response[2]));
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Salí del else if MD");

                }
                else
                {
                    if (response[1] == "AP")
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Ingresé al if AP");
                        enterValue += decimal.Parse(response[2]) * _mil;
                        callbackValueIn?.Invoke(Convert.ToDecimal(response[2]) * _mil);

                        int idDenominacion = Utilities.getDescriptionEnum(response[2].Replace("\r", string.Empty));
                        Utilities.log.Add(new LogTransactional
                        {
                            Fecha = DateTime.Now,
                            IDTrsansaccion = Utilities.IDTransactionDB,
                            Operacion = "Aceptando Billetes",
                            ValorDevolver = 0,
                            ValorDevuelto = "0",
                            ValorPago = Utilities.ValueToPay,
                            ValorIngresado = decimal.Parse(response[2]) * _mil,
                            CantidadDevolucion = 0,
                            EstadoTransaccion = "En Proceso"
                        });

                        //payPadService.InsertarControlAceptance(idDenominacion, Utilities.CorrespondentId2, 1);
                        LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Salí del if AP");

                    }
                    else if (response[1] == "MA")
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Ingresé al else if MA");
                        decimal moneda = decimal.Parse(response[2]);
                        enterValue += moneda;
                        callbackValueIn?.Invoke(Convert.ToDecimal(response[2]));
                        string value = response[2].Replace("\r", string.Empty);
                        if (moneda == 100)
                        {
                            value = response[2].Replace("\r", string.Empty) + "M";
                        }
                        int idDenominacion = Utilities.getDescriptionEnum(value);

                        Utilities.log.Add(new LogTransactional
                        {
                            Fecha = DateTime.Now,
                            IDTrsansaccion = Utilities.IDTransactionDB,
                            Operacion = "Aceptando Monedas",
                            ValorDevolver = 0,
                            ValorDevuelto = "0",
                            ValorPago = Utilities.ValueToPay,
                            ValorIngresado = decimal.Parse(response[2]),
                            CantidadDevolucion = 0,
                            EstadoTransaccion = "En Proceso"
                        });

                        //payPadService.InsertarControlAceptance(idDenominacion, Utilities.CorrespondentId2, 1);
                        LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Salí del else if MA");
                    }

                    ValidateEnterValue();
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessUN: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Respuesta para el caso de total cuando responde el billetero/monedero dispenser
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessTO(string[] response)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Ingresé");
                string responseFull;
                if (response[1] == "OK")
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Ingresé al if OK");
                    responseFull = string.Concat(response[2], ":", response[3]);
                    if (response[2] == "DP")
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Ingresé al if OK DP");
                        ConfigDataDispenser(responseFull, 1);
                    }

                    if (response[2] == "MD")
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Ingresé al if OK MD");
                        ConfigDataDispenser(responseFull);
                    }
                }
                else
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Ingresé al esle");
                    responseFull = string.Concat(response[2], ":", response[3]);
                    if (response[2] == "DP")
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Ingresé al if DP del esle");
                        ConfigDataDispenser(responseFull, 2);
                    }
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ProcessTO: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region Dispenser

        /// <summary>
        /// Inicia el proceso paara el billetero dispenser
        /// </summary>
        /// <param name="valueDispenser">valor a dispensar</param>
        public void StartDispenser(decimal valueDispenser)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StartDispenser: ", "Ingresé");
                dispenserValue = valueDispenser;
                ConfigurateDispenser();
                LogService.CreateLogsPeticionRespuestaDispositivos("StartDispenser: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StartDispenser: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Configura el valor a dispensar para distribuirlo entre monedero y billetero
        /// </summary>
        private void ConfigurateDispenser()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ConfigurateDispenser: ", "Ingresé");
                if (dispenserValue > 0)
                {
                    int amountCoins = Convert.ToInt32(dispenserValue % _mil);
                    decimal amountBills = dispenserValue - amountCoins;
                    if (amountBills >= 2000)
                    {
                        decimal valuePay = amountBills / _mil;
                        if (valuePay % 2 != 0)
                        {
                            valuePay--;
                            amountCoins += _mil;
                        }

                        DispenserMoney(valuePay.ToString());
                        if (amountCoins > 0)
                        {
                            SendMessageCoins(_DispenserCoinOn + (amountCoins / _hundred).ToString());
                        }
                    }
                    else
                    {
                        decimal valuePayCoin = dispenserValue / _hundred;
                        SendMessageCoins(_DispenserCoinOn + valuePayCoin.ToString());
                    }
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ConfigurateDispenser: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ConfigurateDispenser: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Enviar la orden de dispensar al billetero
        /// </summary>
        /// <param name="valuePay"></param>
        private void DispenserMoney(string valuePay)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("DispenserMoney: ", "Ingresé");
                if (!string.IsNullOrEmpty(TOKEN))
                {
                    string message = string.Format("{0}:{1}:{2}", _DispenserBillOn, TOKEN, valuePay);
                    SendMessageBills(message);
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("DispenserMoney: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("DispenserMoney: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region Aceptance

        /// <summary>
        /// Inicia la operación de billetero aceptance
        /// </summary>
        /// <param name="payValue">valor a pagar</param>
        public void StartAceptance(decimal payValue)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StartAceptance: ", "Ingresé");
                this.payValue = payValue;
                SendMessageBills(_AceptanceBillOn);
                SendMessageCoins(_AceptanceCoinOn);
                LogService.CreateLogsPeticionRespuestaDispositivos("StartAceptance: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StartAceptance: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Valida el valor que ingresa
        /// </summary>
        private void ValidateEnterValue()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ValidateEnterValue: ", "Ingresé");
                decimal enterVal = enterValue;
                if (enterValue >= payValue)
                {
                    StopAceptance();
                    enterValue = 0;
                    callbackTotalIn?.Invoke(enterVal);
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ValidateEnterValue: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ValidateEnterValue: ", "Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Para la aceptación de dinero
        /// </summary>
        public void StopAceptance()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StopAceptance: ", "Ingresé");
                SendMessageBills(_AceptanceBillOFF);
                SendMessageCoins(_AceptanceCoinOff);
                LogService.CreateLogsPeticionRespuestaDispositivos("StopAceptance: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("StopAceptance: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region Responses

        public decimal deliveryVal;
        /// <summary>
        /// Procesa la respuesta de los dispenser M y B
        /// </summary>
        /// <param name="data">respuesta</param>
        /// <param name="isRj">si se fue o no al reject</param>
        private void ConfigDataDispenser(string data, int isBX = 0)
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Ingresé");
                string[] values = data.Split(':')[1].Split(';');
                if (isBX < 2)
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al primer if");
                    foreach (var value in values)
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al foreach");
                        int denominacion = int.Parse(value.Split('-')[0]);
                        int cantidad = int.Parse(value.Split('-')[1]);
                        deliveryVal += denominacion * cantidad;
                        if (denominacion < 6000)
                        {
                            LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al if en foreach");
                            denominacion = int.Parse(denominacion.ToString().Substring(0, 1));
                        }
                        else
                        {
                            LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al else en foreach");
                            denominacion = int.Parse(denominacion.ToString().Substring(0, 2));
                        }
                        int idDenominacion = Utilities.getDescriptionEnum(denominacion.ToString());
                    }
                    LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Salí del foreach");
                }

                if (isBX == 0 || isBX == 2)
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al segundo if");
                    LogMessage += string.Concat(data.Replace("\r", string.Empty), "!");
                }

                if (!stateError)
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al tercer if");
                    //if (deliveryVal == 50000) { deliveryVal = 2000; }

                    if (dispenserValue == deliveryVal)
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al if del tercer if");
                        if (isBX == 2 || isBX == 0)
                        {
                            LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al if del if del tercer if");
                            callbackTotalOut?.Invoke(deliveryVal);
                        }
                    }
                }
                else
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al else del tercer if");
                    if (isBX == 2)
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Entré al if del else del tercer if");
                        callbackOut?.Invoke(deliveryVal);
                    }
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ConfigDataDispenser: ", "Error: " + ex.ToString());
            }
        }

        #endregion

        #region Finish

        /// <summary>
        /// Cierra los puertos
        /// </summary>
        public void ClosePorts()
        {
            try
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ClosePorts: ", "Ingresé");
                if (_serialPortBills.IsOpen)
                {
                    _serialPortBills.Close();
                }

                if (_serialPortCoins.IsOpen)
                {
                    _serialPortCoins.Close();
                }
                LogService.CreateLogsPeticionRespuestaDispositivos("ClosePorts: ", "Salí");
            }
            catch (Exception ex)
            {
                LogService.CreateLogsPeticionRespuestaDispositivos("ClosePorts: ", "Error: " + ex.ToString());
            }
        }

        #endregion
    }
}
