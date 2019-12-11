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

            DownloadCertificates();
        }

        public void DownloadCertificates()
        {
            try
            {
                Task.Run(async () => 
                {
                    var pathsCertificates = await AdminPayPlus.ApiIntegration.DownloadCertificates(transaction);
                    if (pathsCertificates != null && pathsCertificates.Count > 0)
                    {
                        PrintCertificates(pathsCertificates);
                    }
                    else
                    {
                        FinishTransaction(false);
                    }  
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void PrintCertificates(List<string> paths)
        {
            try
            {
                int countCertificates = 1;

                if (paths != null && paths.Count > 0)
                {
                    AdminPayPlus.PrinterFile.callbackOut = response =>
                    {
                        if (response && countCertificates < paths.Count)
                        {
                            AdminPayPlus.PrinterFile.Start(paths[countCertificates]);
                            countCertificates++;
                        }

                        if (countCertificates == paths.Count)
                        {
                            FinishTransaction(true);

                        }
                    };

                    AdminPayPlus.PrinterFile.callbackError = error =>
                    {
                        if (countCertificates > 1)
                        {
                            Utilities.ShowModal(MessageResource.ErrorPrintCertificate, EModalType.Error);
                            FinishTransaction(true);
                        }
                        else
                        {
                            FinishTransaction(false);
                        }
                    };

                    AdminPayPlus.PrinterFile.Start(paths[0]);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void FinishTransaction(bool statePrint)
        {
            try
            {
                if (statePrint)
                {
                    Utilities.navigator.Navigate(UserControlView.PaySuccess, false, transaction);
                }
                else
                {
                    Utilities.navigator.Navigate(UserControlView.ReturnMony, false, transaction);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
