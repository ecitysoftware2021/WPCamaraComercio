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

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPaymentData.xaml
    /// </summary>
    public partial class FrmPaymentData : Window
    {
        public FrmPaymentData()
        {
            InitializeComponent();
            CmbTypeBuyer.SelectedIndex = 0;
            CmbIdDType.SelectedIndex = 0;
        }

        private void BtnPay_StylusDown(object sender, StylusDownEventArgs e)
        {

        }

        public void FillTypeDocument(int type)
        {
            Dictionary<string, string> test = new Dictionary<string, string>();
            if (type == 1)
            {
                test.Add("N", "NIT");
            }
            else
            {
                test.Add("C", "Cédula de ciudadanía");
                test.Add("E", "Cédula de extranjería");
                test.Add("P", "Pasaporte");
                test.Add("I", "Tarjeta de identidad");
            }
            CmbIdDType.DisplayMember = "Value";
            CmbIdDType.ValueMember = "Key";
        }

        private void CmbIdDType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CmbTypeBuyer.SelectedIndex == 0)
                {
                    lblLastName.Visible = true;
                    lblPrimerApellido.Text = "Primer Apellido";
                    lblPrimerNombre.Text = "Primer Nombre";
                    lblSegundoNombre.Text = "Segundo Nombre";
                    txtPrimerApellido.Visible = true;
                    lblPrimerApellido.Visible = true;
                    FillTipoDocumento(0);
                    ddlTipoDocumento.SelectedIndex = 0;
                }
                else
                {
                    lblLastName.Visible = false;
                    lblPrimerApellido.Visible = false;
                    lblPrimerNombre.Text = "Razon Social";
                    lblSegundoNombre.Text = "Dirección";
                    txtPrimerApellido.Visible = false;
                    FillTipoDocumento(1);
                    ddlTipoDocumento.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                objUtil.Exception(ex.Message);
            }
        }
    }
}
