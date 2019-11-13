using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace WPFCCMedellin.Classes.Printer
{
    public class PrintService
    {
        private PrintController printController;

        private PrintDocument printDocument;

        private Graphics graphics;

        private int y = 0;

        private int sum = 30;

        private int x = 200;

        private List<DataPrinter> dataPrinter;

        private bool StatePrint;

        private PrintProperties properties;

        public PrintService()
        {
            if (properties == null)
            {
                properties = new PrintProperties(Utilities.GetConfiguration("PortPrinter"), Utilities.GetConfiguration("PrintBandrate"));
            }

            if (printController == null)
            {
                printController = new StandardPrintController();
            }

            if (printDocument == null)
            {
                printDocument = new PrintDocument();

                printDocument.PrintController = printController;

                printDocument.PrintPage += new PrintPageEventHandler(Print);
            }
        }

        public int StatusPrint()
        {
            int status = 1;
            try
            {
                if (properties != null)
                {
                    status = properties.GetPrintStatus();
                    if (status == 0)
                    {
                        StatePrint = true;
                        return 0;
                    }
                }
            }
            catch (Exception EX)
            {
            }
            StatePrint = false;
            return status;
        }

        public string MessageStatus(int status)
        {
            return properties.MessageStatus(status);
        }

        public void Start(List<DataPrinter> dataPrinter)
        {
            try
            {
                Task.Run(() =>
                {
                    StatusPrint();
                    this.dataPrinter = dataPrinter;
                    if (StatePrint && dataPrinter != null)
                    {
                        printDocument.Print();
                    }
                });
            }
            catch (Exception ex)
            {
            }
        }

        private void Print(object sender, PrintPageEventArgs e)
        {
            try
            {
                if (dataPrinter.Count > 0)
                {
                    foreach (var item in dataPrinter)
                    {
                        if (!string.IsNullOrEmpty(item.image))
                        {
                            graphics = e.Graphics;
                            graphics.DrawImage(Image.FromFile(item.image), item.x, item.y);
                        }
                        else
                        {
                            graphics.DrawString(item.value, item.font, item.brush, item.x, item.y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}