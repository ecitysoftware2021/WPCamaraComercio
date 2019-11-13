using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFCCMedellin.Classes;
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

        private ETypePayer typePayer;

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
                    Row1 = "Identificación:",
                    Row2 = "Primer Nombre",
                    Row3 = "Primer Apellido",
                    Row4 = "Teléfono"
                };

                typePayer = ETypePayer.Person;

                viewModel.LoadList(typePayer);

                this.DataContext = viewModel;
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
                var type = (int)((Image)sender).Tag;

                if (type != (int)typePayer)
                {
                    if (type == (int)ETypePayer.Person)
                    {
                        typePayer = ETypePayer.Person;
                        
                        viewModel.Row2 = "Primer Nombre";
                        viewModel.Row3 = "Primer Apellido";
                    }
                    else
                    {
                        typePayer = ETypePayer.Establishment;

                        viewModel.Row2 = "Razón Social";
                        viewModel.Row3 = "Dirección";
                    }

                    viewModel.LoadList(typePayer);

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
                    SaveTransaction();
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
                return true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
                return false;
            }
        }

        private void SaveTransaction()
        {
            try
            {
                Task.Run(async () =>
                {
                    transaction.payer = new PAYER
                    {
                        IDENTIFICATION = viewModel.Value1,
                        NAME = viewModel.Value2,
                        PHONE = decimal.Parse(viewModel.Value4),
                        TYPE_PAYER = typePayer == ETypePayer.Person ? "Persona" : "Empresa",
                        TYPE_IDENTIFICATION = ((TypeDocument)cmb_type_id.SelectedItem).Key
                    };

                    if (typePayer == ETypePayer.Person)
                    {
                        transaction.payer.LAST_NAME = viewModel.Row3;
                    }
                    else
                    {
                        transaction.payer.ADDRESS = viewModel.Row3;
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
                });
                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
