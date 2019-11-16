using Ghostscript.NET;
using Ghostscript.NET.Processor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Printing;
using WPFCCMedellin.Resources;
using System.Windows;

namespace WPFCCMedellin.Classes.Printer
{
    public class PrinterFile
    {
        private string printerName;

        private bool dobleFace;

        private LocalPrintServer printServer;

        public Action<bool> callbackOut;

        public Action<string> callbackError;

        public PrinterFile(string printerName, bool dobleFace = false)
        {
            this.printerName = printerName;
            this.dobleFace = dobleFace;

            if (printServer == null)
            {
                printServer = new LocalPrintServer();
            }
        }

        public void Start(string pathFile)
        {
            try
            {
                if (!string.IsNullOrEmpty(printerName) && !string.IsNullOrEmpty(pathFile) && GetStatus())
                {
                    using (GhostscriptProcessor processor = new GhostscriptProcessor(GhostscriptVersionInfo.GetLastInstalledVersion(), true))
                    {
                        List<string> switches = new List<string>();
                        switches.Add("-empty");
                        switches.Add("-dPrinted");
                        switches.Add("-dBATCH");
                        switches.Add("-dPDFFitPage");
                        switches.Add("-dNOPAUSE");
                        switches.Add("-dNOSAFER");
                        switches.Add("-dNOPROMPT");
                        switches.Add("-dQUIET");
                        switches.Add("-sDEVICE=mswinpr2");
                        switches.Add("-sOutputFile=%printer%" + printerName);
                        switches.Add("-dNumCopies=1");
                        switches.Add(pathFile);
                        processor.Completed += new GhostscriptProcessorEventHandler(OnCompleted);
                        processor.StartProcessing(switches.ToArray(), null);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void OnCompleted(object sender, GhostscriptProcessorEventArgs e)
        {
            try
            {
                callbackOut?.Invoke(true);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        public bool GetStatus()
        {
            bool status = false;
            try
            {
                if (!string.IsNullOrEmpty(printerName))
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        PrintQueue queue = printServer.GetPrintQueue(printerName, new string[0] { });

                        var statusPrinter = queue.QueueStatus;

                        if (statusPrinter == PrintQueueStatus.None)
                        {
                            status = true;
                        }
                    });
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
            return status;
        }
    }
}
