using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;
using WPCamaraComercio.WCFCamaraComercio;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for LoadingModal.xaml
    /// </summary>
    public partial class LoadingModal : Window
    {
        #region Reference
        private Coincidence coincidence;
        private WCFServices services;
        private Utilities utilities;
        NavigationService navigationService;
        string message = string.Concat("Lo sentimos, ",
                               Environment.NewLine,
                               "En este momento el servicio no se encuentra disponible.");
        #endregion

        #region LoadMethods
        public LoadingModal(Coincidence coincidence)
        {
            InitializeComponent();
            this.coincidence = coincidence;
            this.services = new WCFServices();
            navigationService = new NavigationService(this);

            ConsultDetail();
        } 
        #endregion

        #region Methods
        private async void ConsultDetail()
        {
            try
            {
                Utilities.Enrollment = coincidence.Enrollment;
                Utilities.Tpcm = coincidence.Tpcm;

                var task = services.ConsultDetailMerchant(coincidence.Enrollment, coincidence.Tpcm);
                if (await Task.WhenAny(task, Task.Delay(40000)) == task)
                {
                    var response = task.Result;
                    if (response.IsSuccess)
                    {
                        Utilities.DetailResponse = (RespuestaDetalle)response.Result;

                        if (Utilities.DetailResponse.response.resultados != null)
                        {
                            var datos = Utilities.DetailResponse.response.resultados[0];

                            if (datos.certificados != null)
                            {
                                ChangeView(1);
                            }
                            else
                            {
                                navigationService.NavigatorModal("En el momento no hay certificados para generar, vuelva pronto.");
                                ChangeView(2);
                            }
                        }
                        else
                        {
                            FrmModal modal = new FrmModal(message,null);
                            modal.ShowDialog();
                            this.Close();
                        }
                    }
                }
                else
                {
                    FrmModal modal = new FrmModal(message,null);
                    modal.ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                //utilities.saveLogError("RedirecView", "RecordsWindows", ex.ToString());
            }
        }

        private void ChangeView(int type)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (type == 1)
                    {
                        this.LblMessage.Text = string.Concat("Ha seleccionado, ", coincidence.BusinessName, " Desea continuar");

                        this.GifLoadder.Visibility = Visibility.Hidden;

                        this.BtnCancel.Visibility = Visibility.Visible;
                        this.BtnContinue.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.LblMessage.Text = string.Concat("Cancelar busqueda de certificados de ", coincidence.BusinessName);

                        this.GifLoadder.Visibility = Visibility.Hidden;

                        this.BtnCancel.HorizontalAlignment = HorizontalAlignment.Center;
                        this.BtnCancel.Visibility = Visibility.Visible;


                        this.BtnContinue.Visibility = Visibility.Hidden;
                    }
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region Events
        private void BtnContinue_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.DialogResult = false;
        } 
        #endregion
    }
}
