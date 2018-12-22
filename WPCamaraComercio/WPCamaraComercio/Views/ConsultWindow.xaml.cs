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

        private CollectionViewSource view;

        private ObservableCollection<Coincidence> lstPager;

        private object OpcionList;

        private Utilities utilities;

        private ConsultViewModel consultViewModel;

        public ConsultWindow()
        {
            utilities = new Utilities();
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }

        private void InitListCoincidences(List<Coincidence> coincidences)
        {
            this.view = new CollectionViewSource();
            this.lstPager = new ObservableCollection<Coincidence>();
            OpcionList = null;
            InitView();
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

        private void BtnAtras_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.ResetTimer();
                    MainWindow main = new MainWindow();
                    main.Show();
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

        private void InitView()
        {
            try
            {
                CreatePages(consultViewModel.CountConcidences());
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

                view.Source = lstPager;
                view.Filter += new FilterEventHandler(View_Filter);
                lv_Files.DataContext = view;
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
                int index = lstPager.IndexOf((File)e.Item);

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
                File file = (File)lv_Files.SelectedItem;
                RedirectView(file);
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
                view.View.Refresh();
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
                view.View.Refresh();
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

        }
    }
}
