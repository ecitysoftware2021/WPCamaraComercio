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

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FrmCoincidence.xaml
    /// </summary>
    public partial class FrmCoincidence : Window
    {
        int currentPageIndex = 0;
        int itemPerPage = 20;
        int totalPage = 0;
        CollectionViewSource view;
        ObservableCollection<Coincidence> lstPager;
        List<Grid> lstGrids;
        List<Coincidence> coincidences;
        NavigationService navigationService;
        Utilities utilities;

        public FrmCoincidence()
        {
            InitializeComponent();
            TxbTitle.Text = "Coincidencias para '" + Utilities.search + "'";
            utilities = new Utilities();
            navigationService = new NavigationService(this);
            view = new CollectionViewSource();
        }

        private void Coincidence()
        {
            int i = 1;
            foreach (var item in Utilities.Result)
            {
                lstPager.Add(new Coincidence
                {
                    BusinessName = item.nombre,
                    Nit = item.nit,
                    Municipality = item.municipio.Split(')')[1],
                    EstabliCoincide = item.EstablecimientosConCoincidencia,
                    State = item.estado
                });

                i++;
            }

            CreatePages(i);
        }

        private void ListViewItem_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Coincidence coincidence = (Coincidence)lvCredits.SelectedItem;
            Redirect(coincidence);
            //var coinci = (Coincidence)(sender as ListViewItem).Content;
            //var payValidate = coincidences.Where(c => c.Nit == coinci.Nit).FirstOrDefault();
            //if (payValidate == null)
            //{
            //    total += coinci.ValorPagar;
            //    pay.ImageSource = ChangePath(true);
            //    payments.Add(pay);
            //}
            //else
            //{
            //    total -= coinci.ValorPagar;
            //    pay.ImageSource = ChangePath(false);
            //    payments.Remove(pay);
            //}

            //lblAmount.Text = string.Format("{0:C0}", total);
            //lvCredits.Items.Refresh();
        }

        private void Redirect(Coincidence coincidence)
        {
            if (coincidence == null)
            {
                //Modal alerta = new Modal("Debe de seleccionar una tienda.");
                //alerta.ShowDialog();
            }
            else
            {
                //utilities.saveLogTransaction(file.matricula, "Se inicia busqueda de " + file.nombre, "");
                //Utilities.identificacion = file.matricula;

                //if (Utilities.RespuestaConsulta.response.resultados.Count() == 1)
                //{
                    Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados.Where(r => r.nit == coincidence.Nit);
                //}
                //else
                //{
                    //Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados.Where(r => r.nit == Utilities.search);

                    //if (searchType == tipo_busqueda.Nombre && Utilities.ConsultResult.Count() < 1)
                    //{
                    //    Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados.Where(r => r.nombre == Utilities.search);
                    //}
                //}
                foreach (var item in Utilities.ConsultResult)
                {
                    //utilities.Matricula = item.matricula;
                    //utilities.Tpcm = item.tpcm;
                }
            }
            Utilities.ResetTimer();
            navigationService.NavigationTo("FrmInformationCompany");
        }

        #region Pagination
        private void CreatePages(int i)
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
                //btnNext.Visibility = Visibility.Hidden;
                //btnPrev.Visibility = Visibility.Hidden;
            }

            view.Source = lstPager;
            view.Filter += new FilterEventHandler(View_Filter);
            lvCredits.DataContext = view;
            ShowCurrentPageIndex();
            tbTotalPage.Text = totalPage.ToString();
        }

        private void ShowCurrentPageIndex()
        {
            tbCurrentPage.Text = (currentPageIndex + 1).ToString();
        }

        private void View_Filter(object sender, FilterEventArgs e)
        {
            int index = lstPager.IndexOf((Coincidence)e.Item);

            if (index >= itemPerPage * currentPageIndex && index < itemPerPage * (currentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void btnNext_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPageIndex < totalPage - 1)
            {
                currentPageIndex++;
                view.View.Refresh();
            }
            if (currentPageIndex == totalPage - 1)
            {
                //btnNext.Visibility = Visibility.Hidden;
            }

            //btnPrev.Visibility = Visibility.Visible;
        }

        private void btnPrev_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                view.View.Refresh();
            }

            if (currentPageIndex == 0)
            {
               // btnPrev.Visibility = Visibility.Hidden;
            }

            //btnNext.Visibility = Visibility.Visible;
        }
        #endregion

        #region HeaderButons
        private void BtnExit_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            navigationService.NavigationTo("FrmInitial");
        }

        private void BtnBack_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            navigationService.NavigationTo("FrmConsult");
        }
        #endregion
    }
}
