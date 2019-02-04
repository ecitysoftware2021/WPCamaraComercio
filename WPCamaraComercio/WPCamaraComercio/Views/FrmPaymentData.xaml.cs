﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
using WPCamaraComercio.WCFPayPad;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPaymentData.xaml
    /// </summary>
    public partial class FrmPaymentData : Window
    {
        #region References
       // WCFPayPad.CLSTransaction transaction;
        WCFPayPadService payPadService;
        NavigationService navigationService;
        Utilities utilities;
        Api api;
        #endregion

        #region LoadMethods
        public FrmPaymentData()
        {
            InitializeComponent();
            //transaction = new WCFPayPad.CLSTransaction();
            payPadService = new WCFPayPadService();
            navigationService = new NavigationService(this);
            utilities = new Utilities();
            api = new Api();
            CmbTypeBuyer.SelectedIndex = 0;
            CmbIdDType.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }
        #endregion

        #region Methods
        private void Redirect()
        {
            AssingProperties();
            if (CreateTransaction())
            {
                Utilities.ResetTimer();
                navigationService.NavigationTo("FrmPayment"); 
            }
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
                utilities.SaveLogErrorMethods("FillTypeDocument", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void AssingProperties()
        {
            try
            {
                var data = Utilities.ConsultResult;
                string[] municipio = data.municipio.Split(')');
                string[] datosPais = municipio[0].ToString().Split('-');

                PayerData payerData = new PayerData();
                payerData.BuyerAddress = CmbTypeBuyer.SelectedIndex == 1 ? TbxData2.Text : "";
                payerData.BuyerIdentification = TbxIdentification.Text;
                payerData.LastNameBuyer = TbxData3.Text;
                payerData.FirstNameBuyer = TbxData1.Text;
                payerData.SecondNameBuyer = string.Empty;
                payerData.TypeBuyer = CmbTypeBuyer.Text;
                payerData.TypeIdBuyer = ((KeyValuePair<string, string>)CmbIdDType.SelectedItem).Key;
                payerData.Phone = CmbTypeBuyer.SelectedIndex == 1 ? TbxData3dos.Text : TbxData4.Text;
                payerData.Email = string.Empty;
                payerData.CodeCountryBuyer = int.Parse(datosPais[0].Replace("( ", ""));
                payerData.CodeDepartmentBuyer = int.Parse(datosPais[1]);
                payerData.CodeTownBuyer = int.Parse(datosPais[2]);
                payerData.FullNameBuyer = payerData.FirstNameBuyer + " " + payerData.SecondNameBuyer + " " + payerData.LastNameBuyer;
                payerData.ClientPlataform = "DISPENSADOR";

                Utilities.PayerData = payerData;
                utilities.InsertPayerData();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("AssingProperties", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Método para validar que todos los campos estén llenos
        /// </summary>
        /// <returns>Retorna true si todos esán llenos, false si algúno está vacío</returns>
        private bool ValidateFields()
        {
            try
            {
                bool flag = true;
                //Se recorren todos los controles del grid contenedor en busca de que esten llenos los necesarios
                foreach (var control in grdPaymentData.Children)
                {
                    if (control is TextBox)
                    {
                        TextBox textBox = (TextBox)control;

                        string value = textBox.Text;
                        if (value.Length < int.Parse(textBox.Tag.ToString()) || value.Length > textBox.MaxLength)
                        {
                            if (textBox.IsVisible)
                            {
                                flag = false;
                                ControlMessageError(textBox.Name, true);
                            }
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("ValidateFields", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
                return false;
            }
        }

        private bool CreateTransaction()
        {
            try
            {
                ServicePayPadClient WCFPayPad = new ServicePayPadClient();
                CLSTransaction transaction = new CLSTransaction();
                transaction.IDCorresponsal = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));
                transaction.IDTramite = int.Parse(Utilities.GetConfiguration("IDTramite")); ;
                transaction.Referencia = "0";
                transaction.CedulaPagador = TbxIdentification.Text;
                transaction.Contrato = string.Empty;
                transaction.FechaCuota = string.Empty;
                transaction.Total = Utilities.ValueToPay;
                Utilities.IDTransactionDB = WCFPayPad.InsertarTransaccion(transaction);
                return true;
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CreateTransaction", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
                return false;
            }
        }

        /// <summary>
        /// Método para manipular los mensajes de error
        /// </summary>
        /// <param name="tag">la etiqueta del campo al que se le va a manipular el tooltip.</param>
        /// <param name="state">True para mostrarlo, False para ocultarlo.</param>
        private void ControlMessageError(string tag, bool state)
        {
            try
            {
                switch (tag)
                {
                    case "TbxIdentification":
                        navigationService.NavigatorModal("Por favor, ingrese una identificación válida");
                        break;
                    case "TbxData1":
                        if (CmbTypeBuyer.SelectedIndex == 0)
                        {
                            navigationService.NavigatorModal("Por favor, ingrese un nombre válido");
                        }
                        else
                        {
                            navigationService.NavigatorModal("Por favor, ingrese una razón social válida");
                        }
                        break;
                    case "TbxData3":
                        if (CmbTypeBuyer.SelectedIndex == 0)
                        {
                            navigationService.NavigatorModal("Por favor, ingrese un apellido válido");
                        }
                        else
                        {
                            navigationService.NavigatorModal("Por favor, ingrese un teléfono válido");
                        }
                        break;
                    case "TbxData4":
                        if (TbxData4.Visibility == Visibility.Visible)
                        {
                            navigationService.NavigatorModal("Por favor, ingrese un teléfono válido");
                        }
                        break;
                    case "TbxData3dos":
                        if (TbxData3dos.Visibility == Visibility.Visible)
                        {
                            navigationService.NavigatorModal("Por favor, ingrese un teléfono válido");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("ControlMessageError", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }
        #endregion

        #region Events
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
                utilities.SaveLogErrorMethods("BtnPay_StylusDown", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
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
                    TbxData3dos.Visibility = Visibility.Hidden;
                    TbxData3.Visibility = Visibility.Visible;

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
                    TbxData3dos.Visibility = Visibility.Visible;
                    TbxData3.Visibility = Visibility.Hidden;

                    grdPaymentData.DataContext = data;

                    FillTypeDocument(1);
                    CmbIdDType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CmbTypeBuyer_SelectionChanged", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);
        #endregion

        #region HeaderButtons
        private void BtnBack_StylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            Utilities.PayerData = null;
            Utilities.ListCertificates.Clear();
            navigationService.NavigationTo("FrmDetailCompany");
        }

        private void BtnExit_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Utilities.ResetTimer();
                Utilities.PayerData = null;
                Utilities.ListCertificates.Clear();
                //Utilities.GoToInicial();
                Utilities.RestartApp();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("BtnExit_StylusDown", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }
        #endregion

        private void TbxIdentification_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TbxIdentification.Text.Length > TbxIdentification.MaxLength)
                {
                    TbxIdentification.Text = TbxIdentification.Text.Remove(TbxIdentification.MaxLength,1);
                }
                else if (TbxData1.Text.Length > TbxData1.MaxLength)
                {
                    TbxData1.Text = TbxData1.Text.Remove(TbxData1.MaxLength, 1);
                }
                else if (TbxData2.Text.Length > TbxData2.MaxLength)
                {
                    TbxData2.Text = TbxData2.Text.Remove(TbxData2.MaxLength, 1);
                }
                else if (TbxData3.Text.Length > TbxData3.MaxLength)
                {
                    TbxData3.Text = TbxData3.Text.Remove(TbxData3.MaxLength, 1);
                }
                else if (TbxData3dos.Text.Length > TbxData3dos.MaxLength)
                {
                    TbxData3dos.Text = TbxData3dos.Text.Remove(TbxData3dos.MaxLength,1);
                }
                else if (TbxData4.Text.Length > TbxData4.MaxLength)
                {
                    TbxData4.Text = TbxData4.Text.Remove(TbxData4.MaxLength, 1);
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("TbxIdentification_TextChanged", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }
    }
}
