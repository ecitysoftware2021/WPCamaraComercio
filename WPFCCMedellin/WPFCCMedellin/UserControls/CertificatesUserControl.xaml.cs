using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Input;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.ViewModel;
using System.Reflection;

namespace WPFCCMedellin.UserControls
{
    /// <summary>
    /// Lógica de interacción para CertificatesUserControl.xaml
    /// </summary>
    public partial class CertificatesUserControl : UserControl
    {
        private Transaction transaction;

        private DataListViewModel viewModel;

        private ETypeCertificate typeCertificates;

        public CertificatesUserControl(Transaction transaction)
        {
            InitializeComponent();

            typeCertificates = ETypeCertificate.Merchant;
        }

        private void ConfigurateView()
        {
            try
            {
                if (transaction.Files != null && transaction.Files.Count > 0)
                {
                    viewModel = new DataListViewModel
                    {
                        ViewList = new CollectionViewSource(),
                        VisibilityId = Visibility.Visible,
                        VisibilityName = Visibility.Hidden,
                        VisibilityPagination = Visibility.Hidden,
                        VisibilityNext = Visibility.Hidden,
                        VisibilityPrevius = Visibility.Hidden,
                    };

                    viewModel.LoadDataList(transaction, ETypeCertificate.Merchant);

                    this.DataContext = viewModel;
                }
                else
                {
                    Utilities.ShowModal(MessageResource.NoInfoShow, EModalType.Error);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_details_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Utilities.ShowDetailsModal(transaction.Files[0], ETypeCertificate.Establishment);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_back_TouchDown(object sender, TouchEventArgs e)
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

        private void Btn_merchant_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (typeCertificates == ETypeCertificate.Establishment)
                {
                    viewModel.LoadDataList(transaction, ETypeCertificate.Merchant);
                    ConfigureViewList();
                    typeCertificates = ETypeCertificate.Merchant;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_establishment_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (typeCertificates == ETypeCertificate.Merchant)
                {
                    viewModel.LoadDataList(transaction, ETypeCertificate.Establishment);
                    ConfigureViewList();
                    typeCertificates = ETypeCertificate.Establishment;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_acept_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                LoadFiles();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void ConfigureViewList()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    viewModel.ViewList.Source = viewModel.DataList;
                    viewModel.ViewList.Filter += new FilterEventHandler(View_List_Filter);

                    if (typeCertificates == ETypeCertificate.Merchant)
                    {
                        LvMerchant.DataContext = viewModel.ViewList;
                        LvMerchant.Items.Refresh();
                    }
                    else
                    {
                        LvEstablish.DataContext = viewModel.ViewList;
                        LvEstablish.Items.Refresh();
                    }
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void View_List_Filter(object sender, FilterEventArgs e)
        {
            try
            {
                int index = viewModel.DataList.IndexOf((ItemList)e.Item);

                if (index >= (viewModel.CuantityItems * viewModel.CurrentPageIndex) && index < (viewModel.CuantityItems * (viewModel.CurrentPageIndex + 1)))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_menus_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var index = int.Parse(((Image)sender).Tag.ToString());
                if (index > 0)
                {
                    if (viewModel.DataList[index].Item6 > 0)
                    {
                        viewModel.DataList[index].Item6 -= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_plas_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var index = int.Parse(((Image)sender).Tag.ToString());
                if (index < 5)
                {
                    if (viewModel.DataList[index].Item6 < 5)
                    {
                        viewModel.DataList[index].Item6 += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_pagination_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var typePagination = (int)((Image)sender).Tag;

                if (typePagination == 1)
                {
                    if (viewModel.CurrentPageIndex < (viewModel.TotalPage - 1))
                    {
                        viewModel.CurrentPageIndex++;
                        viewModel.ViewList.View.Refresh();
                    }

                    if (viewModel.CurrentPageIndex == (viewModel.TotalPage - 1))
                    {
                        viewModel.VisibilityNext = Visibility.Hidden;
                    }

                    viewModel.VisibilityPrevius = Visibility.Visible;
                }
                else
                {
                    if (viewModel.CurrentPageIndex > 0)
                    {
                        viewModel.CurrentPageIndex--;
                        viewModel.ViewList.View.Refresh();
                    }

                    if (viewModel.CurrentPageIndex == 0)
                    {
                        viewModel.VisibilityPrevius = Visibility.Hidden;
                    }

                    viewModel.VisibilityNext = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void LoadFiles()
        {
            try
            {
                transaction = viewModel.GetListFiles(typeCertificates, transaction);
                if (transaction.Products != null && transaction.Products.Count > 0)
                {
                    transaction.Files = null;

                    Utilities.navigator.Navigate(UserControlView.Payer, true, transaction);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
