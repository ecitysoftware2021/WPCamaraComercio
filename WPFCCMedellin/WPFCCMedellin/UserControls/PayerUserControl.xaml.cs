using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Classes.UseFull;
using WPFCCMedellin.DataModel;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.ViewModel;
using WPFCCMedellin.Services;
using WPFCCMedellin.Services.Object;
using Newtonsoft.Json;

namespace WPFCCMedellin.UserControls
{
    /// <summary>
    /// Lógica de interacción para PayerUserControl.xaml
    /// </summary>
    public partial class PayerUserControl : UserControl
    {
        private DetailViewModel viewModel;

        private Transaction transaction;


        public PayerUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            ConfigView();
        }

        private void ConfigView()
        {
            try
            {
                viewModel = new DetailViewModel
                {
                    Row1 = "(*)Identificación:",
                    Row2 = "(*)Nombre:",
                    Row3 = "(*)Apellido:",
                    Row4 = "(*)Teléfono",
                    Row9 = "(*)Correo",
                    Row10 = "(*)Dirección",
                    OptionsEntries = new CollectionViewSource(),
                    DepartmenEntries = new CollectionViewSource(),
                    CityEntries = new CollectionViewSource(),
                    OptionsList = new List<MockupsModel>(),
                    DepartmentList = new List<MockupsModel>(),
                    CityList = new List<MockupsModel>(),
                    SourceCheckId = ImagesUrlResource.ImageCheckIn,
                    SourceCheckName = ImagesUrlResource.ImageCheckOut,
                };

                viewModel.Value1 = string.Empty;
                viewModel.Value2 = string.Empty;
                viewModel.Value3 = string.Empty;
                viewModel.Value4 = string.Empty;
                viewModel.Value9 = string.Empty;
                viewModel.Value10 = string.Empty;

                viewModel.TypePayer = ETypePayer.Person;

                viewModel.LoadList(viewModel.TypePayer);
                cmb_type_id.SelectedIndex = 0;
                cmb_department_id.SelectedIndex = 0;
                cmb_city_id.SelectedIndex = 0;

                this.DataContext = viewModel;

                InicielizeBarcodeReader();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void InicielizeBarcodeReader()
        {
            try
            {
                AdminPayPlus.ReaderBarCode.callbackOut = data =>
                {
                    if (data != null && viewModel.TypePayer == ETypePayer.Person)
                    {
                        viewModel.Value1 = data.Document;
                        viewModel.Value2 = data.FirstName;
                        viewModel.Value3 = data.LastName;
                        viewModel.Value4 = string.Empty;
                    }
                };

                AdminPayPlus.ReaderBarCode.callbackError = error =>
                {

                };

                AdminPayPlus.ReaderBarCode.Start(Utilities.GetConfiguration("BarcodePort"), int.Parse(Utilities.GetConfiguration("BarcodeBaudRate")));
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Select_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            try
            {
                var type = int.Parse(((Image)sender).Tag.ToString());

                if (type != (int)viewModel.TypePayer)
                {
                    if (type == (int)ETypePayer.Person)
                    {
                        viewModel.TypePayer = ETypePayer.Person;

                        viewModel.SourceCheckId = ImagesUrlResource.ImageCheckIn;
                        viewModel.SourceCheckName = ImagesUrlResource.ImageCheckOut;

                        viewModel.Row2 = "(*)Nombre";
                        viewModel.Row3 = "(*)Apellido";

                        TbxData3.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        viewModel.TypePayer = ETypePayer.Establishment;

                        viewModel.SourceCheckId = ImagesUrlResource.ImageCheckOut;
                        viewModel.SourceCheckName = ImagesUrlResource.ImageCheckIn;

                        viewModel.Row2 = "(*)Razón Social";
                        viewModel.Row3 = "";

                        TbxData3.Visibility = Visibility.Hidden;
                    }

                    viewModel.LoadList(viewModel.TypePayer);
                    viewModel.LoadListCity(new List<MockupsModel>());

                    viewModel.Value1 = string.Empty;
                    viewModel.Value2 = string.Empty;
                    viewModel.Value3 = string.Empty;
                    viewModel.Value4 = string.Empty;
                    viewModel.Value9 = string.Empty;
                    viewModel.Value10 = string.Empty;
                }

                cmb_type_id.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_payment_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            try
            {
                if (ValidateFields())
                {
                    SaveTransaction(((MockupsModel)cmb_type_id.SelectedItem).Key);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private bool ValidateFields()
        {
            try
            {
                bool state = true;
                string filds = string.Empty;
                if (viewModel.Value1.Length < 6)
                {
                    state = false;
                    filds += "Documento, ";
                }

                if (string.IsNullOrEmpty(viewModel.Value2))
                {
                    state = false;
                    filds += "Nombre o Razón Social, ";
                }
                if (viewModel.TypePayer == ETypePayer.Person)
                {
                    if (string.IsNullOrEmpty(viewModel.Value3))
                    {
                        state = false;
                        filds += "Apellido, ";
                    }
                }
                if (!Utilities.IsValidEmailAddress(viewModel.Value9))
                {
                    state = false;
                    filds += "Correo, ";
                }

                if (viewModel.Value4.Length < 6)
                {
                    state = false;
                    filds += "Teléfono, ";
                }
                if (viewModel.Value10.Length < 5)
                {
                    state = false;
                    filds += "Dirección, ";
                }
                if ((MockupsModel)cmb_department_id.SelectedItem == null)
                {
                    state = false;
                    filds += "Departamento, ";
                }
                if ((MockupsModel)cmb_city_id.SelectedItem == null)
                {
                    state = false;
                    filds += "Ciudad, ";
                }
                if (!state)
                {
                    Utilities.ShowModal(string.Concat("Los datos: ", Environment.NewLine, filds, " son requeridos!"), EModalType.Error);
                }
                return state;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
                return false;
            }
        }

        private void SaveTransaction(string typeDocument)
        {
            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        string codDepartamento = string.Empty;
                        string codCiudad = string.Empty;
                        string municipio = string.Empty;
                        await Dispatcher.BeginInvoke((Action)delegate
                         {
                             codDepartamento = ((MockupsModel)cmb_department_id.SelectedItem).Key;
                             codCiudad = ((MockupsModel)cmb_city_id.SelectedItem).Key;
                             municipio = ((MockupsModel)cmb_city_id.SelectedItem).Value;
                         });
                        transaction.payer = new PAYER
                        {
                            IDENTIFICATION = viewModel.Value1,
                            NAME = viewModel.Value2,
                            PHONE = decimal.Parse(viewModel.Value4),
                            TYPE_PAYER = viewModel.TypePayer == ETypePayer.Person ? "Persona" : "Empresa",
                            TYPE_IDENTIFICATION = typeDocument,
                            EMAIL = viewModel.Value9,
                            ADDRESS = viewModel.Value10,
                            codDepartamento = int.Parse(codDepartamento.Substring(codDepartamento.Length - 2, 2)),
                            codMunicipio = int.Parse(codCiudad),
                            municipio = municipio
                        };

                        if (viewModel.TypePayer == ETypePayer.Person)
                        {
                            transaction.payer.LAST_NAME = viewModel.Value3;
                        }

                        await AdminPayPlus.SaveTransactions(this.transaction, false);

                        Utilities.CloseModal();

                        if (this.transaction.IdTransactionAPi == 0)
                        {
                            Utilities.ShowModal(MessageResource.ErrorTransaction, EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                        else
                        {
                            AdminPayPlus.ReaderBarCode.callbackOut = null;
                            AdminPayPlus.ReaderBarCode.callbackError = null;
                            Utilities.navigator.Navigate(UserControlView.Pay, false, transaction);
                        }
                    }
                    catch (Exception ex)
                    {
                        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
                    }
                });
                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_exit_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            try
            {
                AdminPayPlus.ReaderBarCode.callbackOut = null;
                AdminPayPlus.ReaderBarCode.callbackError = null;
                Utilities.navigator.Navigate(UserControlView.Main);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void KeyboardNum_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, sender as TextBox, this);
        }

        private void Keyboard_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, sender as TextBox, this);
        }

        private void btSearchPayer_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (TbxIdentification.Text.Length > 5)
            {

                GetDataPayer();
            }
        }

        public void GetDataPayer()
        {
            try
            {
                Utilities.ShowModal("Consultando datos del pagador...", EModalType.Preload, openDialog: false);
                Task.Run(async () =>
                {
                    string tipoDoc = string.Empty;
                    string doc = string.Empty;
                    await Dispatcher.BeginInvoke((Action)delegate
                     {
                         tipoDoc = ((MockupsModel)cmb_type_id.SelectedItem).Key;
                         doc = TbxIdentification.Text;
                     });


                    var response = AdminPayPlus.ApiIntegration.GetData(new RequestIdentificacionComprador
                    {
                        TipoIdentificacionComprador = tipoDoc,
                        IdentificacionComprador = doc
                    }, "GetPayer").Result;

                    Utilities.CloseModal();
                    if (response.CodeError == 200 && response.Data != null)
                    {
                        var result = JsonConvert.DeserializeObject<ResponsePayer>(response.Data.ToString());

                        if (result != null && result.response != null && string.IsNullOrEmpty(result.response.codigo))
                        {
                            foreach (var item in result.response.resultados)
                            {
                                viewModel.Value9 = item.EmailComprador;
                                viewModel.Value4 = item.TelefonoComprador;
                                viewModel.Value10 = item.DireccionComprador;
                                if (viewModel.TypePayer == ETypePayer.Person)
                                {
                                    viewModel.Value2 = item.PrimerNombreComprador;
                                    viewModel.Value3 = item.PrimerApellidoComprador;
                                }
                                else
                                {
                                    viewModel.Value2 = item.NombreComprador;
                                }
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void cmb_department_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((MockupsModel)(sender as ComboBox).SelectedItem != null)
            {
                string codDepartamento = ((MockupsModel)(sender as ComboBox).SelectedItem).Key;
                GetCityByDepartment(codDepartamento.Substring(codDepartamento.Length - 2, 2));

            }
        }


        public void GetCityByDepartment(string codDepartamento)
        {
            cmb_city_id.IsEnabled = false;
            Task.Run(() =>
            {
                List<MockupsModel> models = new List<MockupsModel>();
                var response = AdminPayPlus.ApiIntegration.GetData(new RequestMunicipios
                {
                    pais = 169,
                    dpto = int.Parse(codDepartamento)
                }, "ConsultarMunicipios").Result;
                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (response.CodeError == 200 && response.Data != null)
                    {
                        var result = JsonConvert.DeserializeObject<City>(response.Data.ToString());

                        if (result != null && result.response != null && string.IsNullOrEmpty(result.response.codigo))
                        {
                            foreach (var item in result.response.resultados)
                            {
                                models.Add(new MockupsModel
                                {
                                    Value = item.nombre,
                                    Key = item.codigo
                                });
                            }
                        }
                    }


                    viewModel.LoadListCity(models);
                    cmb_city_id.SelectedItem = 0;
                    cmb_city_id.IsEnabled = true;
                });
            });

        }
    }
}
