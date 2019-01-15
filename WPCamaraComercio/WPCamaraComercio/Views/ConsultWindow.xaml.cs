using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;
using WPCamaraComercio.ViewModels;

namespace WPCamaraComercio.Views
{
    public partial class ConsultWindow : Window
    {
        #region References
        private int currentPageIndex = 0;
        private int itemPerPage = 12;
        private int totalPage = 0;
        WCFServices services;
        private Utilities utilities;
        private ConsultViewModel consultViewModel;
        NavigationService navigationService;
        #endregion

        #region LoadeMethods
        public ConsultWindow()
        {
            InitializeComponent();
            services = new WCFServices();
            utilities = new Utilities();
            navigationService = new NavigationService(this);
            Init(true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }
        #endregion

        #region Methods
        private void Init(bool initData)
        {
            try
            {
                consultViewModel = new ConsultViewModel
                {
                    headers = Visibility.Hidden,
                    preload = Visibility.Hidden,
                    coincidences = new List<Coincidence>(),
                    viewList = new CollectionViewSource(),
                    sourceCheckName = Utilities.GetConfiguration("ImageCheckOut"),
                    sourceCheckNit = Utilities.GetConfiguration("ImageCheckIn"),
                    message = "Ingrese NIT/Cédula sin dígito de verificación",
                    typeSearch = 2
                };

                if (initData)
                {
                    this.DataContext = consultViewModel;

                    this.consultViewModel.callbackSearch = stateConsult =>
                    {
                        if (!stateConsult)
                        {
                            Utilities.OpenModal("No se encontraron resultados para la busqueda", this);
                            BtnConsultar.IsEnabled = true;
                        }
                        else
                        {
                            this.consultViewModel.headers = Visibility.Visible;
                            CreatePages(consultViewModel.countConcidences);
                            BtnConsultar.IsEnabled = true;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("Init", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void CreatePages(int i)
        {
            try
            {
                int itemcount = i;

                //Calcular el total de páginas que tendrá la vista
                totalPage = itemcount / itemPerPage;
                if (itemcount % itemPerPage != 0)
                {
                    totalPage += 1;
                }

                //Cuando sólo haya una página se ocultaran los botónes de Next y Prev
                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (totalPage == 1)
                    {
                        btnNext.Visibility = Visibility.Hidden;
                        btnPrev.Visibility = Visibility.Hidden;
                    }
                    else if (totalPage > 1)
                    {
                        btnNext.Visibility = Visibility.Visible;
                    }
                    tbTotalPage.Text = totalPage.ToString();
                    ShowCurrentPageIndex();
                });
                GC.Collect();

                Dispatcher.BeginInvoke((Action)delegate
                {   
                    consultViewModel.viewList.Source = consultViewModel.coincidences;
                    consultViewModel.viewList.Filter += new FilterEventHandler(View_Filter);
                    lv_Files.DataContext = consultViewModel.viewList;
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CreatePages", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void LoadInfoDetails(Coincidence coincidence)
        {
            try
            {
                if (coincidence != null)
                {
                    LoadingModal modal = new LoadingModal(coincidence);
                    modal.ShowDialog();

                    lv_Files.SelectedItem = null;
                    if (modal.DialogResult.Value)
                    {
                        Utilities.ResetTimer();
                        navigationService.NavigationTo("FrmDetailCompany");
                    }
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("LoadInfoDetails", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void ShowCurrentPageIndex()
        {
            tbCurrentPage.Text = (currentPageIndex + 1).ToString();
        }

        private void ConsulParameter()
        {
            try
            {
                RestoreView();
                string valueSearch = string.Empty;

                if (consultViewModel.typeSearch == 1)
                {
                    valueSearch = TxtName.Text.ToString();
                }
                else
                {
                    valueSearch = TxtIdentificacion.Text.ToString();
                }

                if (!string.IsNullOrWhiteSpace(valueSearch))
                {
                    consultViewModel.ConsultConcidences(valueSearch, consultViewModel.typeSearch);
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("ConsulParameter", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void RestoreView()
        {
            this.currentPageIndex = 0;
            this.totalPage = 0;
            consultViewModel.coincidences.Clear();
            this.consultViewModel.headers = Visibility.Hidden;
        }
        #endregion

        #region Events
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void View_Filter(object sender, FilterEventArgs e)
        {
            try
            {
                int index = consultViewModel.coincidences.IndexOf((Coincidence)e.Item);

                if (index >= itemPerPage * currentPageIndex && index < itemPerPage * (currentPageIndex + 1))
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
                utilities.SaveLogErrorMethods("View_Filter", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void BtnConsultar_StylusDown(object sender, StylusDownEventArgs e)
        {
            BtnConsultar.IsEnabled = false;
            ConsulParameter();
        }

        private void chkIdentification_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                if (consultViewModel.typeSearch != 2)
                {
                    RestoreView();

                    this.consultViewModel.typeSearch = 2;

                    this.TxtIdentificacion.Text = string.Empty;

                    this.TxtName.Text = string.Empty;

                    this.TxtIdentificacion.Visibility = Visibility.Visible;

                    this.TxtName.Visibility = Visibility.Hidden;

                    this.consultViewModel.sourceCheckNit = Utilities.GetConfiguration("ImageCheckIn");

                    this.consultViewModel.sourceCheckName = Utilities.GetConfiguration("ImageCheckOut");

                    this.consultViewModel.message = "Ingrese NIT/Cédula sin dígito de verificación";
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("chkIdentification_StylusDown", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void chkName_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                if (consultViewModel.typeSearch != 1)
                {
                    RestoreView();

                    this.consultViewModel.typeSearch = 1;

                    this.TxtIdentificacion.Visibility = Visibility.Hidden;

                    this.TxtName.Visibility = Visibility.Visible;

                    this.TxtIdentificacion.Text = "";

                    this.TxtName.Text = "";

                    this.consultViewModel.sourceCheckName = Utilities.GetConfiguration("ImageCheckIn");

                    this.consultViewModel.sourceCheckNit = Utilities.GetConfiguration("ImageCheckOut");

                    this.consultViewModel.message = "Ingrese un nombre valido";
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("chkName_StylusDown", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void Image_StylusDown(object sender, StylusDownEventArgs e)
        {
            Init(false);
        }

        private void BtnNext_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                if (currentPageIndex < totalPage - 1)
                {
                    currentPageIndex++;
                    consultViewModel.viewList.View.Refresh();
                }
                if (currentPageIndex == totalPage - 1)
                {
                    btnNext.Visibility = Visibility.Hidden;
                }

                btnPrev.Visibility = Visibility.Visible;
                ShowCurrentPageIndex();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("BtnNext_StylusDown", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void BtnPrev_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                if (currentPageIndex > 0)
                {
                    currentPageIndex--;
                    consultViewModel.viewList.View.Refresh();
                }

                if (currentPageIndex == 0)
                {
                    btnPrev.Visibility = Visibility.Hidden;
                }

                btnNext.Visibility = Visibility.Visible;
                ShowCurrentPageIndex();
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("BtnPrev_StylusDown", "ConsultWindow", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void lv_Files_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadInfoDetails((Coincidence)lv_Files.SelectedItem);
        }
        #endregion

        #region HeaderButtons
        private void BtnExit_StylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            Utilities.GoToInicial();
        }
        #endregion
    }
}