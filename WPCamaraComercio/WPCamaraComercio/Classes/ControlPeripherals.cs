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
        private WCFPayPadService payPadService;

        private decimal payValue;//Valor a pagar

        private decimal enterValue;//Valor ingresado

        private decimal deliveryValue;//Valor entregado

        private decimal dispenserValue;//Valor a dispensar

        private bool stateError;//Si sucede algun error en un periférico, se pone en true

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

                _serialPortBills = new SerialPort();
                _serialPortCoins = new SerialPort();
                log = new LogDispenser();
                InitPortBills();
                InitPortPurses();
                payPadService = new WCFPayPadService();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que inicializa los billeteros
        /// </summary>
        public void Start()
        {
            try
            {
                SendMessageBills(_StartBills);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método usado para iniciar los valores de la clase
        /// </summary>
        public void StartValues()
        {
            deliveryValue = 0;//valor entregado TEST
            enterValue = 0;//valor ingresado
            deliveryVal = 0;//valor entregado
            LogMessage = string.Empty;//Mensaje del log del dispenser
        }

        /// <summary>
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills()
        {
            try
            {
                if (!_serialPortBills.IsOpen)
                {
                    _serialPortBills.PortName = Utilities.GetConfiguration("PortBills");
                    _serialPortBills.ReadTimeout = 3000;
                    _serialPortBills.WriteTimeout = 500;
                    _serialPortBills.BaudRate = 57600;
                    _serialPortBills.Open();
                }

                _serialPortBills.DataReceived += new SerialDataReceivedEventHandler(_serialPortBillsDataReceived);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Método para inciar el puerto de los monederos
        /// </summary>
        private void InitPortPurses()
        {
            try
            {
                if (!_serialPortCoins.IsOpen)
                {
                    _serialPortCoins.PortName = Utilities.GetConfiguration("PortCoins");
                    _serialPortCoins.ReadTimeout = 3000;
                    _serialPortCoins.WriteTimeout = 500;
                    _serialPortCoins.BaudRate = 57600;
                    _serialPortCoins.Open();
                }

                _serialPortCoins.DataReceived += new SerialDataReceivedEventHandler(_serialPortCoinsDataReceived);
            }
            catch (Exception ex)
            {
                throw ex;
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
                if (_serialPortBills.IsOpen)
                {
                    Thread.Sleep(2000);
                    _serialPortBills.Write(message);
                    log.SendMessage += string.Format("Billetero: {0}\n", message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                if (_serialPortCoins.IsOpen)
                {
                    Thread.Sleep(2000);
                    _serialPortCoins.Write(message);
                    log.SendMessage += string.Format("Monedero: {0}\n", message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                string response = _serialPortBills.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    log.ResponseMessage += string.Format("Respuesta Billetero:{0}\n", response);
                    ProcessResponseBills(response);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                string response = _serialPortCoins.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    log.ResponseMessage += string.Format("Respuesta Monedero: {0}\n", response);
                    ProcessResponseCoins(response);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            string[] response = message.Split(':');
            switch (response[0])
            {
                case "RC":
                    ProcessRC(response);
                    break;
                case "ER":
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
        }

        /// <summary>
        /// Método que procesa la respuesta del puerto de los monederos
        /// </summary>
        /// <param name="message">respuesta del puerto de los monederos</param>
        private void ProcessResponseCoins(string message)
        {
            string[] response = message.Split(':');
            switch (response[0])
            {
                case "RC":
                    ProcessRC(response);
                    break;
                case "ER":
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
        }

        #endregion

        #region ProcessResponseCases

        /// <summary>
        /// Respuesta para el caso de Recepción de un mensaje enviado
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessRC(string[] response)
        {
            if (response[1] == "OK")
            {
                switch (response[2])
                {
                    case "AP":

                        break;
                    case "DP":
                        if (response[3] == "HD" && !string.IsNullOrEmpty(response[4]))
                        {
                            TOKEN = response[4].Replace("\r", string.Empty);
                            callbackToken?.Invoke(true);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Respuesta para el caso de error
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessER(string[] response)
        {
            if (response[1] == "DP" || response[1] == "MD")
            {
                stateError = true;
                callbackError?.Invoke(string.Concat("Error, se alcanzó a entregar:", deliveryValue));
            }
            if (response[1] == "AP")
            {
                stateError = true;
                callbackError?.Invoke("Error, en el billetero Aceptance");
            }
            else if (response[1] == "FATAL")
            {
                Utilities.GoToInicial();
            }
        }

        /// <summary>
        /// Respuesta para el caso de ingreso o salida de un billete/moneda
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessUN(string[] response)
        {
            if (response[1] == "DP")
            {
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


                payPadService.InsertarControlBilletes(idDenominacion, Utilities.CorrespondentId2, 0, int.Parse(response[2]));

            }
            else if (response[1] == "MD")
            {
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


                payPadService.InsertarControlBilletes(idDenominacion, Utilities.CorrespondentId2, 0, int.Parse(response[2]));

            }
            else
            {
                if (response[1] == "AP")
                {
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

                    payPadService.InsertarControlBilletes(idDenominacion, Utilities.CorrespondentId2, 1, int.Parse(response[2]));

                }
                else if (response[1] == "MA")
                {
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

                    payPadService.InsertarControlBilletes(idDenominacion, Utilities.CorrespondentId2, 1, int.Parse(response[2]));

                }

                ValidateEnterValue();
            }
        }

        /// <summary>
        /// Respuesta para el caso de total cuando responde el billetero/monedero dispenser
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessTO(string[] response)
        {
            string responseFull;
            if (response[1] == "OK")
            {
                responseFull = string.Concat(response[2], ":", response[3]);
                if (response[2] == "DP")
                {
                    ConfigDataDispenser(responseFull, 1);
                }

                if (response[2] == "MD")
                {
                    ConfigDataDispenser(responseFull);
                }
            }
            else
            {
                responseFull = string.Concat(response[2], ":", response[3]);
                if (response[2] == "DP")
                {
                    ConfigDataDispenser(responseFull, 2);
                }
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
                dispenserValue = valueDispenser;
                ConfigurateDispenser();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Configura el valor a dispensar para distribuirlo entre monedero y billetero
        /// </summary>
        private void ConfigurateDispenser()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                throw ex;
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
                if (!string.IsNullOrEmpty(TOKEN))
                {
                    string message = string.Format("{0}:{1}:{2}", _DispenserBillOn, TOKEN, valuePay);
                    SendMessageBills(message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                this.payValue = payValue;
                SendMessageBills(_AceptanceBillOn);
                SendMessageCoins(_AceptanceCoinOn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Valida el valor que ingresa
        /// </summary>
        private void ValidateEnterValue()
        {
            decimal enterVal = enterValue;
            if (enterValue >= payValue)
            {
                StopAceptance();
                enterValue = 0;
                callbackTotalIn?.Invoke(enterVal);
            }
        }

        /// <summary>
        /// Para la aceptación de dinero
        /// </summary>
        public void StopAceptance()
        {
            SendMessageBills(_AceptanceBillOFF);
            SendMessageCoins(_AceptanceCoinOff);
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
            string[] values = data.Split(':')[1].Split(';');
            if (isBX < 2)
            {
                foreach (var value in values)
                {
                    int denominacion = int.Parse(value.Split('-')[0]);
                    int cantidad = int.Parse(value.Split('-')[1]);
                    deliveryVal += denominacion * cantidad;
                    if (denominacion < 6000)
                    {
                        denominacion = int.Parse(denominacion.ToString().Substring(0, 1));
                    }
                    else
                    {
                        denominacion = int.Parse(denominacion.ToString().Substring(0, 2));
                    }
                    int idDenominacion = Utilities.getDescriptionEnum(denominacion.ToString());

                    payPadService.InsertarControlBilletes(idDenominacion, Utilities.CorrespondentId2, 1, cantidad);

                }
            }

            if (isBX == 0 || isBX == 2)
            {
                LogMessage += string.Concat(data.Replace("\r", string.Empty), "!");
            }

            if (!stateError)
            {
                //if (deliveryVal == 50000) { deliveryVal = 2000; }

                if (dispenserValue == deliveryVal)
                {
                    if (isBX == 2 || isBX == 0)
                    {
                        callbackTotalOut?.Invoke(deliveryVal);
                    }
                }
            }
            else
            {
                if (isBX == 2)
                {
                    callbackOut?.Invoke(deliveryVal);
                }
            }
        }

        #endregion

        #region Finish

        /// <summary>
        /// Cierra los puertos
        /// </summary>
        public void ClosePorts()
        {
            if (_serialPortBills.IsOpen)
            {
                _serialPortBills.Close();
            }

            if (_serialPortCoins.IsOpen)
            {
                _serialPortCoins.Close();
            }
        }

        #endregion
    }
}
