using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using WP.Impresora;
using WPCamaraComercio.Classes;

namespace WPCamaraComercio
{
    public class ControlPeripheralsNotArduino
    {
        #region Callbacks
        public static Action<ErrorCodes> callbackStatusPrinter;//Calback printer status
        public static Action<ErrorCodes> callbackStatusWebCam;//Calback webcam status
        public static Action<string> callbackStatusClientWebCam;//Calback webcam status
        public static Action<ErrorCodes> callbackStatusBarcode;//Calback barcode status

        #endregion

        #region Objects

        public static PrintProperties _PrintProperties;
        #endregion

        #region Timeouts

        System.Timers.Timer TimerStatusPeripheral; //Timer para checar el estado de lo perifericos
        #endregion


        public ControlPeripheralsNotArduino()
        {
            InitObjects();
        }

        private void InitObjects()
        {
            if (_PrintProperties == null)
            {
                _PrintProperties = new PrintProperties();
            }
        }

        #region Print Message
        public void InitPortPrinter()
        {
            try
            {
                int statusCode = 0;
                statusCode = GetPrinterCode();
                ProcessResponsePrinter(statusCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int GetPrinterCode()
        {
            try
            {
                _PrintProperties.ConfigurationPrinter(Utilities.GetConfiguration("PortPrinter"), Utilities.GetConfiguration("PrintBandrate"));
                return StatusPrint();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int StatusPrint()
        {
            int status = 1;
            try
            {
                if (_PrintProperties != null)
                {
                    status = _PrintProperties.GetPrintStatus();
                }
            }
            catch (Exception EX)
            {
                throw EX;
            }
            return status;
        }

        #endregion



        #region ProccessResponse
        /// <summary>
        /// Método que procesa la respuesta de la impresora
        /// </summary>
        /// <param name="message">respuesta del puerto de los monederos</param>
        private static void ProcessResponsePrinter(int code)
        {

            string message = string.Empty;
            ErrorCodes errorCodes = new ErrorCodes();
            switch (code)
            {
                case 0:
                    errorCodes.STATUS = "OK";
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_OK_MESSAGE;
                    break;
                case 1:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = string.Concat(ErrorCodes.PRINTER_DISCONNECT_MESSAGE, Utilities.GetConfiguration("PortPrinter"));
                    break;
                case 2:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_LIBRARY_DOES_NOT_MATCH_MESSAGE;
                    break;
                case 3:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_HEADER_OPEN_MESSAGE;
                    break;
                case 4:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_KHIFE_NO_RESET_MESSAGE;
                    break;
                case 5:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_OVERHEATING_MESSAGE;
                    break;
                case 6:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_BLACK_LABEL_MESSAGE;
                    break;
                case 7:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_NO_PAPER_MESSAGE;
                    break;
                case 8:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_LOW_PAPER_MESSAGE;
                    break;
                default:
                    errorCodes.STATUS = "OK";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = string.Empty;
                    break;
            }
            callbackStatusPrinter?.Invoke(errorCodes);
        }
        #endregion


    }
}
