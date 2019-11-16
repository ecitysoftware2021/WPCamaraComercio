using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCCMedellin.Models;

namespace WPFCCMedellin.Services
{
    class Request
    {
    }

    public class RequestConsult
    {
        public string paramConsulta { get; set; }

        public int tipo_Busqueda { get; set; }
    }

    public class RequestDetail
    {
        public string Matricula { get; set; }

        public string Tpcm { get; set; }
    }

    public class RequestPayment
    {
        public string AutorizaEnvioEmail { get; set; }

        public string AutorizaEnvioSMS { get; set; }

        public List<Product> Certificados { get; set; }

        public int CodigoDepartamentoComprador { get; set; }

        public int CodigoMunicipioComprador { get; set; }

        public int CodigoPaisComprador { get; set; }

        public string DireccionComprador { get; set; }

        public string EmailComprador { get; set; }

        public string IdCliente { get; set; }

        public string IdentificacionComprador { get; set; }

        public string MunicipioComprador { get; set; }

        public string NombreComprador { get; set; }

        public string PlataformaCliente { get; set; }

        public string PrimerApellidoComprador { get; set; }

        public string PrimerNombreComprador { get; set; }

        public string ReferenciaPago { get; set; }

        public string SegundoApellidoComprador { get; set; }

        public string SegundoNombreComprador { get; set; }

        public string TelefonoComprador { get; set; }

        public string TipoComprador { get; set; }

        public string TipoIdentificacionComprador { get; set; }

        public int ValorCompra { get; set; }
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
