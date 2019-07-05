using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.Models;

namespace WPCamaraComercio.Classes
{
    public class Certificado
    {
        public string matricula { get; set; }
        public string MatriculaEst { get; set; }
        public string tpcm { get; set; }
        public string IdCertificado { get; set; }
        public string CodigoGeneracion { get; set; }
        public string NumeroCertificados { get; set; }
        public EstablishCertificate EstablishCertificate { get; set; }
    }
    public class CLSDatosCertificado
    {
        public string referenciaPago { get; set; }
        public string idcompra { get; set; }
        public string IdCertificado { get; set; }
        public string matricula { get; set; }
        public string matriculaest { get; set; }
        public string tpcm { get; set; }
        public string copia { get; set; }
    }
}
