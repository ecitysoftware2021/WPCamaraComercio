using System;

namespace WPFCCMedellin.Models
{
    class Payer
    {
        public string IDENTIFICATION { get; set; }
        public string NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<decimal> PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public bool STATE { get; set; }
    }
}
