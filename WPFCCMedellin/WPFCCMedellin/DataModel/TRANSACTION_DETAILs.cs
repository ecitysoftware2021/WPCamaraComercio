using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.DataModel
{
    public partial class TRANSACTION_DETAIL
    {
        public int TRANSACTION_DETAIL_ID { get; set; }
        public int TRANSACTION_ID { get; set; }
        public string CODE { get; set; }
        public Nullable<int> DENOMINATION { get; set; }
        public Nullable<int> OPERATION { get; set; }
        public Nullable<int> QUANTITY { get; set; }
        public string DESCRIPTION { get; set; }
        public int STATE { get; set; }
    }
}
