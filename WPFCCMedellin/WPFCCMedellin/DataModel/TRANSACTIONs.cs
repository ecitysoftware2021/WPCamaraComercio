using System;
using System.Collections.Generic;

namespace WPFCCMedellin.DataModel
{
    public partial class TRANSACTION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRANSACTION()
        {
            this.TRANSACTION_DESCRIPTION = new HashSet<TRANSACTION_DESCRIPTION>();
        }

        public int ID { get; set; }
        public Nullable<int> TRANSACTION_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int TYPE_TRANSACTION_ID { get; set; }
        public DateTime DATE_BEGIN { get; set; }
        public DateTime DATE_END { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public Nullable<decimal> INCOME_AMOUNT { get; set; }
        public Nullable<decimal> RETURN_AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public int PAYER_ID { get; set; }
        public int STATE_TRANSACTION_ID { get; set; }
        public int STATE_NOTIFICATION { get; set; }
        public int STATE { get; set; }
        public string TRANSACTION_REFERENCE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSACTION_DESCRIPTION> TRANSACTION_DESCRIPTION { get; set; }
    }

    public partial class ITRANSACTION
    {
        public int ID { get; set; }
        public Nullable<int> TRANSACTION_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int TYPE_TRANSACTION_ID { get; set; }
        public string DATE_BEGIN { get; set; }
        public string DATE_END { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public Nullable<decimal> INCOME_AMOUNT { get; set; }
        public Nullable<decimal> RETURN_AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public int PAYER_ID { get; set; }
        public int STATE_TRANSACTION_ID { get; set; }
        public int STATE_NOTIFICATION { get; set; }
        public int STATE { get; set; }
        public string TRANSACTION_REFERENCE { get; set; }
    }
}
