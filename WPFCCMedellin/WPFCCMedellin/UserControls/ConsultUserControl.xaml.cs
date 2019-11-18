using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.ViewModel;

namespace WPFCCMedellin.UserControls
{
    /// <summary>
    /// Lógica de interacción para ConsultUserControl.xaml
    /// </summary>
    public partial class ConsultUserControl : UserControl
    {
        private Transaction transaction;

        private DataListViewModel viewModel;

        public ConsultUserControl()
        {
            InitializeComponent();

            ConfigurateView();
        }

        private void ConfigurateView()
        {
            try
            {
                viewModel = new DataListViewModel
                {
                    Colum1 = "Empresa",
                    Colum3 = "Municipio",
                    Colum4 = "Estado",
                    SourceCheckId = ImagesUrlResource.ImageCheckIn,
                    SourceCheckName = ImagesUrlResource.ImageCheckOut,
                    ViewList = new CollectionViewSource(),
                    DataList = new List<ItemList>(),
                    Message = MessageResource.EnterId,
                    VisibilityId = Visibility.Visible,
                    VisibilityName = Visibility.Hidden,
                    VisibilityPagination = Visibility.Hidden,
                    VisibilityNext = Visibility.Hidden,
                    VisibilityPrevius = Visibility.Hidden,
                    TypeConsult = EtypeConsult.Id,
                    CurrentPageIndex = 1
                };

                this.DataContext = viewModel;

                viewModel.ViewList.Filter += new FilterEventHandler(View_List_Filter);
                lv_data_list.DataContext = viewModel.ViewList;
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
                    viewModel.ViewList.View.Refresh();
                    lv_data_list.Items.Refresh();
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

                if (index >= (viewModel.CuantityItems * (viewModel.CurrentPageIndex - 1)) && index < (viewModel.CuantityItems * (viewModel.CurrentPageIndex)))
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

        private void Btn_pagination_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var typePagination = int.Parse(((Image)sender).Tag.ToString());

                if (typePagination == 1)
                {
                    if (viewModel.CurrentPageIndex < (viewModel.TotalPage))
                    {
                        viewModel.CurrentPageIndex++;
                        viewModel.ViewList.View.Refresh();
                    }

                    if (viewModel.CurrentPageIndex == (viewModel.TotalPage))
                    {
                        viewModel.VisibilityNext = Visibility.Hidden;
                    }

                    viewModel.VisibilityPrevius = Visibility.Visible;
                }
                else
                {
                    if (viewModel.CurrentPageIndex > 1)
                    {
                        viewModel.CurrentPageIndex--;
                        viewModel.ViewList.View.Refresh();
                    }

                    if (viewModel.CurrentPageIndex == 1)
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

        private void BtnConsult_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ValidateDocument())
                {
                    if (viewModel.TypeConsult == EtypeConsult.Id)
                    {
                        SearchData(text_id.Text, 1);
                    }
                    else
                    {
                        SearchData(text_name.Text, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void SearchData(object reference, int typeConsult)
        {
            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (typeConsult == 1)
                        {
                            var response = await viewModel.ConsultConcidences((string)reference, (int)viewModel.TypeConsult);

                            Utilities.CloseModal();

                            if (response)
                            {
                                viewModel.RefreshView(response);
                                ConfigureViewList();
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    if (viewModel.TypeConsult == EtypeConsult.Id)
                                    {
                                        text_id.Text = string.Empty;
                                    }
                                    else
                                    {
                                        text_name.Text = string.Empty;
                                    }
                                });

                                Utilities.ShowModal(MessageResource.ErrorCoincidences, EModalType.Error);

                                //TimerService.Reset();
                            }
                        }
                        else
                        {
                            var response = await viewModel.ConsultDetails((ItemList)reference);

                            Utilities.CloseModal();

                            if (response != null)
                            {
                                this.transaction = response;

                                if (Utilities.ShowModal(string.Concat("Ha seleccionado ", transaction.payer.NAME, ", ¿desea continuar?"), EModalType.Information, false))
                                {
                                    Utilities.navigator.Navigate(UserControlView.Certificates, true, transaction);
                                }
                                else
                                {
                                    ValidateListView(true);
                                }
                            }
                            else
                            {
                                Utilities.ShowModal(MessageResource.ErrorFoundFiles, EModalType.Error);
                                ValidateListView(true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.CloseModal();
                        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
                    }
                });

                Utilities.ShowModal(MessageResource.ConsultingConinsidences, EModalType.Preload);
            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void ValidateListView(bool activateList)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Thread.Sleep(300);
                    this.IsEnabled = activateList;
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private bool ValidateDocument()
        {
            try
            {
                if (viewModel.TypeConsult == EtypeConsult.Id)
                {
                    if (string.IsNullOrEmpty(text_id.Text) || text_id.Text.Length <= 6 || text_id.Text.Length >= 12)
                    {
                        txt_error.Text = MessageResource.ErrorId;
                        return false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(text_name.Text))
                    {
                        txt_error.Text = MessageResource.ErrorName;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
                return false;
            }
        }

        private void Lv_data_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (ItemList)lv_data_list.SelectedItem;
                if (item != null)
                {
                    ValidateListView(false);
                    SearchData(item, 2);
                    lv_data_list.SelectedItem = null;
                }
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

                if (typeConsul != (int)viewModel.TypeConsult)
                {
                    viewModel.RefreshView(false);

                    if (typeConsul == (int)EtypeConsult.Id)
                    {
                        viewModel.TypeConsult = EtypeConsult.Id;
                        viewModel.VisibilityId = Visibility.Visible;
                        viewModel.VisibilityName = Visibility.Hidden;

                        text_id.Text = "";
                        text_name.Text = "";

                        viewModel.SourceCheckId = ImagesUrlResource.ImageCheckIn;
                        viewModel.SourceCheckName = ImagesUrlResource.ImageCheckOut;
                        viewModel.Tittle = MessageResource.EnterId;
                    }
                    else
                    {
                        viewModel.TypeConsult = EtypeConsult.Name;
                        viewModel.VisibilityId = Visibility.Hidden;
                        viewModel.VisibilityName = Visibility.Visible;

                        text_id.Text = "";
                        text_name.Text = "";

                        viewModel.SourceCheckId = ImagesUrlResource.ImageCheckOut;
                        viewModel.SourceCheckName = ImagesUrlResource.ImageCheckIn;
                        viewModel.Tittle = MessageResource.EnterName;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_exit_TouchDown(object sender, TouchEventArgs e)
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

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txt_error.Text = "";
                if (viewModel.TypeConsult == EtypeConsult.Id)
                {
                    if (text_id.Text.Length > 12)
                    {
                        text_id.Text = text_id.Text.Substring(0, (text_id.Text.Length - 1));
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Text_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                viewModel.RefreshView(false);
                viewModel.DataList.Clear();
                ConfigureViewList();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
