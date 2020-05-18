using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFCCMedellin.Classes;
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
                        Utilities.ShowModal(MessageResource.ErrorDownloadCertificate, EModalType.Error);
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
                int countCertificates = 0;

                if (paths != null && paths.Count > 0)
                {
                    AdminPayPlus.PrinterFile.callbackOut = response =>
                    {
                        countCertificates++;
                        if (response && countCertificates < (paths.Count))
                        {
                            AdminPayPlus.PrinterFile.Start(paths[countCertificates]);
                        }
                        else if (countCertificates == paths.Count)
                        {
                            AdminPayPlus.PrinterFile.callbackOut = null;
                            AdminPayPlus.PrinterFile.callbackError = null;
                            FinishTransaction(true);
                        }
                    };

                    AdminPayPlus.PrinterFile.callbackError = error =>
                    {

                        if (countCertificates > 1)
                        {
                            AdminPayPlus.PrinterFile.callbackOut = null;
                            AdminPayPlus.PrinterFile.callbackError = null;
                            Utilities.ShowModal(MessageResource.ErrorPrintCertificate, EModalType.Error);
                            if (countCertificates == paths.Count)
                            {
                                FinishTransaction(true);
                            }
                        }
                        else
                        {
                            AdminPayPlus.PrinterFile.callbackOut = null;
                            AdminPayPlus.PrinterFile.callbackError = null;
                            Utilities.ShowModal(MessageResource.ErrorCertificatesPrint, EModalType.Error);
                            FinishTransaction(false);
                        }
                    };

                    AdminPayPlus.PrinterFile.Start(paths[countCertificates]);
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
