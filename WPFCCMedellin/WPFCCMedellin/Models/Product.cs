using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Models
{
    public class Product
    {
        public string matricula { get; set; }
        public string MatriculaEst { get; set; }
        public string tpcm { get; set; }
        public string IdCertificado { get; set; }
        public string CodigoGeneracion { get; set; }
        public string NumeroCertificados { get; set; }
        public EstablishCertificate EstablishCertificate { get; set; }
    }

    public class EstablishCertificate
    {
        public decimal CertificateCost { get; set; }
        public string CertificateId { get; set; }
        public string EstablishEnrollment { get; set; }
        public string GenerationCode { get; set; }
        public string NombreCertificado { get; set; }
    }
}
