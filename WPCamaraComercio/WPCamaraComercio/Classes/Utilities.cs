using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPCamaraComercio.Models;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Views;
using WPCamaraComercio.WCFCamaraComercio;
using WPCamaraComercio.WCFPayPad;

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

        public static Resultado ConsultResult = new Resultado();

        public static string Enrollment { get; set; }

        public static string Tpcm { get; set; }

        public static RespuestaDetalle DetailResponse { get; set; }

        public static List<MerchantDetail> ListMerchantDetail = new List<MerchantDetail>();

        public static List<Certificado> ListCertificates = new List<Certificado>();

        public static decimal ValueToPay { get; set; }

        public static int IDTransactionDB { get; set; }

        public static PayerData PayerData { get; set; }

        public static string TOKEN { get; set; }

        public static int Session { get; set; }

        public static ControlPeripherals control = new ControlPeripherals();

        public static decimal EnterTotal;

        public static decimal DispenserVal { get; set; }

        #endregion

        #region GeneralEvents

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
                Debug.WriteLine(time.Seconds);
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
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                MainWindow main = new MainWindow();
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

        public void RestartApplication()
        {
            Process pc = new Process();
            Process pn = new Process();
            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = Path.Combine(Directory.GetCurrentDirectory(), "WPCamaraComercio.exe");
            pn.StartInfo = si;
            pn.Start();
            pc = Process.GetCurrentProcess();
            pc.Kill();
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
                    IDCorresponsal = Convert.ToInt32(GetConfiguration("8")),
                    Fecha = DateTime.Now
                };

                error.CreateLogsMethods(error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método encargado de actualizar la transacción a cualquiera de los estados posibles
        /// </summary>
        /// <param name="state">Id del estado al cual queremos actualizar la transacción</param>
        /// <returns></returns>
        public bool UpdateTransaction(CLSEstadoEstadoTransaction state)
        {
            try
            {
                bool response = false;

                switch (state)
                {
                    case CLSEstadoEstadoTransaction.Iniciada://Iniciada
                        response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Iniciada);
                        break;
                    case CLSEstadoEstadoTransaction.Aprobada://Aprobada
                        response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Aprobada);
                        break;
                    case CLSEstadoEstadoTransaction.Cancelada://Cancelada
                        response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                        break;
                    default://Cancelada
                        response = PayPadClient.ActualizarEstadoTransaccion(IDTransactionDB, WCFPayPad.CLSEstadoEstadoTransaction.Cancelada);
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
