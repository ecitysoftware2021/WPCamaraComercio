using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
//using WPCamaraComercio.WCFPayPad;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPaymentData.xaml
    /// </summary>
    public partial class FrmPaymentData : Window
    {
        #region References
        //WCFPayPad.CLSTransaction transaction;
        WCFPayPadService WCFPayPad;
        NavigationService navigationService;
        Utilities utilities;
        Api api;
        private LogErrorGeneral log;
        private int Num = 0;
        #endregion

        #region LoadMethods
        public FrmPaymentData()
        {
            InitializeComponent();
            //transaction = new WCFPayPad.CLSTransaction();
            WCFPayPad = new WCFPayPadService();
            navigationService = new NavigationService(this);
            utilities = new Utilities();
            api = new Api();
            log = new LogErrorGeneral();
            CmbTypeBuyer.SelectedIndex = 0;
            CmbIdDType.SelectedIndex = 0;
            //CamaraComercio CM = new CamaraComercio();
            //CM.StartValues();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }
        #endregion

        #region Methods
        private async void Redirect()
        {
            AssingProperties();
            if (await CreateTransaction())
            {
                CreateLog();
                Utilities.ResetTimer();
                navigationService.NavigationTo("FrmPayment");
            }
            else
            {
                navigationService.NavigatorModal("No se pudo crear la transacción, por favor intente más tarde.");
                BtnPay.IsEnabled = true;
            }
        }

        private void CreateLog()
        {
            try
            {
                log.IdTransaction = Utilities.IDTransactionDB;
                log.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                log.Description = "Se crea la transacción y se deja en estado iniciada";
                log.ValuePay = Utilities.ValueToPay;
                log.IDCorresponsal = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));
                log.State = "Iniciada";
                Utilities.SaveLogTransactions(log, "LogTransacciones\\Iniciadas");
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CreateLog", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private async Task<bool> CreateTransaction()
        {
            try
            {
                string Nombre,Apellido,Identificacion;
                decimal Telefono;

                if (Num == 0)
                {
                    Identificacion = TbxIdentification.Text;
                    Nombre = TbxData1.Text;
                    Apellido = TbxData3.Text;
                    Telefono = Convert.ToDecimal(TbxData4.Text);
                }
                else
                {
                    Identificacion = TbxIdentification.Text;
                    Nombre = TbxData1.Text;
                    Apellido = "";
                    Telefono = Convert.ToDecimal(TbxData3.Text);
                }

                return await AdminPaypad.CreateTransaction(Identificacion,Nombre,Apellido,Telefono);
                //CLSTransaction transaction = new CLSTransaction();
                //transaction.IDCorresponsal = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));
                //transaction.IDTramite = int.Parse(Utilities.GetConfiguration("IDTramite"));
                //transaction.Referencia = "0";
                //transaction.CedulaPagador = TbxIdentification.Text;
                //transaction.Contrato = string.Empty;
                ////transaction.PersonaID = 1;
                //transaction.FechaCuota = string.Empty;
                //transaction.Total = Utilities.ValueToPay;
                //Utilities.IDTransactionDB = WCFPayPad.InsertarTransaccion(transaction);
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CreateTransaction", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
                return false;
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
                payerData.BuyerAddress = string.Empty;
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
                //utilities.InsertPayerData();
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
                BtnPay.IsEnabled = false;
                if (ValidateFields())
                {
                    Redirect();
                }
                else
                {
                    BtnPay.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("BtnPay_StylusDown", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
                BtnPay.IsEnabled = true;
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

        private void Window_StylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.time = TimeSpan.Parse(Utilities.Duration);
        }
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
                Utilities.GoToInicial();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("BtnExit_StylusDown", "FrmPaymentData", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }
        #endregion
    }
}
