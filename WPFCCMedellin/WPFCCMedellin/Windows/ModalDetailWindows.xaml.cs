using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services.Object;
using WPFCCMedellin.ViewModel;

namespace WPFCCMedellin.Windows
{
    /// <summary>
    /// Lógica de interacción para ModalDetailWindows.xaml
    /// </summary>
    public partial class ModalDetailWindows : Window
    {
        private DetailViewModel viewModel;

        private ETypeCertificate type;

        public ModalDetailWindows(object data, ETypeCertificate type)
        {
            InitializeComponent();

            this.type = type;

            ConfigView(data);
        }

        private void ConfigView(object data)
        {
            try
            {
                if (type == ETypeCertificate.Merchant)
                {
                    var infoMerchant = (ResultadoDetalle)data;
                    viewModel = new DetailViewModel
                    {
                        Row1 = "Razón social:",
                        Value1 = infoMerchant.come_Nom,
                        Row2 = "Última renovación:",
                        Value2 = infoMerchant.UltRenv,
                        Row3 = "Establecimientos activos:",
                        Value3 = infoMerchant.numeroestablecimientosactivos,
                        Row4 = "Dirección comercial:",
                        Value4 = string.Concat(infoMerchant.dir_come, " - ", infoMerchant.Mpio_Come_Nom),
                        Row5 = "Nit:",
                        Value5 = infoMerchant.identificacion.Replace(",", ".").Trim(),
                        Row6 = "Tipo de sociedad:",
                        Value6 = infoMerchant.Tpcm_Desc,
                        Row7 = "Fecha inicio:",
                        Value7 = infoMerchant.Fec_Inicio,
                        Row8 = "Estado:",
                        Value8 = infoMerchant.Activo,
                        Tittle = "Detalles Comerciante",
                    };
                }
                else
                {
                    var infoEstablishment = (Establecimiento)data;
                    viewModel = new DetailViewModel
                    {
                        Row1 = "Establecimiento:",
                        Value1 = infoEstablishment.NombreEstablecimiento,
                        Row2 = "Dirección:",
                        Value2 = infoEstablishment.DireccionEstablecimiento,
                        Row3 = "Matrícula:",
                        Value3 = infoEstablishment.MatriculaEst,
                        Row4 = "Estado:",
                        Value4 = infoEstablishment.EstadoEstablecimiento == "yes" ? "Activo" : "Inactivo",
                        Tittle = "Detalles Establecimiento",
                    };
                }
                this.DataContext = viewModel;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Btn_acept_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
