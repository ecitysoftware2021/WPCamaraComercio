using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                        //FrmMenu frmMenu = new FrmMenu();
                        //frmMenu.Show();
                        //break;
                    case "FrmConsult":
                        //FrmConsult frmConsult = new FrmConsult();
                        //frmConsult.Show();
                        //break;
                    case "FrmInitial":
                        //FrmInitial frmInitial = new FrmInitial();
                        //frmInitial.Show();
                        //break;
                    case "FrmDetails":
                        //FrmDetails frmDetails = new FrmDetails();
                        //frmDetails.Show();
                        //break;
                    case "FrmPoll":
                        //FrmPoll frmPoll = new FrmPoll();
                        //frmPoll.Show();
                        //break;
                    case "FrmPay":
                        //FrmPay frmPay = new FrmPay();
                        //frmPay.Show();
                        //break;
                    case "FrmCancelPay":
                    //FrmCancelPay frmcancelpayment = new FrmCancelPay();
                    //frmcancelpayment.Show();
                    //break;
                    case "FrmFinishTransaction":
                        //FrmFinishTransaction frmFinishTransaction = new FrmFinishTransaction();
                        //frmFinishTransaction.Show();
                        break;
                    case "FrmCodeBarDetail":
                        //FrmCodeBarDetail frmCodeBarDetail = new FrmCodeBarDetail();
                        //frmCodeBarDetail.Show();
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
