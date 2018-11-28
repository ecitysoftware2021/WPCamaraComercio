using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Models;
//
namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmConsultar.xaml
    /// </summary>
    public partial class FrmConsultar : Window
    {
        #region "Referencias"
        Utilities utilities;
        Img img;
        #endregion

        #region "Constructor"
        public FrmConsultar()
        {
            InitializeComponent();
            utilities = new Utilities();
            img = new Img();
        }
        #endregion

        #region "Eventos"
        /// <summary>
        /// Evento que me reinicia el timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        /// <summary>
        /// Evento que me inicia el timer y asigna tag a las opciones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.Timer(tbTimer);
                img.Tag1 = "S";
                img.Tag2 = "N";
                DataContext = img;
            }
            catch (Exception ex)
            {
                utilities.saveLogError("Window_Loaded", "FrmSearch", ex.ToString());
            }
        }

        /// <summary>
        /// Evento que me cambian la opción elegida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgIdentificacion_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            string tag = img.Tag1;

            if (tag.Equals("S"))
            {
                img.Tag1 = "N";
                imgIdentificacion.Source = Utilities.SetButtonImage("Others", "circulo", "png");
                img.Tag2 = "S";
                imgNombre.Source = Utilities.SetButtonImage("Others", "ok", "png");
            }
            else
            {
                img.Tag1 = "S";
                imgIdentificacion.Source = Utilities.SetButtonImage("Others", "ok", "png");
                img.Tag2 = "N";
                imgNombre.Source = Utilities.SetButtonImage("Others", "circulo", "png");
            }
        }

        /// <summary>
        /// Evento que me cambian la opción elegida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgNombre_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            string tag = img.Tag2;

            if (tag.Equals("S"))
            {
                img.Tag2 = "N";
                imgNombre.Source = Utilities.SetButtonImage("Others", "circulo", "png");
                img.Tag1 = "S";
                imgIdentificacion.Source = Utilities.SetButtonImage("Others", "ok", "png");
            }
            else
            {
                img.Tag2 = "S";
                imgNombre.Source = Utilities.SetButtonImage("Others", "ok", "png");
                img.Tag1 = "N";
                imgIdentificacion.Source = Utilities.SetButtonImage("Others", "circulo", "png");
            }
        }
        #endregion

        #region "HeadersButtons"
        /// <summary>
        /// Botón que me redirecciona a la ventana anterior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    FrmMenu menu = new FrmMenu();
                    menu.Show();
                    this.Close();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnBack_PreviewStylusDown", "FrmSearch", ex.ToString());
            }
        }

        /// <summary>
        /// Botón que me redirecciona a la ventana inicial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExit_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Utilities.GoToInicial();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnExit_PreviewStylusDown", "FrmSearch", ex.ToString());
            }
        }
        #endregion

        #region "ButtonConsult"
        private void BtnConsultar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtNitCedula.Text))
                {
                    FrmModal modal = new FrmModal("Debe de ingresar el NIT ó Cédula");
                    modal.ShowDialog();
                }
                else
                {
                    load_gif.Visibility = Visibility.Visible;


                }
            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnConsultar_PreviewMouseDown", "FrmSearch", ex.ToString());
            }
        }
        #endregion
    }
}
