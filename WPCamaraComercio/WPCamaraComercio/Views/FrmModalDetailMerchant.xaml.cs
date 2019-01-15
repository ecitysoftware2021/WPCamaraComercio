using System.Windows;
using System.Windows.Input;
using WPCamaraComercio.Models;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FrmModalDetail.xaml
    /// </summary>
    public partial class FrmModalDetailMerchant : Window
    {
        public FrmModalDetailMerchant(MerchantDetail merchantDetail)
        {
            InitializeComponent();
            TxbBusinessName.Text = merchantDetail.rSocial;
            TxbNit.Text = merchantDetail.ident.Replace(",", ".").Trim();
            TxbSocietyType.Text = merchantDetail.tSociedad;
            TxbAddress.Text = string.Concat(merchantDetail.dComercial, " - ", merchantDetail.mun);
            TxbState.Text = merchantDetail.estado;
            TxbNumberEstablish.Text = merchantDetail.nEstablecimientos;
            TxbDateInitial.Text = merchantDetail.fInicio;
            TxbLastRenovation.Text = merchantDetail.uRenovacion;
        }

        private void BtnOk_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.Close();
        }
    }
}
