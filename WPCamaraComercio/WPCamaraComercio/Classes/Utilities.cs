using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
//using WPCamaraComercio.WCFPayPad;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.Classes
{
    public class Utilities
    {
        #region References

        //ServicePayPadClient PayPadClient = new ServicePayPadClient();

        public static string Duration = GetConfiguration("Duration");

        public static TimeSpan time;

        public static DispatcherTimer timer;

        public static int CorrespondentId = int.Parse(GetConfiguration("IDCorresponsal"));

        public static int CorrespondentId2 = int.Parse(GetConfiguration("IDCorresponsal"));

        public static ResponseConsultGeneral RespuestaConsulta = new ResponseConsultGeneral();
        

        public static string search { get; set; }

        public static string BuyID { get; set; }

        public static ResultadoGeneral ConsultResult = new ResultadoGeneral();

        public static string Enrollment { get; set; }

        public static string Tpcm { get; set; }

        public static ResponseDetalleComerciante DetailResponse { get; set; }

        public static List<MerchantDetail> ListMerchantDetail = new List<MerchantDetail>();

        public static List<Certificado> ListCertificates = new List<Certificado>();

        public static decimal ValueToPay { get; set; }

        public static int IDTransactionDB { get; set; }

        public static PayerData PayerData { get; set; }

        public static string TOKEN { get; set; }

        public static int Session { get; set; }

        public static ControlPeripherals control;

        public static decimal EnterTotal;

        public static decimal DispenserVal { get; set; }

        public static DataPayPad dataPaypad = new DataPayPad();

        public static Api api;

        public static decimal ValueReturned { get; set; }

        public static long ValueDelivery { get; set; }// valor entregado en la pantalla de retiro

        public static List<LogTransactional> log = new List<LogTransactional>();
        #endregion

        #region GeneralEvents

        public Utilities()
        {

        }

        public Utilities(int i)
        {
            try
            {
                api = new Api();
                control = new ControlPeripherals();
                control.StopAceptance();
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
                //var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                //FrmInitial main = new FrmInitial();
                FrmTransactionsList main = new FrmTransactionsList();
                main.Show();
                //window.Close();
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
        /// Método que me guarda un error ocurrido en la aplicación
        /// </summary>
        /// <param name="Metodo"></param>
        /// <param name="Clase"></param>
        /// <param name="Mensaje"></param>
        /// </summary>
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

        public static void CrearLogTransactional(List<LogTransactional> data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var pathFile = "C:\\LogTransactionalCCM";
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

        

        public enum EDenominacion
        {
            [Description("100M")]
            Cien = 1,
            [Description("200")]
            Doscientos = 2,
            [Description("500")]
            Quinientos = 3,
            [Description("1")]
            Mil = 4,
            [Description("2")]
            BDosMil = 5,
            [Description("5")]
            BCincoMil = 6,
            [Description("10")]
            BDiezMil = 7,
            [Description("20")]
            BVeinteMil = 8,
            [Description("50")]
            BCincuentaMil = 9,
            [Description("100")]
            BMil = 11
        }

        public static int getDescriptionEnum(string value)
        {
            int idTypeEnum = 0;
            Type enumType = typeof(EDenominacion);
            var values = enumType.GetEnumValues();
            foreach (EDenominacion item in values)
            {
                MemberInfo info = enumType.GetMember(item.ToString()).First();
                var description = info.GetCustomAttribute<DescriptionAttribute>();
                if (value == description.Description)
                {
                    EDenominacion enumm = (EDenominacion)Enum.Parse(typeof(EDenominacion), item.ToString());
                    idTypeEnum = (int)enumm;
                    break;
                }
            }
            return idTypeEnum;
        }
    }
    #endregion
}

