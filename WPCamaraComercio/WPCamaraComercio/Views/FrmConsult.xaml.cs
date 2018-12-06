using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para FrmConsult.xaml
    /// </summary>
    public partial class FrmConsult : Window
    {
        #region References

        Utilities utilities;

        #endregion

        #region "Constructor"
        public FrmConsult()
        {
            InitializeComponent();
            utilities = new Utilities();
        }
        #endregion

        #region Timer

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.Timer(tbTimer);
                img = new Img
                {
                    Tag1 = "S",
                    Tag2 = "N"
                };
                DataContext = img;
            }
            catch (Exception ex)
            {
                utilities.saveLogError("Window_Loaded", "FrmSearch", ex.ToString());
            }
        }

        #endregion

        #region Events

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
                //  TxtNombre.Visibility = Visibility.Visible;
                TxtIdentificacion.Visibility = Visibility.Hidden;
                TxtIdentificacion.Text = string.Empty;
            }
            else
            {
                img.Tag1 = "S";
                imgIdentificacion.Source = Utilities.SetButtonImage("Others", "ok", "png");
                img.Tag2 = "N";
                imgNombre.Source = Utilities.SetButtonImage("Others", "circulo", "png");
                // TxtNombre.Visibility = Visibility.Hidden;
                TxtIdentificacion.Visibility = Visibility.Visible;
                // TxtNombre.Text = string.Empty;
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
                //TxtNombre.Visibility = Visibility.Hidden;
                TxtIdentificacion.Visibility = Visibility.Visible;

            }
            else
            {
                img.Tag2 = "S";
                imgNombre.Source = Utilities.SetButtonImage("Others", "ok", "png");
                img.Tag1 = "N";
                imgIdentificacion.Source = Utilities.SetButtonImage("Others", "circulo", "png");
                // TxtNombre.Visibility = Visibility.Visible;
                TxtIdentificacion.Visibility = Visibility.Hidden;
                TxtIdentificacion.Text = string.Empty;
            }
        }

        private void BtnConsultar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtIdentificacion.Text))
                {
                    FrmModal modal = new FrmModal("Debe de ingresar el NIT ó Cédula");
                    modal.ShowDialog();
                }
                //else
                //if (string.IsNullOrEmpty(TxtNombre.Text))
                //{
                //    FrmModal modal = new FrmModal("Debe de ingresar el NIT ó Cédula");
                //    modal.ShowDialog();
                //}
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

        #region HeaderButtons

        private void BtnBack_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            Dispatcher.BeginInvoke((Action)delegate
            {
                FrmMenu menu = new FrmMenu();
                menu.Show();
                this.Close();
            });
            GC.Collect();
        }

        private void BtnExit_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.GoToInicial();
        }

        #endregion

    }
}
