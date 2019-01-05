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
using WPCamaraComercio.Objects;

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
            try
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

                CmbIdDType.ItemsSource = test;
            }
            catch (Exception ex)
            {

            }
        }

        private void CmbTypeBuyer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PaymentData data = new PaymentData();

                if (CmbTypeBuyer.SelectedIndex == 0)
                {
                    data.Data1 = "Primer Nombre";
                    data.Data2 = "Segundo Nombre";
                    data.Data3 = "Primer Apellido";
                    data.Data4 = "Teléfono";
                    TxbData4.Visibility = Visibility.Visible;
                    TbxInfo4.Visibility = Visibility.Visible;

                    grdPaymentData.DataContext = data;

                    FillTypeDocument(0);
                    CmbIdDType.SelectedIndex = 0;
                }
                else
                {
                    data.Data1 = "Razón Social";
                    data.Data2 = "Dirección";
                    data.Data3 = "Teléfono";
                    data.Data4 = "";
                    TxbData4.Visibility = Visibility.Hidden;
                    TbxInfo4.Visibility = Visibility.Hidden;

                    grdPaymentData.DataContext = data;

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
