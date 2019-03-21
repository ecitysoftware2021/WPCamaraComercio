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
                FrmModal frmModal = new FrmModal(message,Page);
                frmModal.ShowDialog();
            }));
        }

        public void NavigationTo(string page)
        {
            Page.Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (page)
                {
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
                    case "FrmPayment":
                        FrmPayment frmPayment = new FrmPayment();
                        frmPayment.Show();
                        break;
                    default:
                        break;
                }
                Page.Close();
            }));
        }
    }
}
