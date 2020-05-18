using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFCCMedellin.Classes.UseFull
{
   public class ReaderBarCode
    {
        public Action<DataDocument> callbackOut;//Calback para cuando sale cieerta cantidad del dinero

        public Action<string> callbackError;//Calback de error

        private SerialPort _serialBarCodeReader;

        public ReaderBarCode()
        {
            if (_serialBarCodeReader == null)
            {
                _serialBarCodeReader = new SerialPort();
            }
        }

        public void Start(string portReaderBarCode, int barcodeBaudRate)
        {
            try
            {
                if (_serialBarCodeReader != null)
                {
                    InitializePortBarcode(portReaderBarCode, barcodeBaudRate);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void InitializePortBarcode(string portName, int barcodeBaudRate)
        {
            try
            {
                if (!_serialBarCodeReader.IsOpen)
                {
                    _serialBarCodeReader.PortName = portName;
                    _serialBarCodeReader.BaudRate = barcodeBaudRate;
                    _serialBarCodeReader.Open();
                    _serialBarCodeReader.ReadTimeout = 200;
                    _serialBarCodeReader.DtrEnable = true;
                    _serialBarCodeReader.RtsEnable = true;
                    _serialBarCodeReader.DataReceived += new SerialDataReceivedEventHandler(_readerBarcode_DataReceived);
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message);
            }
        }

        private void _readerBarcode_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(1000);
                string response = _serialBarCodeReader.ReadExisting();
                if (!string.IsNullOrEmpty(response))
                {
                    ProcessResponseBarcode(response);
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }

        private void ProcessResponseBarcode(string response)
        {
            try
            {
                if (!string.IsNullOrEmpty(response))
                {
                    var dataReader = new DataDocument();

                    response = response.Remove(0, response.IndexOf("PubDSK_1") + 6);

                    string documentData = string.Empty;
                    string fullName = string.Empty;

                    if (response.IndexOf("0M") > 0)
                    {
                        dataReader.Gender = "Masculino";
                        dataReader.Date = response.Substring(response.IndexOf("0M") + 2, 8);
                        documentData = response.Substring(0, response.IndexOf("0M"));
                    }
                    else
                    {
                        documentData = response.Substring(0, response.IndexOf("0F"));
                        dataReader.Date = response.Substring(response.IndexOf("0F") + 2, 8);
                        dataReader.Gender = "Femenino";
                    }

                    foreach (var item in documentData.ToCharArray())
                    {
                        if (char.IsLetter(item))
                        {
                            fullName += item;
                        }
                        else if (char.IsWhiteSpace(item) || item.Equals('\0'))
                        {
                            fullName += " ";
                        }
                        else if (char.IsNumber(item))
                        {
                            dataReader.Document += item;
                        }
                    }
                    fullName = (fullName.TrimStart()).TrimEnd();

                    dataReader.Document = dataReader.Document.Substring(dataReader.Document.Length - 10, 10);

                    foreach (var item in fullName.Split(' '))
                    {
                        if (!string.IsNullOrEmpty(item) && item.Length > 1)
                        {
                            dataReader.FullName += string.Concat(item, " ");
                        }
                    }

                    dataReader.FirstName = dataReader.FullName.Split(' ')[2] ?? string.Empty;
                    dataReader.SecondName = dataReader.FullName.Split(' ')[3] ?? string.Empty;
                    dataReader.LastName = dataReader.FullName.Split(' ')[0] ?? string.Empty;
                    dataReader.SecondLastName = dataReader.FullName.Split(' ')[1] ?? string.Empty;

                    if (!string.IsNullOrEmpty(dataReader.Document) && !string.IsNullOrEmpty(dataReader.FullName))
                    {
                        callbackOut?.Invoke(dataReader);
                    }
                    else
                    {
                        callbackError?.Invoke("Datos de lectura imcompletos");
                    }
                }
                else
                {
                    callbackError?.Invoke("no se logro realizar la lectura");
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }
    }

    public class DataDocument
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string Date { get; set; }
        public string Gender { get; set; }
        public string Document { get; set; }

    }
}
