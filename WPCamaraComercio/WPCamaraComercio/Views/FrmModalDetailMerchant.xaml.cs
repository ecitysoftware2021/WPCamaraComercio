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
            //if (string.IsNullOrEmpty(merchantDetail.sigla))
            //{
            //    label12.Visible = false;
            //}
            //lblSigla.Text = objComerciante.sigla;
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
