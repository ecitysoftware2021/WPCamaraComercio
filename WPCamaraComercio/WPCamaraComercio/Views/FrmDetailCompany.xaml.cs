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
using WPCamaraComercio.WCFCamaraComercio;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FrmDetailCompany.xaml
    /// </summary>
    public partial class FrmDetailCompany : Window
    {
        CollectionViewSource view;
        ObservableCollection<DetailMerchant> lstDetailMerchant;
        public string prueba = "hola";
        ObservableCollection<DetailEstablish> lstDetailEstablish;
        int currentPageIndex = 0;
        int itemPerPage = 10;
        int totalPage = 0;
        decimal total = 0;
        bool isSelectAll = false;
        string message = string.Empty;
        int tipo;
        private string matricula;
        private string tpcm;
        WCFServices services = new WCFServices();
        public FrmDetailCompany()
        {
            InitializeComponent();
            lstDetailMerchant = new ObservableCollection<Models.DetailMerchant>();
            lstDetailEstablish = new ObservableCollection<DetailEstablish>();
            view = new CollectionViewSource();
            var task = services.ConsultInformation("890900608", tipo_busqueda.Nit);
            tipo = 1;
            var response = task.Result;

            Utilities.RespuestaConsulta = (RespuestaConsulta)response.Result;
            matricula = Utilities.RespuestaConsulta.response.resultados[0].matricula;
            tpcm = Utilities.RespuestaConsulta.response.resultados[0].tpcm;
            ConsultInformation();

            AssingProperties();
            //ComboBox ddlCantidad = new ComboBox();
            //ddlCantidad.Items.Insert(0, "Seleccionar");
            //for (int i = 1; i <= 5; i++)
            //{
            //    ddlCantidad.Items.Insert(i, i);
            //}
        }

        private void ConsultInformation()
        {
            PeticionDetalle petition = new PeticionDetalle();
            petition.Matricula = matricula;
            petition.Tpcm = tpcm;

            var task = services.ConsultDetailMerchant(petition);
            //Utilities.Loading(frmLoading, true, this);

            var response = task.Result;
            if (response.IsSuccess)
            {
                //Utilities.Loading(frmLoading, false, this);
                Utilities.DetailResponse = (RespuestaDetalle)response.Result;
            }
        }

        private void AssingProperties()
        {
            try
            {
                if (tipo == 1)
                {
                    GenerateMerchant();
                }
                else
                {
                    GenerateEstablish();
                }
            }
            catch (Exception ex)
            {
                //navigationService.NavigationTo(ex.Message);
            }
        }

        private void GenerateMerchant()
        {
            try
            {
                foreach (var item in Utilities.DetailResponse.response.resultados)
                {
                    if (item.certificados != null)
                    {
                        foreach (var item2 in item.certificados)
                        {
                            lstDetailMerchant.Add(new DetailMerchant
                            {
                                CertificateName = item2.NombreCertificado,
                                Amount = item2.ValorCertificado
                            });
                        }
                    }
                }

                GrdEstablish.Visibility = Visibility.Hidden;
                GrdMerchant.Visibility = Visibility.Visible;
                int i = lstDetailMerchant.Count();
                CreatePages(i, lstDetailMerchant);
                LvMerchant.DataContext = view;
            }
            catch (Exception ex)
            {
                //navigationService.NavigationTo(ex.Message);
            }
        }

        private void GenerateEstablish()
        {
            try
            {
                int column = 1;
                int row = 1;
                foreach (var item in Utilities.DetailResponse.response.resultados)
                {
                    if (item.establecimientos != null)
                    {
                        foreach (var item2 in item.establecimientos)
                        {
                            Details objDetail = new Details();
                            objDetail.dire = item2.DireccionEstablecimiento;
                            objDetail.nombreest = item2.NombreEstablecimiento;
                            objDetail.mat = item2.MatriculaEst;
                            objDetail.estado = item2.EstadoEstablecimiento;

                            //RowDefinition rowDefinition = new RowDefinition();
                            //rowDefinition.Height = new GridLength(100);
                            ////GrdEstablish.RowDefinitions.Add(rowDefinition);
                                                        //Grid.SetColumn(label, column);
                            //Grid.SetRow(label, row);
                            //LvEstablish.Items.Add(label);
                            //row += 1;

                            foreach (var item3 in item2.CertificadosEstablecimiento)
                            {


                                lstDetailEstablish.Add(new DetailEstablish
                                {
                                    Establish = item2.NombreEstablecimiento,
                                    Amount = item3.ValorCertificado,
                                    details = objDetail
                                });
                            }
                        }
                    }
                }

                GrdEstablish.Visibility = Visibility.Visible;
                GrdMerchant.Visibility = Visibility.Hidden;

                int i = lstDetailEstablish.Count();
                CreatePages(i, lstDetailEstablish);
                LvEstablish.DataContext = view;
            }
            catch (Exception ex)
            {
                //navigationService.NavigationTo(ex.Message);
            }
        }

        private void DetailMerchant()
        {

        }

        private void Details(Details details)
        {
            MessageBox.Show(details.nombreest);
            //frmDetalles objForm = new frmDetalles(details);
            //objForm.ShowDialog();
        }

        #region Pagination

        private void CreatePages<T>(int i, T lstPager)
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
            tbTotalPage.Text = totalPage.ToString();
        }

        private void ShowCurrentPageIndex()
        {
            tbCurrentPage.Text = (currentPageIndex + 1).ToString();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                view.View.Refresh();
            }

            if (currentPageIndex == 0)
            {
                //btnPrev.Visibility = Visibility.Hidden;
            }

            //btnNext.Visibility = Visibility.Visible;

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
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

        #endregion

        private void BtnComerciant_StylusDown(object sender, StylusDownEventArgs e)
        {
            lstDetailEstablish.Clear();
            lstDetailMerchant.Clear();
            tipo = 1;
            AssingProperties();
        }

        private void BtnEstablish_StylusDown(object sender, StylusDownEventArgs e)
        {
            lstDetailEstablish.Clear();
            lstDetailMerchant.Clear();
            tipo = 2;
            AssingProperties();
        }

        private void TextBlock_StylusDown(object sender, StylusDownEventArgs e)
        {
            TextBlock text = (TextBlock)sender;
            Details dt = (Details)text.Tag;
            Details(dt);
        }
    }
}
