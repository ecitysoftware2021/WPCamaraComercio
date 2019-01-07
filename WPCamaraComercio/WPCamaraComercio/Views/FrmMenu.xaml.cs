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
        #region References

        Utilities utilities;

        #endregion

        #region LoadMethods

        public FrmMenu()
        {
            InitializeComponent();
            utilities = new Utilities();
        }

        #endregion

        #region HeaderButtons

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

        #region Timer

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }

        #endregion

        #region Events

        private void BtnCamara_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
               Utilities.ResetTimer();
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ConsultWindow menu = new ConsultWindow();
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
