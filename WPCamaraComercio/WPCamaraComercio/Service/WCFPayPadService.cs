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
        public ServicePayPadClient WCFPayPad = new ServicePayPadClient();

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
        public  void InsertarControlMonedas(int idDenominacion, int paypadId,int operacion,int cantidad)
        {
            WCFPayPad.InsertarControlMonedas(idDenominacion, paypadId, operacion, cantidad);
        }
        public  void InsertarControlBilletes(int idDenominacion, int paypadId,int operacion,int cantidad)
        {
             var res =WCFPayPad.InsertarControlDispenser(idDenominacion, paypadId, operacion, cantidad);
        }
    }
}
