using System;
using System.Collections.Generic;
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
    /// Interaction logic for LoadingModal.xaml
    /// </summary>
    public partial class LoadingModal : Window
    {
        private Coincidence coincidence;

        private WCFServices services;

        private Utilities utilities;

        public LoadingModal(Coincidence coincidence)
        {
            InitializeComponent();

            this.coincidence = coincidence;

            this.services = new WCFServices();

            ConsultDetail();
        }


        private void ConsultDetail()
        {
            try
            {
                Utilities.Enrollment = coincidence.Enrollment;
                Utilities.Tpcm = coincidence.Tpcm;

                Task.Run(async () =>
                {
                    var response = await services.ConsultDetailMerchant(coincidence.Enrollment, coincidence.Tpcm);

                    if (response.IsSuccess)
                    {
                        Utilities.DetailResponse = (RespuestaDetalle)response.Result;
                        ChangeView();
                    }
                });
            }
            catch (Exception ex)
            {
                //utilities.saveLogError("RedirecView", "RecordsWindows", ex.ToString());
            }
        }

        private void ChangeView()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    this.LblMessage.Text = string.Concat("Ha seleccionado, ", coincidence.BusinessName, " Desea continuar");

                    this.GifLoadder.Visibility = Visibility.Hidden;

                    this.BtnCancel.Visibility = Visibility.Visible;
                    this.BtnContinue.Visibility = Visibility.Visible;
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BtnContinue_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
