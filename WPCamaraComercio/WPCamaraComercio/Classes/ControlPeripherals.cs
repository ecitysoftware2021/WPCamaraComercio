using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private string _DispenserBillOFF = "OR:OFF:DP";//Cerrar billetero Dispenser

        private string _AceptanceCoinOn = "OR:ON:MA";//Operar Monedero Aceptance

        private string _DispenserCoinOn = "OR:ON:MD:";//Operar Monedero Dispenser

        private string _AceptanceCoinOff = "OR:OFF:MA";//Cerrar Monedero Aceptance

        #endregion

        #region Callbacks

        public Action<decimal> callbackValueIn;//Calback para cuando ingresan un billete

        public Action<decimal> callbackValueOut;//Calback para cuando sale un billete

        public Action<decimal> callbackTotalIn;//Calback para cuando se ingresa la totalidad del dinero

        public Action<decimal> callbackTotalOut;//Calback para cuando sale la totalidad del dinero

        public Action<string> callbackError;//Calback de error

        public Action<string> callbackMessage;//Calback de mensaje

        #endregion

        #region EvaluationValues

        private static int _mil = 1000;
        private static int _hundred = 100;
        private static int _tens = 10;

        #endregion

        #region Variables

        private decimal payValue;//Valor a pagar

        private decimal enterValue;//Valor ingresado

        private decimal deliveryValue;//Valor entregado

        private decimal dispenserValue;//Valor a dispensar

        private static string TOKEN;//Llabe que retorna el dispenser

        public static string LogMessage;//Mensaje para el log

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
                log = new LogDispenser
                {
                    DateDispenser = DateTime.Now,
                    TransactionId = Utilities.IDTransactionDB.ToString(),
                };
                InitPortBills();
                InitPortPurses();
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
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills()
        {
            try
            {
                if (!_serialPortBills.IsOpen)
                {
                    _serialPortBills.PortName = Utilities.GetConfiguration("PortBills");
                    _serialPortBills.ReadTimeout = 500;
                    _serialPortBills.WriteTimeout = 500;
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
                if (!_serialPortBills.IsOpen)
                {
                    _serialPortCoins.PortName = Utilities.GetConfiguration("PortCoins");
                    _serialPortCoins.ReadTimeout = 500;
                    _serialPortCoins.WriteTimeout = 500;
                    _serialPortCoins.Open();
                }

                _serialPortBills.DataReceived += new SerialDataReceivedEventHandler(_serialPortCoinsDataReceived);
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
                    log.ResponseMessage += string.Format("Respuesta Billetero: {0}\n", response);
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
                    log.ResponseMessage += string.Format("Respuesta Billetero: {0}\n", response);
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
        private static void ProcessRC(string[] response)
        {
            if (response[1] == "OK")
            {
                switch (response[2])
                {
                    case "AP":

                        break;
                    case "DP":
                        if (response[3] == "HD" && string.IsNullOrEmpty(response[4]))
                        {
                            TOKEN = response[4];
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
            if (response[1] == "DP")
            {
                LogMessage += string.Concat("Error: ", response[2], Environment.NewLine, "Valor entregado: ", deliveryValue, Environment.NewLine);
                callbackError?.Invoke(string.Concat("Error, se alcanzó a entregar:", deliveryValue));
            }
        }

        /// <summary>
        /// Respuesta para el caso de ingreso o salida de un billete/moneda
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessUN(string[] response)
        {
            if (response[1] == "DP" || response[1] == "MD")
            {
                deliveryValue += decimal.Parse(response[2]);
                callbackValueOut?.Invoke(Convert.ToDecimal(response[2]));
            }
            else
            {
                enterValue += decimal.Parse(response[2]);
                callbackValueIn?.Invoke(Convert.ToDecimal(response[2]));
                ValidateEnterValue();
            }
        }

        /// <summary>
        /// Respuesta para el caso de total cuando responde el billetero/monedero dispenser
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessTO(string[] response)
        {
            if (response[1] == "OK")
            {
                if (response[2] == "DP" || response[1] == "MD")
                {
                    ConfigDataDispenser(response[3]);
                }
            }
            else
            {
                if (response[2] == "DP")
                {
                    ConfigDataDispenser(response[3], true);
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
                SendMessageCoins(_DispenserCoinOn);
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
                            SendMessageCoins((amountCoins / _hundred).ToString());
                        }
                    }
                    else
                    {
                        decimal valuePayCoin = dispenserValue / _hundred;
                        SendMessageCoins(valuePayCoin.ToString());
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
                    SendMessageBills(string.Format("{0}:{0}:{1}", _DispenserBillOn, TOKEN, valuePay));
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
            if (enterValue >= payValue)
            {
                StopAceptance();
                callbackTotalIn?.Invoke(enterValue);
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

        /// <summary>
        /// Procesa la respuesta del billeteri dispenser
        /// </summary>
        /// <param name="data">respuesta</param>
        /// <param name="isRj">si se fue o no al reject</param>
        private void ConfigDataDispenser(string data, bool isRj = false)
        {
            string[] values = data.Split(';');
            decimal deliveryVal = 0;
            foreach (var value in values)
            {
                int denominacion = int.Parse(value.Split('-')[0]);
                int cantidad = int.Parse(value.Split('-')[1]);
                deliveryVal += denominacion * cantidad;
                LogMessage += string.Concat("Respuesta billetero: ", values, Environment.NewLine);
                if (isRj)
                {
                    LogMessage += string.Concat("Cantidad en el reject: ", deliveryValue, Environment.NewLine);
                }
                else
                {
                    LogMessage += string.Concat("Cantidad dispensada: ", deliveryValue, Environment.NewLine);
                }
            }
            if (dispenserValue == deliveryVal)
            {
                callbackTotalOut?.Invoke(deliveryVal);
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
