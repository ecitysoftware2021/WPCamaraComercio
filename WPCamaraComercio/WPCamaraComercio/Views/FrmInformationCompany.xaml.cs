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
using WPCamaraComercio.Service;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmInformacionEmpresa.xaml
    /// </summary>
    public partial class FrmInformationCompany : Window
    {
        NavigationService navigationService;

        public FrmInformationCompany()
        {
            InitializeComponent();
            navigationService = new NavigationService(this);
        }

        #region Timer
        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }
        #endregion

        #region HeaderButtons
        private void BtnExit_StylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            Utilities.GoToInicial();
        }

        private void BtnBack_StylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            navigationService.NavigationTo("FrmConsult");
        }
        #endregion

        private void BtnContinue_StylusDown(object sender, StylusDownEventArgs e)
        {

        }

    }
}
