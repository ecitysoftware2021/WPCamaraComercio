using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services.Object;

namespace WPFCCMedellin.UserControls.Administrator
{
    /// <summary>
    /// Lógica de interacción para AdministratorUserControl.xaml
    /// </summary>
    public partial class AdministratorUserControl : UserControl
    {
        #region "Referencias"
        private PaypadOperationControl dataContol;

        private int type = 1;

        private ETypeAdministrator typeOperation;
        #endregion

        #region "Constructor"
        public AdministratorUserControl(PaypadOperationControl dataContol, ETypeAdministrator typeOperation)
        {
            InitializeComponent();

            this.dataContol = dataContol;

            this.typeOperation = typeOperation;

            InitView();
        }
        #endregion

        #region "Eventos"
        private async void UpdateDataControl()
        {
            try
            {
                Task.Run(async () =>
                {

                    this.dataContol.viewList = null;

                    var responseApi = await AdminPayPlus.UpdateBalance(this.dataContol);
                    if (responseApi)
                    {
                        Utilities.PrintVoucher(this.dataContol, typeOperation);

                        Utilities.CloseModal();

                        Utilities.ShowModal(MessageResource.TransactionFinish, EModalType.Error, false);

                        Utilities.RestartApp();
                    }

                    Utilities.CloseModal();
                    Utilities.ShowModal(MessageResource.ErrorTransaction, EModalType.Error, false);
                    Utilities.RestartApp();
                });

                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload, false);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
        #endregion

        #region "ListView"
        private void InitView()
        {
            try
            {
                this.DataContext = dataContol;

                if (typeOperation == ETypeAdministrator.Balancing)
                {
                    txtDescription.Text = MessageResource.RemoveMonyAceptance;
                }
                else
                {
                    txtDescription.Text = MessageResource.EnterMonyDispenser;
                }

                RefreshList();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void RefreshList()
        {
            try
            {
                dataContol.viewList = new CollectionViewSource();
                if (this.type == 1 && this.typeOperation == ETypeAdministrator.Balancing)
                {
                    dataContol.viewList.Source = dataContol.DATALIST.Where(x => (x.DEVICE_TYPE_ID == 2 || x.DEVICE_TYPE_ID == 4) && x.AMOUNT_NEW > 0).ToList();
                }
                else
                {
                    dataContol.viewList.Source = dataContol.DATALIST.Where(x => (x.DEVICE_TYPE_ID == 3 || x.DEVICE_TYPE_ID == 8) && x.AMOUNT_NEW > 0).ToList();
                }

                Dispatcher.BeginInvoke((Action)delegate
                {
                    lv_denominations.DataContext = dataContol.viewList;
                    lv_denominations.Items.Refresh();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
        #endregion

        private void BtnNext_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (type == 1 && typeOperation == ETypeAdministrator.Balancing)
                {
                    type = 2;
                    txtDescription.Text = MessageResource.RemoveMonyDispenser;
                    RefreshList();
                }
                else
                {
                    UpdateDataControl();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void BtnCancell_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Utilities.navigator.Navigate(UserControlView.Config);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}