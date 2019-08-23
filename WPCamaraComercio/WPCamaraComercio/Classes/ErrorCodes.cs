using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio
{

    public class ErrorCodes
    {
        #region Coins and Bills Message
        /// <summary>
        /// No communication in bill acceptor and dispenser COM port
        /// </summary>
        public string ER_FATAL_APDP { get { return "ER:APDP:FATAL:"; } }
        /// <summary>
        /// No communication in coin acceptor and dispenser COM port
        /// </summary>
        public string ER_FATAL_CACD { get { return "ER:CACD:FATAL:"; } }
        public string ER_FATAL_PT { get { return "ER:PT:FATAL:"; } }

        public static string TIMEOUT_MESSAGE { get { return "Sin respuesta del {0}"; } }

        public static string BILL_ACCEPTOR_OK_MESSAGE { get { return "El billetero aceptador está conectado"; } }
        public static string BILL_DISPENSER_OK_MESSAGE { get { return "El billetero dispensador está conectado"; } }
        public static string COIN_ACCEPTOR_DISPENSER_OK_MESSAGE { get { return "Los monederos están conectados"; } }

        #endregion

        #region All Peripheral Messages
        public string ERROR_MESSAGE { get; set; }
        public string STATUS { get; set; }
        public string ERROR_CODE { get; set; }
        #endregion

        #region Barcode Reader
        public static string BARCODE_OK_MESSAGE { get { return "El lector está conectado"; } }
        public static string BARCODE_ERROR_MESSAGE { get { return "El lector tiene error"; } }
        public static string BARCODE_NOT_CONNECTED_MESSAGE { get { return "El lector no está conectado o no se encontró el puerto"; } }
        #endregion

        #region WebCam Messages 
        public static string WEBCAM_OK_MESSAGE { get { return "La cámara está conectada!"; } }
        public static string WEBCAM_ERROR_MESSAGE { get { return "La cámara no está conectada"; } }
        #endregion

        #region Printer Messages
        public static string PRINTER_OK_MESSAGE { get { return "La impresora está conectada"; } }


        public static string PRINTER_DISCONNECT_MESSAGE { get { return "ER:Printer:La impresora no está conectada o encendida, puerto: "; } }
        public static string PRINTER_LIBRARY_DOES_NOT_MATCH_MESSAGE { get { return "ER:Printer:La impresora y la biblioteca de llamadas no coinciden"; } }
        public static string PRINTER_HEADER_OPEN_MESSAGE { get { return "ER:Printer:Encabezado de impresión abierto"; } }
        public static string PRINTER_KHIFE_NO_RESET_MESSAGE { get { return "ER:Printer:Cuchillo de corte no RESET "; } }
        public static string PRINTER_OVERHEATING_MESSAGE { get { return "ER:Printer:Sobrecalentamiento de cabezal de impresión "; } }
        public static string PRINTER_BLACK_LABEL_MESSAGE { get { return "ER:Printer:Error de etiqueta negra"; } }
        public static string PRINTER_NO_PAPER_MESSAGE { get { return "ER:Printer:No hay papel en la impresora"; } }
        public static string PRINTER_LOW_PAPER_MESSAGE { get { return "ER:Printer:El papel se agotará"; } }
        #endregion
    }

}
