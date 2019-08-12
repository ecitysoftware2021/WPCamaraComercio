using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Objects
{
    public class RequestTransactions
    {
        public int PayPadId { get; set; }

        public int TransactId { get; set; }

        public int StateId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public string DateStartString { get; set; }

        public string DateFinishString { get; set; }
    }
}
