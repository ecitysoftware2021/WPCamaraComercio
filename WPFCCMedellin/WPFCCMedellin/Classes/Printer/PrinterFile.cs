using Ghostscript.NET;
using Ghostscript.NET.Processor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Printing;
using WPFCCMedellin.Resources;
using System.Windows;
using System.Management;

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
                        if (dobleFace)
                        {
                            switches.Add("-dDuplex");
                            switches.Add("-dTumble");
                        }
                        switches.Add("-dNOPROMPT");
                        switches.Add("-dQUIET");
                        switches.Add("-sDEVICE=mswinpr2");
                        switches.Add("-sOutputFile=%printer%" + printerName);
                        switches.Add("-dNumCopies=1");
                        switches.Add(pathFile);
                        processor.Completed += new GhostscriptProcessorEventHandler(OnCompleted);
                        processor.Error += new GhostscriptProcessorErrorEventHandler(OnErrror);
                        processor.StartProcessing(switches.ToArray(), null);
                    }
                }
                else
                {
                    callbackError?.Invoke("Error imprimiendo");
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void OnErrror(object sender, GhostscriptProcessorErrorEventArgs e)
        {
            callbackError?.Invoke("Error imprimiendo");
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
                        ManagementScope scope = new ManagementScope(@"\root\cimv2");
                        scope.Connect();

                        // Select Printers from WMI Object Collections
                        ManagementObjectSearcher searcher = new
                        ManagementObjectSearcher("SELECT * FROM Win32_Printer");

                        string printerN = "";
                        foreach (ManagementObject printer in searcher.Get())
                        {
                            printerN = printer["Name"].ToString();
                            if (printerN.Equals(printerName))
                            {
                                if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                                {
                                    status = false;
                                }
                                else
                                {
                                    PrintQueue queue = printServer.GetPrintQueue(printerName, new string[0] { });

                                    var statusPrinter = queue.QueueStatus;

                                    if (statusPrinter == PrintQueueStatus.DoorOpen ||
                                        statusPrinter == PrintQueueStatus.Error ||
                                        statusPrinter == PrintQueueStatus.NotAvailable ||
                                        statusPrinter == PrintQueueStatus.NoToner ||
                                        statusPrinter == PrintQueueStatus.Offline ||
                                        statusPrinter == PrintQueueStatus.OutOfMemory ||
                                        statusPrinter == PrintQueueStatus.OutputBinFull ||
                                        statusPrinter == PrintQueueStatus.PagePunt ||
                                        statusPrinter == PrintQueueStatus.PaperJam ||
                                        statusPrinter == PrintQueueStatus.PaperOut ||
                                        statusPrinter == PrintQueueStatus.PaperProblem ||
                                        statusPrinter == PrintQueueStatus.ServerUnknown ||
                                        statusPrinter == PrintQueueStatus.UserIntervention)
                                    {
                                        status = false;
                                    }
                                    else
                                    {
                                        status = true;    // return true even if the printer is "out of paper"
                                    }
                                }
                            }
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
