using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.WCFPayPad;

namespace WPCamaraComercio.Classes.Smart
{
    public class SmartCoins
    {
        private decimal payValue;

        private decimal intoValue;

        private SerialPort _serialPortCoins;

        private static int HUNDRED = 100;

        public Action<decimal> callbackTotal;

        public Action<decimal> callbackValue;

        public Action<string> callbackError;

        private int statePay = 0;

        private static int READTIMEOUT = 500;

        private static int WHRITETIMEOUT = 3000;

        private static string PORTCOINS;

        private string basseAddress;

        private string COMAND_ON_COINS = "1001";

        private string COMAND_OFF_COINS = "1111";

        private int type = 1;

        private decimal paymentValue = 0;

        private int stateDispenser = 0;

        private decimal returnValue;

        List<Log> log = new List<Log>();

        ServicePayPadClient WCFPayPadWS = new ServicePayPadClient();

        int idCorrespo = int.Parse(ConfigurationManager.AppSettings["IDCorresponsal"]);

        public SmartCoins()
        {
            _serialPortCoins = new SerialPort();

            PORTCOINS = Utilities.GetConfiguration("PortMonedero");

            Init();
        }

        public void Start()
        {
            try
            {
                this.type = 1;
                SendMessage(COMAND_ON_COINS);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Stop()
        {
            if (_serialPortCoins.IsOpen)
            {
                SendMessage(COMAND_OFF_COINS);
                _serialPortCoins.Close();
            }
            statePay = 2;
        }

        private void Init()
        {
            InitAcceptanceCoins();
        }

        private void InitAcceptanceCoins()
        {
            try
            {
                if (!_serialPortCoins.IsOpen)
                {
                    _serialPortCoins.PortName = PORTCOINS;                                                                                  // Set the read/write timeouts
                    _serialPortCoins.ReadTimeout = READTIMEOUT;
                    _serialPortCoins.WriteTimeout = WHRITETIMEOUT;
                    _serialPortCoins.Open();
                }
                _serialPortCoins.DataReceived += new SerialDataReceivedEventHandler(_serialPortCoins_DataReceived);
            }
            catch (Exception ex)
            {
                // objUtil.saveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message);
            }
        }

        public bool validateAcceptance()
        {
            return true;
        }

        private void SendMessage(string message)
        {
            try
            {
                _serialPortCoins.Write(message);
                //_serialPortM.DiscardOutBuffer();
            }
            catch (Exception ex)
            {
                // objUtil.saveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message);
            }
        }

        private void _serialPortCoins_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                ProccesResponceCoins(_serialPortCoins.ReadLine());
            }
            catch (Exception ex)
            {
                ProccesResponceCoins(_serialPortCoins.ReadExisting());
            }
        }

        public void ReturnCoins(decimal valueInCoins)
        {
            try
            {
                this.type = 2;
                var mount = valueInCoins / HUNDRED;
                SendMessage(mount.ToString());
            }
            catch (Exception ex)
            {
                // objUtil.saveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message);
            }
        }

        private void Finish()
        {
            try
            {
                if (type == 1)
                {
                    statePay = 2;
                }
                else
                {
                    stateDispenser = 2;
                }
                Stop();
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }

        private void ProccesResponceCoins(string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    if (this.type == 1)
                    {
                        var value = Convert.ToDecimal(message);
                        if (value > 0)
                        {
                            this.intoValue = this.intoValue + value;
                            callbackValue?.Invoke(value);
                        }
                    }
                    else
                    {
                        decimal totalDispenser = 0;
                        if (message.ToUpper().Contains("OFF"))
                        {
                            string[] data = message.Replace("\r", "").Split(';');
                            int control = 0;
                            foreach (var item in data)
                            {
                                if (control > 0)
                                {
                                    string denomination = item.Split(':')[0];
                                    int cuantity = int.Parse(item.Split(':')[1]);
                                    int idDenomination = Utilities.GetDescriptionEnum(denomination);
                                    totalDispenser = totalDispenser + (Convert.ToDecimal(denomination) * cuantity);

                                    log.Add(new Log
                                    {
                                        Fecha = DateTime.Now,
                                        IDTrsansaccion = Utilities.IDTransactionDB,
                                        Operacion = "Devolución más respuesta monedero " + message,
                                        ValorDevolver = totalDispenser,
                                        ValorDevuelto = denomination,
                                        CantidadDevolucion = cuantity,
                                        ValorPago = Utilities.ValueToPay,
                                        ValorIngresado = Utilities.ValueEnter,
                                        EstadoTransaccion = "Aprobada"
                                    });
                                    Task.Run(() => 
                                    {
                                        WCFPayPadWS.InsertarControlMonedas(idDenomination, idCorrespo, 0, cuantity);
                                    });
                                }
                                control++;
                            }
                        }
                        callbackTotal?.Invoke(totalDispenser);
                    }
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }
    }
}
