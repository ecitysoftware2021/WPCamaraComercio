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

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmMenu.xaml
    /// </summary>
    public partial class FrmMenu : Window
    {
        #region "Referencias"
        Utilities utilities;
        #endregion

        #region "Constructor"
        public FrmMenu()
        {
            InitializeComponent();
            utilities = new Utilities();
        }
        #endregion

        #region "HeaderButton"
        /// <summary>
        /// Botón que me redirecciona a la ventana inicial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHome_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Utilities.GoToInicial();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnHome_MouseDown", "FrmMenu", ex.ToString());
            }
        }
        #endregion

        #region "EventTimer"
        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }
        #endregion

        #region "Button"
        /// <summary>
        /// Botón que me direcciona a la venta de consulta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCamara_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.ResetTimer();
                    FrmMenu menu = new FrmMenu();
                    menu.Show();
                    this.Close();
                });
                GC.Collect();

            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnCamara_PreviewMouseDown", "FrmMenu", ex.ToString());
            }
        }
        #endregion

    }
}
