using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Classes
{
    class Logs
    {
    }

    public class LogDispenser
    {
        public string SendMessage { get; set; }

        public string ResponseMessage { get; set; }

        public string TransactionId { get; set; }

        public DateTime DateDispenser { get; set; }
    }

    public class LogErrorGeneral
    {
        public string UserId { get; set; }
        public int IdTransaction { get; set; }
        public decimal ValuePay { get; set; }
        public string Date { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public int IDCorresponsal { get; set; }
    }

    public class LogError
    {
        public DateTime Fecha { get; set; }
        public int IDTrsansaccion { get; set; }
        public string Operacion { get; set; }
        public string Error { get; set; }
    }
}
