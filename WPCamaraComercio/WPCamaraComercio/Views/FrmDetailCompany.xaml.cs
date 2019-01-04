﻿using System;
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
            selectedDetail = new List<SelectedDetail>();
            frmLoading = new FrmLoading();
          GrdEstablish.Visibility = Visibility.Hidden;
            var task = services.ConsultInformation("811040812", tipo_busqueda.Nit);
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
                if (Utilities.DetailResponse.response.resultados != null)
                {
                    Utilities.ListMerchantDetail.Add(FillMerchantDetail(Utilities.DetailResponse.response.resultados[0]));

                    //if (tipo == 1)
                    //{
                        GenerateMerchant();
                        //Utilities.Loading(frmLoading, false, this);
                    //}
                    //else
                    //{
                        GenerateEstablish();
                        //Utilities.Loading(frmLoading, false, this);
                    //}
                }
                else
                {
                        //modal error
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
                var datos = Utilities.DetailResponse.response.resultados[0];
                if (datos.certificados != null)
                {
                    foreach (var item in datos.certificados)
                    {
                        EstablishCertificate objEstablishCertificate = new EstablishCertificate();
                        objEstablishCertificate.CertificateCost = decimal.Parse(item.ValorCertificado);
                        objEstablishCertificate.CertificateId = item.IdCertificado;
                        objEstablishCertificate.EstablishEnrollment = item.MatriculaEstablecimiento;
                        objEstablishCertificate.GenerationCode = item.CodigoGeneracion;

                        lstDetailMerchant.Add(new DetailMerchant
                        {
                            CertificateName = item.NombreCertificado,
                            Amount = Convert.ToDecimal(item.ValorCertificado),
                            EstablishCertificate = objEstablishCertificate,
                        });
                    }
                }

                //GrdEstablish.Visibility = Visibility.Hidden;
                //GrdMerchant.Visibility = Visibility.Visible;

                int i = lstDetailMerchant.Count();
                // CreatePages(i, lstDetailMerchant);
                LvMerchant.DataContext = lstDetailMerchant;
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
                var datos = Utilities.DetailResponse.response.resultados[0];
                if (datos.establecimientos != null)
                {
                    foreach (var item in datos.establecimientos)
                    {
                        Details objDetail = new Details();
                        objDetail.dire = item.DireccionEstablecimiento;
                        objDetail.nombreest = item.NombreEstablecimiento;
                        objDetail.mat = item.MatriculaEst;
                        objDetail.estado = item.EstadoEstablecimiento;

                        foreach (var item2 in item.CertificadosEstablecimiento)
                        {
                            EstablishCertificate objEstablishCertificate = new EstablishCertificate();
                            objEstablishCertificate.CertificateCost = decimal.Parse(item2.ValorCertificado);
                            objEstablishCertificate.CertificateId = item2.IdCertificado;
                            objEstablishCertificate.EstablishEnrollment = item2.MatriculaEstablecimiento;
                            objEstablishCertificate.GenerationCode = item2.CodigoGeneracion;

                            lstDetailEstablish.Add(new DetailEstablish
                            {
                                Establish = item.NombreEstablecimiento,
                                Amount = item2.ValorCertificado,
                                Details = objDetail,
                                Certificate = item2.NombreCertificado,
                                EstablishCertificate = objEstablishCertificate
                            });
                        }
                    }
                }

                //GrdEstablish.Visibility = Visibility.Visible;
                //GrdMerchant.Visibility = Visibility.Hidden;

                int i = lstDetailEstablish.Count();
               // CreatePages(i, lstDetailEstablish);
                LvEstablish.DataContext = lstDetailEstablish;
            }
            catch (Exception ex)
            {
                //navigationService.NavigationTo(ex.Message);
            }
        }

        private MerchantDetail FillMerchantDetail(ResultadoDetalle data)
        {
            TxbData1.Text = data.come_Nom;
            MerchantDetail objMerchantDetail = new MerchantDetail();
            objMerchantDetail.rSocial = data.come_Nom;
            objMerchantDetail.sigla = data.come_sigla;
            objMerchantDetail.ident = data.identificacion;
            objMerchantDetail.tSociedad = data.Tpcm_Desc;
            objMerchantDetail.dComercial = data.dir_come;
            objMerchantDetail.mun = data.Mpio_Come_Nom;
            objMerchantDetail.estado = data.Activo;
            objMerchantDetail.nEstablecimientos = data.numeroestablecimientosactivos;
            objMerchantDetail.fInicio = data.Fec_Inicio;
            objMerchantDetail.uRenovacion = data.UltRenv;
            return objMerchantDetail;
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
            //lstDetailEstablish.Clear();
            //lstDetailMerchant.Clear();
            //Utilities.Loading(frmLoading, true, this);
            //tipo = 1;

            GrdEstablish.Visibility = Visibility.Hidden;
            GrdMerchant.Visibility = Visibility.Visible;
            BtnComerciant.Opacity = 1;
            BtnEstablish.Opacity = 0.4;
            //AssingProperties();
        }

        private void BtnEstablish_StylusDown(object sender, StylusDownEventArgs e)
        {
            //lstDetailEstablish.Clear();
            //lstDetailMerchant.Clear();
            //Utilities.Loading(frmLoading, true, this);
            //tipo = 2;
            GrdEstablish.Visibility = Visibility.Visible;
            GrdMerchant.Visibility = Visibility.Hidden;
            BtnComerciant.Opacity = 0.4;
            BtnEstablish.Opacity = 1;
            
            //AssingProperties();
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

        private void TxbData2_StylusDown(object sender, StylusDownEventArgs e)
        {
            MessageBox.Show(Utilities.ListMerchantDetail[0].nEstablecimientos);
        }
    }
}
