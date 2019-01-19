using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPCamaraComercio.Models;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
using WPCamaraComercio.Views;
using WPCamaraComercio.WCFCamaraComercio;
using WPCamaraComercio.WCFPayPad;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.Classes
{
    public class Utilities
    {
        #region References

        ServicePayPadClient PayPadClient = new ServicePayPadClient();

        public static string Duration = GetConfiguration("Duration");

        public static TimeSpan time;

        public static DispatcherTimer timer;

        public static int CorrespondentId = int.Parse(GetConfiguration("IDCorresponsal"));

        public static RespuestaConsulta RespuestaConsulta = new RespuestaConsulta();

        public static Resultado[] Result { get; set; }

        public static string search { get; set; }

        public static string BuyID { get; set; }

        public static Resultado ConsultResult = new Resultado();

        public static string Enrollment { get; set; }

        public static string Tpcm { get; set; }

        public static RespuestaDetalle DetailResponse { get; set; }

        public static List<MerchantDetail> ListMerchantDetail = new List<MerchantDetail>();

        public static List<Certificado> ListCertificates = new List<Certificado>();

        //internal void UpdateTransaction(object _enterValue, int v, string buyID)
        //{
        //    throw new NotImplementedException();
        //}

        public static decimal ValueToPay { get; set; }

        public static int IDTransactionDB { get; set; }

        public static PayerData PayerData { get; set; }

        public static string TOKEN { get; set; }

        public static int Session { get; set; }

        public static ControlPeripherals control;

        public static decimal EnterTotal;

        public static decimal DispenserVal { get; set; }

        public static DataPayPad dataPaypad = new DataPayPad();
        Api api = new Api();

        public static decimal ValueReturned { get; set; }
        

        #endregion

        #region GeneralEvents

        public Utilities()
        {

        }

        public Utilities(int i)
        {
            try
            {
                //Duration = GetConfiguration("Duration");
                //CountSlider = 0;
                //flagSlider = false;
                //dataPaypad = new DataPaypad();
                control = new ControlPeripherals();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Se usa para abrir la modal de información/error
        /// </summary>
        /// <param name="Message">mensaje para ser mostrado</param>
        public static void OpenModal(string Message, Window w, bool state = false)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                FrmModal modal = new FrmModal(Message, w, state);
                modal.ShowDialog();
            }));
            return;
        }

        /// <summary>
        /// Se usa para abrir la modal de información/error
        /// </summary>
        /// <param name="Message">mensaje para ser mostrado</param>
        public static void ModalError(string Message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                FrmModalErrors modal = new FrmModalErrors(Message);
                modal.ShowDialog();
            }));
            return;
        }

        /// <summary>
        /// Se usa para ocultar o mostrar la modal de carga
        /// </summary>
        /// <param name="window">objeto de la clase FrmLoading  </param>
        /// <param name="state">para saber si se oculta o se muestra true:muestra, false: oculta</param>
        public static void Loading(Window window, bool state, Window w)
        {
            if (state)
            {
                window.Show();
                w.IsEnabled = false;
            }
            else
            {
                window.Hide();
                w.IsEnabled = true;
            }
        }

        /// <summary>
        /// Método que me busca en el appConfing 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiguration(string key)
        {
            AppSettingsReader reader = new AppSettingsReader();
            return reader.GetValue(key, typeof(String)).ToString();
        }

        /// <summary>
        /// Método que me inicia el contador del tiempo
        /// </summary>
        /// <param name="textblock"></param>
        public static void Timer(TextBlock textblock)
        {
            time = TimeSpan.Parse(Duration);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (a, b) =>
            {
                time = time.Subtract(new TimeSpan(0, 0, 1));
                textblock.Text = (time.Seconds < 10) ? string.Format("0{0}:0{1}", time.Minutes, time.Seconds) : string.Format("0{0}:{1}", time.Minutes, time.Seconds);

                if (time.Minutes == 0 && time.Seconds == 0)
                {
                    timer.Stop();
                    GoToInicial();
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Método que me recetea el tiempo
        /// </summary>
        public static void ResetTimer()
        {
            time = TimeSpan.Parse(Duration);
            timer.Stop();
        }

        /// <summary>
        /// Método que me redirecciona a la ventana de inicio
        /// </summary>
        public static void GoToInicial()
        {

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                FrmInitial main = new FrmInitial();
                main.Show();
                window.Close();
                CloseWindows(main.Title);
            }));
            GC.Collect();

        }

        public static void CloseWindows(string Title)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.IsLoaded && w.Title != Title)
                {
                    w.Close();
                }
            }
        }

        /// <summary>
        /// Se usa para cambiar la imagen de un botón
        /// </summary>
        /// <param name="folder">nombre de la carpeta donde está la imagen ex: Buttons</param>
        /// <param name="path">nombre de la imagen ex: b-consultar</param>
        /// <param name="ext">tipo de extensión de la imagen ex: png</param>
        /// <returns></returns>
        public static BitmapImage SetButtonImage(string folder, string path, string ext)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(string.Concat("/WPCamaraComercio;component/Images/", folder, "/", path, ".", ext), UriKind.Relative);
            logo.EndInit();

            return logo;
        }

        /// <summary>
        /// Método que me llama a la clase CLSPrint para imprimir la factura
        /// </summary>
        public void ImprimirComprobante()
        {
            try
            {
                Print print = new Print();

                //dataPrinter.Tipo = Utilities.Tipo;
                //dataPrinter.FechaPago = DateTime.Now;
                //dataPrinter.Usuario = receipt.PROPIETARIO;
                //dataPrinter.Identificacion = receipt.DOCUMENTO;
                //dataPrinter.Valor = String.Format("{0:C0}", transaction.ValueTotal);
                //dataPrinter.ValorIngresado = String.Format("{0:C2}", payModel.ValorIngresado);
                //dataPrinter.ValorDevuelto = String.Format("{0:C2}", payModel.ValorRestante);

                print.ImprimirComprobante();
            }
            catch (Exception ex)
            {
                //saveLogError("btnConsultar_StylusDown", "FrmSearch", ex.ToString());
            }
        }

        /// <summary>
        /// Método que me guarda un error ocurrido en la aplicación
        /// </summary>
        /// <param name="Metodo"></param>
        /// <param name="Clase"></param>
        /// <param name="Mensaje"></param>
        /// 
        public void FillLogError(string error, string operacion)
        {
            List<LogError> logError = new List<LogError>();
            logError.Add(new LogError
            {
                Fecha = DateTime.Now,
                IDTrsansaccion = IDTransactionDB,
                Operacion = operacion,
                Error = error
            });

            CreateLogError(logError);
            logError.Clear();
        }

        public void CreateLogError(List<LogError> dato)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dato);
                var pathFile = "C:\\LogErrores";
                if (!Directory.Exists(pathFile))
                {
                    Directory.CreateDirectory(pathFile);
                }
                var file = "Log" + DateTime.Now.ToString("yyyyMMdd") + ".json";
                var nameFile = Path.Combine(pathFile, file);
                if (!File.Exists(nameFile))
                {
                    var archivo = File.CreateText(nameFile);
                    archivo.Close();
                }
                using (StreamWriter sw = File.AppendText(nameFile))
                {
                    sw.WriteLine(json);
                }
            }
            catch { }
        }

        public static void SaveLogDispenser(LogDispenser log)
        {
            LogService logService = new LogService
            {
                NamePath = "C:\\LogDispenser",
                FileName = string.Concat("Log", DateTime.Now.ToString("yyyyMMdd"), ".json")
            };

            logService.CreateLogs(log);
        }

        public static void SaveLogTransactions(LogErrorGeneral log, string path)
        {
            LogService logService = new LogService
            {
                NamePath = path,
                FileName = string.Concat("Log", DateTime.Now.ToString("yyyyMMdd"), ".json")
            };

            logService.CreateLogsTransactions(log);
        }

        /// <summary>
        /// Encargado de serealizar el objeto en JSON para el log de transacciones
        /// </summary>
        /// <param name="identification">identificaion de la persona que realiza el trámite</param>
        public static string CreateJSON()
        {
            string json = string.Empty;
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new StaticPropertyContractResolver()
            };

            json = JsonConvert.SerializeObject(new InfoUser(), serializerSettings);

            return json;
        }


        /// <summary>
        /// Clase encargada de serializar un objeto estático(clase con atributos estáticos)
        /// en un JSON
        /// </summary>
        public class StaticPropertyContractResolver : DefaultContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var baseMembers = base.GetSerializableMembers(objectType);
                PropertyInfo[] staticMembers = objectType.GetProperties(BindingFlags.Static | BindingFlags.Public);
                baseMembers.AddRange(staticMembers);

                return baseMembers;
            }
        }

        /// <summary>
        /// Método usado para regresar a la pantalla principal
        /// </summary>
        public static void RestartApp()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Process pc = new Process();
                Process pn = new Process();
                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = Path.Combine(Directory.GetCurrentDirectory(), "WPCamaraComercio.exe");
                pn.StartInfo = si;
                pn.Start();
                pc = Process.GetCurrentProcess();
                pc.Kill();
            }));
            GC.Collect();
        }

        public void SaveLogErrorMethods(string Method, string Class, string Message)
        {
            try
            {
                LogErrorMethods error = new LogErrorMethods
                {
                    Message = Message,
                    NameClass = Class,
                    NameMethod = Method,
                    IDCorresponsal = Convert.ToInt32(GetConfiguration("IDCorresponsal")),
                    Fecha = DateTime.Now.ToString("yyyy’-‘MM’-‘dd’")
                };

                error.CreateLogsMethods(error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///// <summary>
        ///// Método encargado de actualizar la transacción a cualquiera de los estados posibles
        ///// </summary>
        ///// <param name="state">Id del estado al cual queremos actualizar la transacción</param>
        ///// <returns></returns>
        //public bool UpdateTransaction(CLSEstadoEstadoTransaction state)
        //{
        //    try
        //    {
        //        bool response = false;

        //        switch (state)
        //        {
        //            case CLSEstadoEstadoTransaction.Iniciada://Iniciada
        //                response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Iniciada);
        //                break;
        //            case CLSEstadoEstadoTransaction.Aprobada://Aprobada
        //                response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Aprobada);
        //                break;
        //            case CLSEstadoEstadoTransaction.Cancelada://Cancelada
        //                response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
        //                break;
        //            default://Cancelada
        //                response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
        //                break;
        //        }
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        /// <summary>
        /// Método encargado de crear la transacciòn en bd y retornar el id de esta misma   
        /// </summary>
        /// TODO:CAMBIAR COMENTARIO
        /// <param name="payFee">Objeto que contiene la info de la transacción</param>
        public async void CreateTransaction()
        {
            try
            {
                Transaction Transaction = new Transaction
                {
                    TOTAL_AMOUNT = ValueToPay,
                    DATE_BEGIN = DateTime.Now,
                    DESCRIPTION = string.Empty,
                    TYPE_TRANSACTION_ID = 3,
                    STATE_TRANSACTION_ID = 1,
                };

                var response = await api.GetResponse(new RequestApi
                {
                    Data = Transaction
                }, "SaveTransaction");

                if (response != null)
                {
                    if (response.CodeError == 200)
                    {
                        IDTransactionDB = Convert.ToInt32(response.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLogErrorMethods("AssingProperties", "FrmPaymentData", ex.ToString());
                //navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        public async Task<bool> UpdateTransaction(decimal Enter, int state, string BuyID, decimal Return = 0)
        {
            try
            {

                Transaction Transaction = new Transaction
                {
                    STATE_TRANSACTION_ID = state,
                    DATE_END = DateTime.Now,
                    DESCRIPTION = BuyID,
                    INCOME_AMOUNT = Enter,
                    RETURN_AMOUNT = Return,
                    TRANSACTION_ID = IDTransactionDB
                };

                var response = await api.GetResponse(new RequestApi
                {
                    Data = Transaction
                }, "UpdateTransaction");

                if (response != null)
                {
                    if (response.CodeError == 200)
                    {
                        IDTransactionDB = Convert.ToInt32(response.Data);
                        return true;
                    }

                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void InsertPayerData()
        {
            PayerDataDB payer = new PayerDataDB
            {
                ADDRESS = PayerData.BuyerAddress,
                EMAIL = PayerData.Email,
                IDENTIFICATION = PayerData.BuyerIdentification,
                LAST_NAME = PayerData.LastNameBuyer,
                NAME = PayerData.FirstNameBuyer,
                PHONE = PayerData.Phone,
                STATE = true
            };

            var response = await api.GetResponse(new RequestApi
            {
                Data = payer
            }, "SavePayer");

            if (response != null)
            {
                if (response.CodeError == 200)
                {
                    //IDTransactionDB = Convert.ToInt32(response.Data);
                }
            }
        }
        #endregion
    }
}
