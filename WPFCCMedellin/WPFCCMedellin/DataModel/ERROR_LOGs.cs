using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.DataModel
{
    public partial class ERROR_LOG
    {
        public int ERROR_LOG_ID { get; set; }
        public string NAME_CLASS { get; set; }
        public string NAME_FUNCTION { get; set; }
        public string MESSAGE_ERROR { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<System.DateTime> DATE { get; set; }
        public int TYPE { get; set; }
        public Nullable<bool> STATE { get; set; }
    }
}
