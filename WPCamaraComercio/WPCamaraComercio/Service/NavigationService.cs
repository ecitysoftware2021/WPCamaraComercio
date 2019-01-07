using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Views;

namespace WPCamaraComercio.Service
{
    class NavigationService
    {
        public Window Page { get; set; }

        public NavigationService(Window page)
        {
            this.Page = page;
        }

        public void NavigatorModal(string message)
        {
            Page.Dispatcher.BeginInvoke(new Action(() =>
            {
                //FrmModal frmModal = new FrmModal(message);
                //frmModal.ShowDialog();
            }));
        }

        public void NavigationTo(string page)
        {
            Page.Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (page)
                {
                    case "FrmMenu":
                        FrmMenu frmMenu = new FrmMenu();
                        frmMenu.Show();
                        break;
                    case "ConsultWindow":
                        ConsultWindow frmConsult = new ConsultWindow();
                        frmConsult.Show();
                        break;
                    case "FrmPaymentData":
                        FrmPaymentData frmPaymentData = new FrmPaymentData();
                        frmPaymentData.Show();
                        break;
                    case "FrmDetailCompany":
                        FrmDetailCompany frmDetailCompany = new FrmDetailCompany();
                        frmDetailCompany.Show();
                        break;
                    case "FrmInformationCompany":
                        FrmInformationCompany frmInformationCompany = new FrmInformationCompany();
                        frmInformationCompany.Show();
                        break;
                    case "FrmPayment":
                        FrmPayment frmPayment = new FrmPayment();
                        frmPayment.Show();
                        break;
                    case "FrmCancelPay":
                        //FrmCancelPay frmcancelpayment = new FrmCancelPay();
                        //frmcancelpayment.Show();
                        //break;
                    case "FrmFinishTransaction":
                        //FrmFinishTransaction frmFinishTransaction = new FrmFinishTransaction();
                        //frmFinishTransaction.Show();
                        break;
                    case "FrmCoincidence":
                        FrmCoincidence frmCoincidence = new FrmCoincidence();
                        frmCoincidence.Show();
                        break;
                    case "FrmLoading":
                        //FrmLoading frmLoading = new FrmLoading();
                        //frmLoading.Show();
                        break;
                    default:
                        break;
                }

                Page.Close();
            }));
        }
    }
}
