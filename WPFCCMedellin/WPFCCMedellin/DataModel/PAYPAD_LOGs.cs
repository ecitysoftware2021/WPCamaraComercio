using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.DataModel
{
    public partial class PAYPAD_LOG
    {
        public int PAYPAD_LOG_ID { get; set; }
        public string REFERENCE { get; set; }
        public string DESCRIPTION { get; set; }
        public string DATE { get; set; }
        public int STATE { get; set; }
    }
}
