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
using WPCamaraComercio.Objects;
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
        WCFServices services;
        List<ComboData> ListData;
        List<SelectedDetail> selectedDetail;
        FrmLoading frmLoading;

        public FrmDetailCompany()
        {
            InitializeComponent();
            services = new WCFServices();
            lstDetailMerchant = new ObservableCollection<Models.DetailMerchant>();
            lstDetailEstablish = new ObservableCollection<DetailEstablish>();
            view = new CollectionViewSource();
            ListData = new List<ComboData>();
            selectedDetail = new List<SelectedDetail>();
            frmLoading = new FrmLoading();


            ListData.Add(new ComboData { Id = 1, Value = "Uno" });
            ListData.Add(new ComboData { Id = 2, Value = "Dos" });
            ListData.Add(new ComboData { Id = 3, Value = "Tres" });
            ListData.Add(new ComboData { Id = 4, Value = "Cuatro" });
            ListData.Add(new ComboData { Id = 5, Value = "Cinco" });

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
                    Utilities.Loading(frmLoading, false, this);
                }
                else
                {
                    GenerateEstablish();
                    Utilities.Loading(frmLoading, false, this);
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
                    TxbData1.Text = item.come_Nom;
                    if (item.certificados != null)
                    {
                        foreach (var item2 in item.certificados)
                        {
                            EstablishCertificate objEstablishCertificate = new EstablishCertificate();
                            objEstablishCertificate.CertificateCost = decimal.Parse(item2.ValorCertificado);
                            objEstablishCertificate.CertificateId = item2.IdCertificado;
                            objEstablishCertificate.EstablishEnrollment = item2.MatriculaEstablecimiento;
                            objEstablishCertificate.GenerationCode = item2.CodigoGeneracion;

                            lstDetailMerchant.Add(new DetailMerchant
                            {
                                CertificateName = item2.NombreCertificado,
                                Amount = Convert.ToDecimal(item2.ValorCertificado),
                                EstablishCertificate = objEstablishCertificate,
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
                foreach (var item in Utilities.DetailResponse.response.resultados)
                {
                    TxbData1.Text = item.come_Nom;
                    if (item.establecimientos != null)
                    {
                        foreach (var item2 in item.establecimientos)
                        {
                            Details objDetail = new Details();
                            objDetail.dire = item2.DireccionEstablecimiento;
                            objDetail.nombreest = item2.NombreEstablecimiento;
                            objDetail.mat = item2.MatriculaEst;
                            objDetail.estado = item2.EstadoEstablecimiento;

                            foreach (var item3 in item2.CertificadosEstablecimiento)
                            {
                                EstablishCertificate objEstablishCertificate = new EstablishCertificate();
                                objEstablishCertificate.CertificateCost = decimal.Parse(item3.ValorCertificado);
                                objEstablishCertificate.CertificateId = item3.IdCertificado;
                                objEstablishCertificate.EstablishEnrollment = item3.MatriculaEstablecimiento;
                                objEstablishCertificate.GenerationCode = item3.CodigoGeneracion;

                                lstDetailEstablish.Add(new DetailEstablish
                                {
                                    Establish = item2.NombreEstablecimiento,
                                    Amount = item3.ValorCertificado,
                                    Details = objDetail,
                                    Certificate = item3.NombreCertificado,
                                    EstablishCertificate = objEstablishCertificate
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
            Utilities.Loading(frmLoading, true, this);
            tipo = 1;
            BtnComerciant.Opacity = 1;
            BtnEstablish.Opacity = 0.4;
            AssingProperties();
        }

        private void BtnEstablish_StylusDown(object sender, StylusDownEventArgs e)
        {
            lstDetailEstablish.Clear();
            lstDetailMerchant.Clear();
            Utilities.Loading(frmLoading, true, this);
            tipo = 2;
            BtnComerciant.Opacity = 0.4;
            BtnEstablish.Opacity = 1;
            AssingProperties();
        }

        private void TextBlock_StylusDown(object sender, StylusDownEventArgs e)
        {
            TextBlock text = (TextBlock)sender;
            Details dt = (Details)text.Tag;
            Details(dt);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            // ... Set Window Title to Expander Header value.
            var expander = sender as Expander;
            this.Title = expander.Header.ToString();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            // ... Change Window Title.
            var expander = sender as Expander;
            this.Title = expander.Header.ToString();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            EstablishCertificate establishCertificate = (EstablishCertificate)combo.Tag;
            int quantity = 0;
            if (!int.TryParse(combo.SelectedValue.ToString(), out quantity))
            {
                quantity = 0;
            }

            var stablish = selectedDetail.Where(d => d.EstablishCertificate.EstablishEnrollment == establishCertificate.EstablishEnrollment).FirstOrDefault();
            if (stablish == null)
            {
                if (quantity != 0)
                {
                    selectedDetail.Add(new SelectedDetail
                    {
                        EstablishCertificate = establishCertificate,
                        Quantity = quantity
                    });

                    total += quantity * establishCertificate.CertificateCost;
                }
            }
            else
            {
                if (quantity != 0)
                {
                    total -= stablish.Quantity * stablish.EstablishCertificate.CertificateCost;
                    stablish.Quantity = quantity;
                    total += quantity * establishCertificate.CertificateCost;
                }
                else
                {
                    selectedDetail.Remove(stablish);
                    total -= stablish.Quantity * stablish.EstablishCertificate.CertificateCost;
                }
            }

            lblAmount.Text = string.Format("{0:C0}", total);
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
