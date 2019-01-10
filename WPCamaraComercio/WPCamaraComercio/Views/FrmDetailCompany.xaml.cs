using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        #region Reference
        CollectionViewSource view;
        ObservableCollection<DetailMerchant> lstDetailMerchant;
        ObservableCollection<DetailEstablish> lstDetailEstablish;
        decimal total = 0;
        string message = string.Empty;
        NavigationService navigationService;
        WCFServices services;
        List<SelectedDetail> selectedDetail;
        List<Certificado> ListCertificates;
        FrmLoading frmLoading;
        decimal valueToPay = 0;
        Utilities utilities; 
        #endregion

        public FrmDetailCompany()
        {
            InitializeComponent();
            services = new WCFServices();
            lstDetailMerchant = new ObservableCollection<Models.DetailMerchant>();
            lstDetailEstablish = new ObservableCollection<DetailEstablish>();
            view = new CollectionViewSource();
            navigationService = new NavigationService(this);
            selectedDetail = new List<SelectedDetail>();
            ListCertificates = new List<Certificado>();
            frmLoading = new FrmLoading();
            utilities = new Utilities();
            GrdEstablish.Visibility = Visibility.Hidden;
            //var task = services.ConsultInformation("811040812", tipo_busqueda.Nit);
            //tipo = 1;
            //var response = task.Result;
            //Utilities.RespuestaConsulta = (RespuestaConsulta)response.Result;
            //matricula = Utilities.RespuestaConsulta.response.resultados[0].matricula;
            //tpcm = Utilities.RespuestaConsulta.response.resultados[0].tpcm;
            //ConsultInformation();

            AssingProperties();
        }

        private void AssingProperties()
        {
            try
            {
                if (Utilities.DetailResponse.response.resultados != null)
                {
                    Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados[0];
                    Utilities.ListMerchantDetail.Add(FillMerchantDetail(Utilities.DetailResponse.response.resultados[0]));

                    GenerateMerchant();
                    GenerateEstablish();
                }
                else
                {
                    navigationService.NavigatorModal("No hay datos para mostrar en estos momentos.");
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

                LvEstablish.DataContext = lstDetailEstablish;
            }
            catch (Exception ex)
            {
                //navigationService.NavigationTo(ex.Message);
            }
        }

        private MerchantDetail FillMerchantDetail(ResultadoDetalle data)
        {
            try
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
            catch (Exception ex)
            {
                return null;
            }
            
        }

        private void Details(Details details)
        {
            FrmModalDetailEstablish FrmModalDetailEstablish = new FrmModalDetailEstablish(details);
            FrmModalDetailEstablish.ShowDialog();
        }

        private void BtnComerciant_StylusDown(object sender, StylusDownEventArgs e)
        {
            GrdEstablish.Visibility = Visibility.Hidden;
            GrdMerchant.Visibility = Visibility.Visible;
            BtnComerciant.Opacity = 1;
            BtnEstablish.Opacity = 0.4;
        }

        private void BtnEstablish_StylusDown(object sender, StylusDownEventArgs e)
        {
            GrdEstablish.Visibility = Visibility.Visible;
            GrdMerchant.Visibility = Visibility.Hidden;
            BtnComerciant.Opacity = 0.4;
            BtnEstablish.Opacity = 1;
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
            try
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
                valueToPay = total;
            }
            catch (Exception ex)
            {

            }
        }

        private void TxbData2_StylusDown(object sender, StylusDownEventArgs e)
        {
            FrmModalDetailMerchant frmModalDetail = new FrmModalDetailMerchant(Utilities.ListMerchantDetail[0]);
            frmModalDetail.ShowDialog();
        }

        private void BtnAcept_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                string lblPay = lblAmount.Text.Replace("$", "").Replace(".00", "").Replace(",00", "").Trim();
                if (lblPay == "0" || valueToPay == 0)
                {
                    //objUtil.OpenModal("Debe seleccionar un certificado");
                }
                else
                {
                    foreach (var item in selectedDetail)
                    {
                        Certificado certificate = new Certificado();
                        certificate.NumeroCertificados = item.Quantity.ToString();
                        certificate.IdCertificado = item.EstablishCertificate.CertificateId;
                        certificate.matricula = Utilities.Enrollment;
                        certificate.MatriculaEst = item.EstablishCertificate.EstablishEnrollment;
                        certificate.tpcm = Utilities.Tpcm;
                        certificate.CodigoGeneracion = item.EstablishCertificate.GenerationCode;
                        ListCertificates.Add(certificate);
                    }
                    Utilities.ListCertificates = ListCertificates;
                    Utilities.ValueToPay = valueToPay;

                    Utilities.ResetTimer();
                    navigationService.NavigationTo("FrmPaymentData");
                }
            }
            catch (Exception ex)
            {
                //objUtil.Exception(ex.Message);
            }
        }

        private void BtnExit_StylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                Utilities.GoToInicial();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnHome_MouseDown", "FrmMenu", ex.ToString());
            }
        }

        private void BtnBack_StylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            navigationService.NavigationTo("ConsultWindow");
        }

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }
    }
}
