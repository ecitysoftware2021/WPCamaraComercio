using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Services.Object
{
    public class DataCity
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
    }

    public class ResponseCity
    {
        public string codigo { get; set; }
        public string mensaje { get; set; }
        public List<DataCity> resultados { get; set; }
    }

    public class City
    {
        public ResponseCity response { get; set; }
    }
}
