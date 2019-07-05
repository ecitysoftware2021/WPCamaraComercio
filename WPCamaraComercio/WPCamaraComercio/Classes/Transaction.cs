using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Classes
{
    class Tbl_Transaction
    {
    }

    public class TRANSACTION
    {
        public int ID { get; set; }
        public Nullable<int> TRANSACTION_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int TYPE_TRANSACTION_ID { get; set; }
        public Nullable<DateTime> DATE_BEGIN { get; set; }
        public Nullable<DateTime> DATE_END { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public Nullable<decimal> INCOME_AMOUNT { get; set; }
        public Nullable<decimal> RETURN_AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public int PAYER_ID { get; set; }
        public int STATE_TRANSACTION_ID { get; set; }
        public int STATE_NOTIFICATION { get; set; }
        public int STATE { get; set; }
        public string TRANSACTION_REFERENCE { get; set; }

        public List<TRANSACTION_DESCRIPTION> TRANSACTION_DESCRIPTION = new List<TRANSACTION_DESCRIPTION>();
    }
    public class TRANSACTION_DESCRIPTION
    {
        public int TRANSACTION_ID { get; set; }
        public int TRANSACTION_DESCRIPTION_ID { get; set; }
        public string REFERENCE { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public string OBSERVATION { get; set; }
        public Nullable<bool> STATE { get; set; }
    }

    public class PAYER
    {
        public int PAYER_ID { get; set; }
        public string IDENTIFICATION { get; set; }
        public string NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<decimal> PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public bool STATE { get; set; }
    }
}

