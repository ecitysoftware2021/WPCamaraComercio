using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Classes
{
    public class Datos
    {
        public string AutorizaEnvioEmail { get; set; }

        public string AutorizaEnvioSMS { get; set; }

        public Certificado[] Certificados { get; set; }

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

        public decimal ValorCompra { get; set; }
    }
}
