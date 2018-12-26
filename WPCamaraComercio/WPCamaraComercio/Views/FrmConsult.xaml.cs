using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WPCamaraComercio.Keyboard;
using WPCamaraComercio.Service;
using WPCamaraComercio.WCFCamaraComercio;
//
namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmConsult.xaml
    /// </summary>
    public partial class FrmConsult : Window
    {
        #region References

        Utilities utilities;
        TouchScreenKeyNumeric numericKey;
        WCFServices services;
        FrmLoading frmLoading;
        tipo_busqueda searchType;
        string searchString;
        NavigationService navigateService;

        #endregion

        #region LoadMethods

        public FrmConsult()
        {
            InitializeComponent();
            searchString = TxtIdentificacion.Text;
            searchType = tipo_busqueda.Nit;
            frmLoading = new FrmLoading();
            services = new WCFServices();
            utilities = new Utilities();
            navigateService = new NavigationService(this);
        }

        #endregion

        #region Timer

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }

        #endregion

        #region HeaderButtons

        private void BtnBack_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            Dispatcher.BeginInvoke((Action)delegate
            {
                FrmMenu menu = new FrmMenu();
                menu.Show();
                this.Close();
            });
            GC.Collect();
        }

        private void BtnExit_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.GoToInicial();
        }

        #endregion

        #region Events

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => lblError.Visibility = Visibility.Hidden;

        private void chkIdentification_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            string tag = chkIdentification.Tag.ToString();
            string identificationTag, nameTag, identificationPath, namePath;
            switch (tag)
            {
                case "S":
                    identificationTag = "N";
                    nameTag = "S";
                    identificationPath = "circulo";
                    namePath = "ok";
                    ChangeStateTextbox(TxtIdentificacion);
                    ChangeStateTextbox(TxtName, true);
                    break;
                case "N":
                    identificationTag = "S";
                    nameTag = "N";
                    identificationPath = "ok";
                    namePath = "circulo";
                    ChangeStateTextbox(TxtIdentificacion, true);
                    ChangeStateTextbox(TxtName);
                    break;
                default:
                    identificationTag = "N";
                    nameTag = "S";
                    identificationPath = "circulo";
                    namePath = "ok";
                    ChangeStateTextbox(TxtIdentificacion);
                    ChangeStateTextbox(TxtName, true);
                    break;
            }

            chkIdentification.Tag = identificationTag;
            chkName.Tag = nameTag;
            chkIdentification.Source = Utilities.SetButtonImage("Others", identificationPath, "png");
            chkName.Source = Utilities.SetButtonImage("Others", namePath, "png");
        }

        private void chkName_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            string tag = chkName.Tag.ToString();
            string identificationTag, nameTag, identificationPath, namePath;
            switch (tag)
            {
                case "S":
                    identificationTag = "S";
                    nameTag = "N";
                    identificationPath = "ok";
                    namePath = "circulo";
                    ChangeStateTextbox(TxtIdentificacion, true);
                    ChangeStateTextbox(TxtName);
                    break;
                case "N":
                    identificationTag = "N";
                    nameTag = "S";
                    identificationPath = "circulo";
                    namePath = "ok";
                    ChangeStateTextbox(TxtIdentificacion);
                    ChangeStateTextbox(TxtName, true);
                    break;
                default:
                    identificationTag = "S";
                    nameTag = "N";
                    identificationPath = "ok";
                    namePath = "circulo";
                    ChangeStateTextbox(TxtIdentificacion, true);
                    ChangeStateTextbox(TxtName);
                    break;
            }

            chkIdentification.Tag = identificationTag;
            chkName.Tag = nameTag;
            chkIdentification.Source = Utilities.SetButtonImage("Others", identificationPath, "png");
            chkName.Source = Utilities.SetButtonImage("Others", namePath, "png");
        }

        private void BtnConsultar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            bool state = false;
            try
            {
                if (string.IsNullOrEmpty(TxtIdentificacion.Text) && string.IsNullOrEmpty(TxtName.Text))
                {
                    lblError.Visibility = Visibility.Visible;
                    return;
                }

                state = chkIdentification.Tag.ToString().Equals("S");
                if (!state)
                {
                    searchString = TxtName.Text;
                    searchType = tipo_busqueda.Nombre;
                }

                ConsultInformation();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnConsultar_PreviewMouseDown", "FrmSearch", ex.ToString());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cambia el estado del textbox que se le envía
        /// </summary>
        /// <param name="txt">textbox a ocultar/mostrar</param>
        /// <param name="state">false(default)oculta y true muestra</param>
        private void ChangeStateTextbox(TextBox txt, bool state = false)
        {
            txt.Text = string.Empty;
            if (!state)
            {
                txt.Visibility = Visibility.Hidden;
            }
            else
            {
                txt.Visibility = Visibility.Visible;
            }
        }

        private async void ConsultInformation()
        {
            var task = services.ConsultInformation(searchString, 1);
            Utilities.Loading(frmLoading, true, this);
            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                var response = task.Result;
                if (response.IsSuccess)
                {
                    Utilities.Loading(frmLoading, false, this);
                    Utilities.RespuestaConsulta = (RespuestaConsulta)response.Result;
                    if (Utilities.RespuestaConsulta.response.resultados.Count() > 1)
                    {
                        Utilities.Result = Utilities.RespuestaConsulta.response.resultados;
                        Utilities.search = TxtIdentificacion.Text;
                        Utilities.ResetTimer();
                        navigateService.NavigationTo("FrmCoincidence");
                    }
                    else
                    {
                        if (searchType == tipo_busqueda.Nit)
                        {
                            Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados.Where(r => r.nit == r.nit);
                            foreach (var item in Utilities.ConsultResult)
                            {
                                utilities.Matricula = item.matricula;
                                utilities.Tpcm = item.tpcm;
                            }
                        }
                        else
                        {
                            if (Utilities.RespuestaConsulta.response.resultados.Count() == 1)
                            {
                                Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados.Where(r => r.nit == r.nit);
                            }
                            else
                            {
                                Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados.Where(r => r.nit == TxtIdentificacion.Text);

                                if (searchType == tipo_busqueda.Nombre && Utilities.ConsultResult.Count() < 1)
                                {
                                    Utilities.ConsultResult = Utilities.RespuestaConsulta.response.resultados.Where(r => r.nombre == TxtIdentificacion.Text);
                                }
                            }
                            foreach (var item in Utilities.ConsultResult)
                            {
                                utilities.Matricula = item.matricula;
                                utilities.Tpcm = item.tpcm;
                            }
                        }
                        Utilities.ResetTimer();
                        navigateService.NavigationTo("FrmInformationCompany");
                    }
                }
                else
                {
                    Utilities.Loading(frmLoading, false, this);
                    Utilities.OpenModal(string.Concat("No se encontraron registros con este número de identificación",
                        Environment.NewLine, "Por favor intentelo de nuevo."), this);
                }
            }
            else
            {
                Utilities.Loading(frmLoading, false, this);
                Utilities.OpenModal(string.Concat("Lo sentimos, ",
                        Environment.NewLine, "No se pudo establecer conexión con el servicio.",
                        Environment.NewLine, "Por favor intentelo de nuevo."), this);
            }
        }

        #endregion
    }
}
