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
        string data1 = string.Empty;
        string data2 = string.Empty;
        string data3 = string.Empty;
        string data4 = string.Empty;

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
            CmbIdDType.DisplayMemberPath = "Value";
            CmbIdDType.SelectedValuePath = "Key";
        }

        private void CmbTypeBuyer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CmbTypeBuyer.SelectedIndex == 0)
                {
                    data1 = "Primer Nombre";
                    data2 = "Segundo Nombre";
                    data3 = "Primer Apellido";
                    data4 = "Teléfono";

                    FillTypeDocument(0);
                    CmbIdDType.SelectedIndex = 0;
                }
                else
                {
                    data1 = "Razón Social";
                    data2 = "Dirección";
                    data3 = "Teléfono";
                    data4 = "";
                    TxbData4.Visibility = Visibility.Hidden;
                    TbxInfo4.Visibility = Visibility.Hidden;

                    FillTypeDocument(1);
                    CmbIdDType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                //objUtil.Exception(ex.Message);
            }
        }

    }
}
