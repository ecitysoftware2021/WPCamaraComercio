using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Classes
{
    public class ResultadoGeneral
    {
        public string id { get; set; }
        public string activo { get; set; }
        public string nit { get; set; }
        public string digito_verificacion { get; set; }
        public string matricula { get; set; }
        public string matricula_est { get; set; }
        public string tpcm { get; set; }
        public string nombre { get; set; }
        public string estado { get; set; }
        public string municipio { get; set; }
        public string fecha_renovacion { get; set; }
        public string detalle { get; set; }
        public string EstablecimientosConCoincidencia { get; set; }
        public string crr { get; set; }
        public string Orden { get; set; }
        public string RecordId { get; set; }
    }

    public class ResponseGeneral
    {
        public string codigo { get; set; }
        public string mensaje { get; set; }
        public int registros { get; set; }
        public string frid { get; set; }
        public string lrid { get; set; }
        public List<ResultadoGeneral> resultados { get; set; }
    }

    public class ResultGeneral
    {
        public object estado { get; set; }
        public object error { get; set; }
        public ResponseGeneral response { get; set; }
    }

    public class ResponseConsultGeneral
    {
        public bool IsSuccess { get; set; }
        public object Message { get; set; }
        public ResultGeneral Result { get; set; }
    }
}
