using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Models
{
    public partial class PAYPAD_ERROR_CONSOLE
    {
        public int PAYPAD_CONSOLE_ERROR_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int ERROR_ID { get; set; }
        public int ERROR_LEVEL_ID { get; set; }
        public Nullable<int> DEVICE_PAYPAD_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public System.DateTime DATE { get; set; }
        public string OBSERVATION { get; set; }
        public int STATE { get; set; }
    }
}
