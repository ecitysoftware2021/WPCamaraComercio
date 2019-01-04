using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WPCamaraComercio.Views;
using WPCamaraComercio.WCFCamaraComercio;

namespace WPCamaraComercio.Classes
{
    public class Utilities
    {
        #region References
        public static string Duration = GetConfiguration("Duration");
        
        public static TimeSpan time;

        public static DispatcherTimer timer;
        
        public static int CorrespondentId = int.Parse(GetConfiguration("IDCorresponsal"));

        public static RespuestaConsulta RespuestaConsulta { get; set; }

        public static Resultado[] Result { get; set; }

        public static string search { get; set; }

        public static IEnumerable<Resultado> ConsultResult { get; set; }

        static string _Matricula;

        public string Matricula
        {
            get
            {
                return _Matricula;
            }
            set
            {
                _Matricula = value;
            }
        }

        static string _Tpcm;

        public string Tpcm
        {
            get
            {
                return _Tpcm;
            }
            set
            {
                _Tpcm = value;
            }
        }

        public static RespuestaDetalle DetailResponse { get; set; }

        public static List<MerchantDetail> ListMerchantDetail = new List<MerchantDetail>();

        //public static Dictionary<int, int> CertificateQuantity = new Dictionary<int, int>()
        //{
        //    {1,1},
        //    {2,2},
        //    {3,3},
        //    {4,4},
        //    {5,5}
        //};
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
                CLSPrint objPrint = new CLSPrint();

                //dataPrinter.Tipo = Utilities.Tipo;
                //dataPrinter.FechaPago = DateTime.Now;
                //dataPrinter.Usuario = receipt.PROPIETARIO;
                //dataPrinter.Identificacion = receipt.DOCUMENTO;
                //dataPrinter.Valor = String.Format("{0:C0}", transaction.ValueTotal);
                //dataPrinter.ValorIngresado = String.Format("{0:C2}", payModel.ValorIngresado);
                //dataPrinter.ValorDevuelto = String.Format("{0:C2}", payModel.ValorRestante);

                objPrint.ImprimirComprobante();
            }
            catch (Exception ex)
            {
                saveLogError("btnConsultar_StylusDown", "FrmSearch", ex.ToString());
            }
        }

        /// <summary>
        /// Método que me guarda un error ocurrido en la aplicación
        /// </summary>
        /// <param name="Metodo"></param>
        /// <param name="Clase"></param>
        /// <param name="Mensaje"></param>
        public void saveLogError(string Metodo, string Clase, string Mensaje)
        {
            //try
            //{
                //using (BDCamaraComercioEntities conexion = new BDCamaraComercioEntities())
                //{
                //    Tbl_LogError error = new Tbl_LogError();
                //    error.Message = Mensaje;
                //    error.NameClass = Clase;
                //    error.NameMethod = Metodo;
                //    error.IDCorresponsal = Convert.ToInt32(GetConfiguration("IDCorresponsal"));
                //    error.Fecha = DateTime.Now;
                //    error.State = false;

                //    conexion.Tbl_LogError.Add(error);
                //    conexion.SaveChanges();
                //}

            //    FrmModal fail = new FrmModal(Mensaje);
            //    fail.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    FrmModal fail = new FrmModal(Mensaje);
            //    fail.ShowDialog();
            //}
        }
        #endregion
    }
}
