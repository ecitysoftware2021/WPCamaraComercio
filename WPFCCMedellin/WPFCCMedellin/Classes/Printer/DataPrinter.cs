using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Classes.Printer
{
    public class DataPrinter
    {
        public string key { get; set; }

        public string value { get; set; }

        public string image { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public Font font { get; set; }

        public SolidBrush brush { get; set; }
    }
}
