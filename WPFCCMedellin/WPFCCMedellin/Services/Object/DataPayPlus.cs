using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Services.Object
{
    public class DataPayPlus
    {
        public bool State { get; set; }

        public bool StateAceptance { get; set; }

        public bool StateDispenser { get; set; }

        public string Message { get; set; }

        public bool StateBalanece { get; set; }

        public bool StateUpdate { get; set; }

        public bool StateUpload { get; set; }

        public object ListImages { get; set; }

        public int ContTransactionsNotific { get; set; }

        public int ContTransactions { get; set; }
    }
}
