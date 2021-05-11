using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Services.Object
{
    public class DataPayer
    {
        public string TipoComprador { get; set; }
        public string TipoIdentificacionComprador { get; set; }
        public string IdentificacionComprador { get; set; }
        public string PrimerNombreComprador { get; set; }
        public string SegundoNombreComprador { get; set; }
        public string PrimerApellidoComprador { get; set; }
        public string SegundoApellidoComprador { get; set; }
        public string NombreComprador { get; set; }
        public string EmailComprador { get; set; }
        public string TelefonoComprador { get; set; }
        public string TipoViaComprador { get; set; }
        public string NumeroViaComprador { get; set; }
        public string AdicionalViaComprador { get; set; }
        public string NumeroCruceComprador { get; set; }
        public string AdicionalCruceComprador { get; set; }
        public string UbicacionComprador { get; set; }
        public string DireccionComprador { get; set; }
        public string CodigoPaisComprador { get; set; }
        public string CodigoDepartamentoComprador { get; set; }
        public string CodigoMunicipioComprador { get; set; }
        public string MunicipioComprador { get; set; }
        public string AutorizaEnvioEmail { get; set; }
        public string AutorizaEnvioSMS { get; set; }
        public string NombrePaisComprador { get; set; }
        public string NombreDepartamentoComprador { get; set; }
        public string NombreMunicipioComprador { get; set; }
        public string Fuente { get; set; }
        public string BloqueaNombre { get; set; }
        public string DepartamentoDANE { get; set; }
        public string MunicipioDANE { get; set; }
    }

    public class Response
    {
        public string codigo { get; set; }
        public string mensaje { get; set; }
        public List<DataPayer> resultados { get; set; }
    }

    public class ResponsePayer
    {
        public Response response { get; set; }
    }
}
