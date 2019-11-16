using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Classes.UseFull;
using WPFCCMedellin.DataModel;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.ViewModel;

namespace WPFCCMedellin.UserControls
{
    /// <summary>
    /// Lógica de interacción para PayerUserControl.xaml
    /// </summary>
    public partial class PayerUserControl : UserControl
    {
        private DetailViewModel viewModel;

        private Transaction transaction;

        private ReaderBarCode readerBarCode;

        public PayerUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            if (readerBarCode == null)
            {
                readerBarCode = new ReaderBarCode();
            }

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
                    OptionsEntries = new CollectionViewSource(),
                    OptionsList = new List<TypeDocument>(),
                    SourceCheckId = ImagesUrlResource.ImageCheckIn,
                    SourceCheckName = ImagesUrlResource.ImageCheckOut,
                };

                viewModel.TypePayer = ETypePayer.Person;

                viewModel.LoadList(viewModel.TypePayer);
                cmb_type_id.SelectedIndex = 0;

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
                readerBarCode.callbackOut = data =>
                {
                    if (data != null)
                    {
                        viewModel.Value1 = data.Document;
                        viewModel.Value2 = data.FirstName;
                        viewModel.Value3 = data.LastName;
                    }
                };

                readerBarCode.callbackError = error =>
                {

                };

                readerBarCode.Start(Utilities.GetConfiguration("BarcodePort"), int.Parse(Utilities.GetConfiguration("BarcodeBaudRate")));
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
                    }
                    else
                    {
                        viewModel.TypePayer = ETypePayer.Establishment;


                        viewModel.SourceCheckId = ImagesUrlResource.ImageCheckOut;
                        viewModel.SourceCheckName = ImagesUrlResource.ImageCheckIn;

                        viewModel.Row2 = "(*)Razón Social";
                        viewModel.Row3 = "(*)Dirección";
                    }

                    viewModel.LoadList(viewModel.TypePayer);

                    viewModel.Value1 = string.Empty;
                    viewModel.Value2 = string.Empty;
                    viewModel.Value3 = string.Empty;
                    viewModel.Value4 = string.Empty;
                }
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
                    SaveTransaction(((TypeDocument)cmb_type_id.SelectedItem).Key);
                }
                else
                {
                    Utilities.ShowModal(MessageResource.InfoIncorrect, EModalType.Error);
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
                if (viewModel.Value1.Length < 6)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(viewModel.Value2))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(viewModel.Value3))
                {
                    return false;
                }

                if (viewModel.Value4.Length < 6)
                {
                    return false;
                }
                return true;
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
                        transaction.payer = new PAYER
                        {
                            IDENTIFICATION = viewModel.Value1,
                            NAME = viewModel.Value2,
                            PHONE = decimal.Parse(viewModel.Value4),
                            TYPE_PAYER = viewModel.TypePayer == ETypePayer.Person ? "Persona" : "Empresa",
                            TYPE_IDENTIFICATION = typeDocument
                        };

                        if (viewModel.TypePayer == ETypePayer.Person)
                        {
                            transaction.payer.LAST_NAME = viewModel.Value3;
                        }
                        else
                        {
                            transaction.payer.ADDRESS = viewModel.Value3;
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
                Utilities.navigator.Navigate(UserControlView.Main);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
