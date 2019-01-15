using System.Windows;
using System.Windows.Input;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FrmModalErrors.xaml
    /// </summary>
    public partial class FrmModalErrors : Window
    {
        public FrmModalErrors(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
        }

        private void BtnOk_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.Close();
        }
    }
}
