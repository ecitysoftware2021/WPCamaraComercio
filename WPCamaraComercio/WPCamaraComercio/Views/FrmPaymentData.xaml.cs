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
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPaymentData.xaml
    /// </summary>
    public partial class FrmPaymentData : Window
    {
        WCFPayPad.CLSTransaction transaction;
        WCFPayPadService WCFPayPad;
        NavigationService navigationService;

        public FrmPaymentData()
        {
            InitializeComponent();
            transaction = new WCFPayPad.CLSTransaction();
            WCFPayPad = new WCFPayPadService();
            navigationService = new NavigationService(this);
            CmbTypeBuyer.SelectedIndex = 0;
            CmbIdDType.SelectedIndex = 0;
        }

        private void BtnPay_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                if (ValidateFields())
                {
                    Redirect();
                }
            }
            catch (Exception ex)
            {
                //objUtil.Exception(ex.Message);
            }
        }

        private void Redirect()
        {
            AssingProperties();
            //CreateTransaction();

            Utilities.ResetTimer();
            navigationService.NavigationTo("FrmPayment");
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
                    TbxData4.Visibility = Visibility.Visible;

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
                    TbxData4.Visibility = Visibility.Hidden;

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

        private void AssingProperties()
        {
            var data = Utilities.ConsultResult;
            string[] municipio = data.municipio.Split(')');
            string[] datosPais = municipio[0].ToString().Split('-');

            PayerData payerData = new PayerData();
            payerData.BuyerAddress = CmbTypeBuyer.SelectedIndex == 1 ? TbxData2.Text : "";
            payerData.BuyerIdentification = TbxIdentification.Text;
            payerData.LastNameBuyer = TbxData3.Text;
            payerData.FirstNameBuyer = TbxData1.Text;
            payerData.SecondNameBuyer = CmbTypeBuyer.SelectedIndex != 1 ? TbxData2.Text : "";
            payerData.TypeBuyer = CmbTypeBuyer.Text;
            payerData.TypeIdBuyer = ((KeyValuePair<string, string>)CmbIdDType.SelectedItem).Key;
            payerData.Phone = CmbTypeBuyer.SelectedIndex == 1 ? TbxData3.Text : TbxData4.Text;
            payerData.Email = string.Empty;
            payerData.CodeCountryBuyer = int.Parse(datosPais[0].Replace("( ", ""));
            payerData.CodeDepartmentBuyer = int.Parse(datosPais[1]);
            payerData.CodeTownBuyer = int.Parse(datosPais[2]);
            payerData.FullNameBuyer = payerData.FirstNameBuyer + " " + payerData.SecondNameBuyer + " " + payerData.LastNameBuyer;
            payerData.ClientPlataform = "DISPENSADOR";

            Utilities.PayerData = payerData;
        }

        private void CreateTransaction()
        {
            transaction.IDCorresponsal = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));
            transaction.IDTramite = int.Parse(Utilities.GetConfiguration("IDTramite")); ;
            transaction.Referencia = "0";
            transaction.Total = Utilities.ValueToPay;
            Utilities.IDTransactionDB = WCFPayPad.InsertarTransaccion(transaction);
        }

        /// <summary>
        /// Método para validar que todos los campos estén llenos
        /// </summary>
        /// <returns>Retorna true si todos esán llenos, false si algúno está vacío</returns>
        private bool ValidateFields()
        {
            bool flag = false;

            //Se recorren todos los controles del grid contenedor en busca de que esten llenos los necesarios
            foreach (var control in grdPaymentData.Children)
            {
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    string value = textBox.Text;
                    if (string.IsNullOrEmpty(value) || value.Contains("Ingrese") || value.Length <= 3)
                    {
                        ControlMessageError(textBox.Tag.ToString(), true);
                        flag = false;
                    }
                    else
                        flag = true;
                }
                else if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;
                    if (comboBox.SelectedIndex == 0)
                    {
                        ControlMessageError(comboBox.Tag.ToString(), true);
                        flag = false;
                    }
                    else
                        flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// Método para manipular los mensajes de error
        /// </summary>
        /// <param name="tag">la etiqueta del campo al que se le va a manipular el tooltip.</param>
        /// <param name="state">True para mostrarlo, False para ocultarlo.</param>
        private void ControlMessageError(string tag, bool state)
        {
            switch (tag)
            {
                case "NumberId":
                    if (state)
                    {
                        //lblErrorMessageNumber.Visibility = Visibility.Visible;
                        return;
                    }
                    //lblErrorMessageNumber.Visibility = Visibility.Hidden;
                    break;
                case "DepositValue":
                    if (state)
                    {
                        //lblErrorMessageValue.Visibility = Visibility.Visible;
                        return;
                    }
                    //lblErrorMessageValue.Visibility = Visibility.Hidden;
                    break;
                case "Type":
                    if (state)
                    {
                        //lblErrorMessageId.Visibility = Visibility.Visible;
                        return;
                    }
                    //lblErrorMessageId.Visibility = Visibility.Hidden;
                    break;
                case "NumberAccount":
                    if (state)
                    {
                        //lblErrorNumberAccount.Visibility = Visibility.Visible;
                        return;
                    }
                    //lblErrorMessageId.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void BtnBack_StylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            navigationService.NavigationTo("FrmDetailCompany");
        }

        private void BtnExit_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Utilities.GoToInicial();
            }
            catch (Exception ex)
            {
                //utilities.saveLogError("BtnHome_MouseDown", "FrmMenu", ex.ToString());
            }
        }

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }
    }
}
