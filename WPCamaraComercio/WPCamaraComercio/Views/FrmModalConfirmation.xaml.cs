using System.Windows;
using System.Windows.Input;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FrmModalErrors.xaml
    /// </summary>
    public partial class FrmModalConfirmation : Window
    {
        public FrmModalConfirmation(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
        }

        private void BtnOk_StylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_StylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = false;
        }
    }
}
