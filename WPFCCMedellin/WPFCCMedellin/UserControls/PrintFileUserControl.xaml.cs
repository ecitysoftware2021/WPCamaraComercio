using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Classes.Printer;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;

namespace WPFCCMedellin.UserControls
{
    /// <summary>
    /// Lógica de interacción para PrintFileUserControl.xaml
    /// </summary>
    public partial class PrintFileUserControl : UserControl
    {
        private Transaction transaction;

        public PrintFileUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;
        }

        public void DownloadCertificates()
        {
            try
            {
                Task.Run(async () => 
                {
                    bool statusPrint = true;
                    var pathsCertificates = await AdminPayPlus.ApiIntegration.DownloadCertificates(transaction);
                    if (pathsCertificates.Count > 0)
                    {
                        foreach (var path in pathsCertificates)
                        {
                            if (!AdminPayPlus.PrinterFile.Start(path))
                            {
                                statusPrint = false;
                                break;
                            }
                        }
                    }

                    if (!statusPrint)
                    {
                        Utilities.ShowModal("", EModalType.Error);
                    }

                    Utilities.navigator.Navigate(UserControlView.PaySuccess, false, transaction);
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
