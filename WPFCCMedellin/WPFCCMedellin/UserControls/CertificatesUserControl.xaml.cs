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
using System.Collections.Generic;

namespace WPFCCMedellin.UserControls
{
    /// <summary>
    /// Lógica de interacción para CertificatesUserControl.xaml
    /// </summary>
    public partial class CertificatesUserControl : UserControl
    {
        private Transaction transaction;

        private DataListViewModel viewModel;

        public CertificatesUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            ConfigurateView();
        }

        private void ConfigurateView()
        {
            try
            {
                if (transaction.Files != null && transaction.Files.Count > 0)
                {
                    viewModel = new DataListViewModel
                    {
                        DataList = new List<ItemList>(),
                        TypeCertificates = ETypeCertificate.Merchant,
                        ViewList = new CollectionViewSource(),
                        VisibilityId = Visibility.Visible,
                        VisibilityName = Visibility.Hidden,
                        VisibilityPagination = Visibility.Hidden,
                        VisibilityNext = Visibility.Hidden,
                        VisibilityPrevius = Visibility.Hidden,
                        Message = transaction.payer.NAME,
                        Amount = 0
                    };

                    viewModel.LoadDataList(transaction, ETypeCertificate.Merchant);
                    this.DataContext = viewModel;
                    ConfigureViewList();
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
                Utilities.ShowDetailsModal(transaction.Files[0], viewModel.TypeCertificates);
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

        private void Select_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var typeConsul = int.Parse(((Image)sender).Tag.ToString());

                if (typeConsul != (int)viewModel.TypeCertificates)
                {
                    viewModel.RefreshView(false);
                    viewModel.Amount = 0;

                    if (viewModel.TypeCertificates == ETypeCertificate.Establishment)
                    {
                        viewModel.TypeCertificates = ETypeCertificate.Merchant;
                        viewModel.VisibilityId = Visibility.Visible;
                        viewModel.VisibilityName = Visibility.Hidden;
                        btn_merchant.Opacity = 1;
                        btn_establishment.Opacity = 0.4;

                        viewModel.LoadDataList(transaction, ETypeCertificate.Merchant);
                        ConfigureViewList();
                    }
                    else
                    {
                        viewModel.TypeCertificates = ETypeCertificate.Establishment;
                        viewModel.VisibilityId = Visibility.Hidden;
                        viewModel.VisibilityName = Visibility.Visible;
                        btn_merchant.Opacity = 0.4;
                        btn_establishment.Opacity = 1;

                        viewModel.LoadDataList(transaction, ETypeCertificate.Establishment);
                        ConfigureViewList();
                    }
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

                    if (viewModel.TypeCertificates == ETypeCertificate.Merchant)
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
                
                if (viewModel.DataList[index].Item6 > 0)
                {
                    viewModel.DataList[index].Item6 -= 1;
                    viewModel.Amount -= viewModel.DataList[index].Item5;
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
                
                if (viewModel.DataList[index].Item6 < 5)
                {
                    viewModel.DataList[index].Item6 += 1;
                    viewModel.Amount += viewModel.DataList[index].Item5;
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
                transaction = viewModel.GetListFiles(viewModel.TypeCertificates, transaction);
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

        private void Btn_exit_TouchDown(object sender, TouchEventArgs e)
        {

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {

        }
    }
}
