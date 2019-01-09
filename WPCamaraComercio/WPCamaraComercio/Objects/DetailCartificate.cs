using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Objects
{
    public class DetailCartificate
    {
        public string NombreCertificado { get; set; }
        public int Cantidad { get; set; }
        public string Matricula { get; set; }
        public string TipoCertificado { get; set; }
        public int IDCertificado { get; set; }
        public List<string> Certificados { get; set; }
    }
}
