using System.Windows;
using System.Windows.Input;
using WPCamaraComercio.Models;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FrmModalDetailEstablish.xaml
    /// </summary>
    public partial class FrmModalDetailEstablish : Window
    {
        public FrmModalDetailEstablish(Details details)
        {
            InitializeComponent();
            TxbAddress.Text = details.dire;
            TxbBusinessName.Text = details.nombreest;
            TxbEnrollment.Text = details.mat;
            TxbState.Text = details.estado == "yes" ? "Activo" : "Inactivo";
        }

        private void BtnOk_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.Close();
        }
    }
}
