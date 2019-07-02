using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Classes
{
    public class CertificadoComerciante
    {
        public string MatriculaEstablecimiento { get; set; }
        public string NombreCertificado { get; set; }
        public string CodigoGeneracion { get; set; }
        public string IdCertificado { get; set; }
        public string ValorCertificado { get; set; }
    }

    public class CertificadosEstablecimiento
    {
        public string MatriculaEstablecimiento { get; set; }
        public string NombreCertificado { get; set; }
        public string CodigoGeneracion { get; set; }
        public string IdCertificado { get; set; }
        public string ValorCertificado { get; set; }
    }

    public class Establecimiento
    {
        public string MatriculaEst { get; set; }
        public string NombreEstablecimiento { get; set; }
        public string DireccionEstablecimiento { get; set; }
        public string EstadoEstablecimiento { get; set; }
        public string MunicipioEstablecimiento { get; set; }
        public List<CertificadosEstablecimiento> CertificadosEstablecimiento { get; set; }
    }

    public class ResultadoDetalle
    {
        public object Desc_Cicm { get; set; }
        public string dir_come { get; set; }
        public string Activo { get; set; }
        public string Fec_Inicio { get; set; }
        public string UltRenv { get; set; }
        public string Mpio_Come_Nom { get; set; }
        public string identificacion { get; set; }
        public string come_Nom { get; set; }
        public string numeroestablecimientosactivos { get; set; }
        public string come_sigla { get; set; }
        public string Tpcm_Desc { get; set; }
        public List<CertificadoComerciante> certificados { get; set; }
        public List<Establecimiento> establecimientos { get; set; }
    }

    public class TiposCertificado
    {
        public string IdCertificado { get; set; }
        public string NombreCertificado { get; set; }
        public string DescripcionCertificado { get; set; }
        public string ValorCertificado { get; set; }
    }

    public class ResponseDetalle
    {
        public string codigo { get; set; }
        public string mensaje { get; set; }
        public List<ResultadoDetalle> resultados { get; set; }
        public List<TiposCertificado> TiposCertificados { get; set; }
    }

    public class ResultDetalle
    {
        public object estado { get; set; }
        public object error { get; set; }
        public ResponseDetalle response { get; set; }
    }

    public class ResponseDetalleComerciante
    {
        public bool IsSuccess { get; set; }
        public object Message { get; set; }
        public ResultDetalle Result { get; set; }
    }
}
