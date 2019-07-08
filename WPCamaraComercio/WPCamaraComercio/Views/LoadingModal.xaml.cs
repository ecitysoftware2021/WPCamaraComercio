using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for LoadingModal.xaml
    /// </summary>
    public partial class LoadingModal : Window
    {
        #region Reference
        private Coincidence coincidence;
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
            navigationService = new NavigationService(this);

            ConsultDetail();
        }
        #endregion

        #region Methods
        private async void ConsultDetail()
        {
            try
            {

                Api api = new Api();
                PeticionDetalle peticion = new Classes.PeticionDetalle
                {
                    Matricula = coincidence.Enrollment,
                    Tpcm = coincidence.Tpcm
                };



                var task = api.GetData(new RequestApi
                {
                    Data = peticion
                }, "GetDetalle");

                if (await Task.WhenAny(task, Task.Delay(30000)) == task)
                {
                    var response = task.Result;
                    if (response.CodeError == 200)
                    {
                        Utilities.DetailResponse = JsonConvert.DeserializeObject<ResponseDetalleComerciante>(response.Data.ToString());

                        if (Utilities.DetailResponse.Result.response.resultados != null)
                        {
                            var datos = Utilities.DetailResponse.Result.response.resultados[0];

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
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                FrmModal modal = new FrmModal(message);
                                modal.ShowDialog();
                            }));

                        }
                    }
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        FrmModal modal = new FrmModal(message);
                        modal.ShowDialog();

                    }));
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
                        this.LblMessage.Text = string.Concat("Ha seleccionado ", coincidence.BusinessName, ", ¿desea continuar?");

                        this.GifLoadder.Visibility = Visibility.Hidden;

                        this.BtnCancel.Visibility = Visibility.Visible;
                        this.BtnContinue.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.LblMessage.Text = string.Concat("Cancelar búsqueda de certificados de ", coincidence.BusinessName);

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
