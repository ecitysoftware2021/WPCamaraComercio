using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Objects
{
    public class Transaction
    {
        public int TRANSACTION_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int TYPE_TRANSACTION_ID { get; set; }
        public DateTime DATE_BEGIN { get; set; }
        public DateTime DATE_END { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal INCOME_AMOUNT { get; set; }
        public decimal RETURN_AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public int PAYER_ID { get; set; }
        public int STATE_TRANSACTION_ID { get; set; }
    }
}
