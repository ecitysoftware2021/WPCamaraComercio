using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.WCFPayPad;

namespace WPCamaraComercio.Service
{
    public class WCFPayPadService
    {
        ServicePayPadClient WCFPayPad = new ServicePayPadClient();

        public int InsertarTransaccion(CLSTransaction objTransaction)
        {
            return WCFPayPad.InsertarTransaccion(objTransaction);
        }
        public bool InsertarDetalleTransaccion(int IDTransaccion, CLSEstadoEstadoDetalle Estado, decimal Valor)
        {
            return WCFPayPad.InsertarDetalleTransaccion(IDTransaccion, Estado, Valor);
        }
        public bool InsertarAuditoria(int IDTransaccion, CLSEstadoEstadoAuditoria Estado)
        {
            return WCFPayPad.InsertarAuditoria(IDTransaccion, Estado);
        }
        public string ConsultaEstadoFactura(int IDCorresponsal, int IDTramite, string Referencia)
        {
            return WCFPayPad.ConsultaEstadoFactura(IDCorresponsal, IDTramite, Referencia);
        }
        public bool InsertException(int IDCorresponsal, string Exception)
        {
            return WCFPayPad.InsertException(IDCorresponsal, Exception);
        }
    }
}
