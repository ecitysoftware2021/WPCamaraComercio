using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Services.Object
{
    public class ResponseApi
    {
        public int CodeError { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class ResponseAuth
    {
        public int CodeError { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public object Session { get; set; }
        public object User { get; set; }
    }

    public class ResponseConsecutive
    {
        public string PREFIJO { get; set; }
        public string RESOLUCION { get; set; }
        public DateTime FECHA_RESOLUCION { get; set; }
        public double RANGO_DESDE { get; set; }
        public double RANGO_HASTA { get; set; }
        public double RANGO_ACTUAL { get; set; }
        public Nullable<bool> IS_AVAILABLE { get; set; }
        public Nullable<int> CUSTOMER_ID { get; set; }
    }
}
