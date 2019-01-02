using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;
using WPCamaraComercio.ViewModels;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para ConsultWindow.xaml
    /// </summary>
    public partial class ConsultWindow : Window
    {
        private int currentPageIndex = 0;

        private int itemPerPage = 10;

        private int totalPage = 0;

        private object OpcionList;

        private Utilities utilities;

        private ConsultViewModel consultViewModel;

        public ConsultWindow()
        {
            
            InitializeComponent();
            utilities = new Utilities();
            Init(true);
        }

        private void Init(bool initData)
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
                    }
                    else
                    {
                        this.consultViewModel.headers = Visibility.Visible;
                        InitViewList();
                    }
                };
            } 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // Utilities.Timer(tbTimer);
        }

        #region "HeadersButtons"
        private void BtnExit_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.GoToInicial();
                    Utilities.ResetTimer();
                    MainWindow inicio = new MainWindow();
                    inicio.Show();
                    this.Close();
                });
                GC.Collect();

            }
            catch (Exception ex)
            {
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        #endregion

        private void InitViewList()
        {
            try
            {
                CreatePages(consultViewModel.countConcidences);
            }
            catch (Exception ex)
            {
                utilities.saveLogError("InitView", "RecordsWindows", ex.ToString());
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
                if (totalPage == 1)
                {
                    btnNext.Visibility = Visibility.Hidden;
                    btnPrev.Visibility = Visibility.Hidden;
                }

                consultViewModel.viewList.Filter += new FilterEventHandler(View_Filter);
                lv_Files.DataContext = consultViewModel.viewList;
                //ShowCurrentPageIndex();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("CreatePages", "RecordsWindows", ex.ToString());
            }
        }

        private void RedirectView(Coincidence coincidence)
        {
            try
            {

            }
            catch (Exception ex)
            {
                utilities.saveLogError("RedirecView", "RecordsWindows", ex.ToString());
            }
        }

        private void View_Filter(object sender, FilterEventArgs e)
        {
            try
            {
                int index = consultViewModel.coincidences.IndexOf((Coincidence) e.Item);

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
                utilities.saveLogError("ViewFilter", "RecordsWindows", ex.ToString());
            }
        }

        private void lv_Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Coincidence coincidence = (Coincidence)lv_Files.SelectedItem;
                RedirectView(coincidence);
            }
            catch (Exception ex)
            {
                utilities.saveLogError("lv_Files_SelectionChanged", "RecordsWindows", ex.ToString());
            }
        }

        private void btnNext_PreviewStylusDown(object sender, StylusDownEventArgs e)
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

        private void ShowCurrentPageIndex()
        {
            tbCurrentPage.Text = (currentPageIndex + 1).ToString();
        }

        private void btnPrev_PreviewStylusDown(object sender, StylusDownEventArgs e)
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

        private void BtnConsultar_StylusDown(object sender, StylusDownEventArgs e)
        {
            ConsulParameter();
        }

        private void ConsulParameter()
        {
            try
            {
                string valueSearch = "";

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
            }
        }

        private void chkIdentification_StylusDown(object sender, StylusDownEventArgs e)
        {
            if (consultViewModel.typeSearch != 2)
            {
                this.consultViewModel.typeSearch = 2;

                this.TxtIdentificacion.Text = "";

                this.TxtName.Text = "";

                this.TxtIdentificacion.Visibility = Visibility.Visible;

                this.TxtName.Visibility = Visibility.Hidden;

                this.consultViewModel.sourceCheckNit = Utilities.GetConfiguration("ImageCheckIn");

                this.consultViewModel.sourceCheckName = Utilities.GetConfiguration("ImageCheckOut");

                this.consultViewModel.message = "Ingrese NIT/Cédula sin dígito de verificación";

            }
        }

        private void chkName_StylusDown(object sender, StylusDownEventArgs e)
        {
            if (consultViewModel.typeSearch != 1)
            {
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

        private void Image_StylusDown(object sender, StylusDownEventArgs e)
        {
            Init(false);
        }
    }
}
