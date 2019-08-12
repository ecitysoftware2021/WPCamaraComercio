using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Classes
{
    public class ResponseTransactionList
    {
        public int Número_Pedido { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime Fecha_Pago { get; set; }

        public decimal Total { get; set; }

        public string IVA { get; set; }

        public string Medio_Pago { get; set; }

        public string Estado_Pedido { get; set; }

        public string estado_Pago { get; set; }

        public string Identificación { get; set; }

        public string Nombre_Cliente { get; set; }

        public string Apellido_Cliente { get; set; }

        public string Email { get; set; }

        public string Teléfono { get; set; }

        public string Tipo { get; set; }

        public int TicketId { get; set; }

        public int Cod_Transacción { get; set; }

        public string Ciclo { get; set; }

        public string Banco { get; set; }

        public string Campo_1 { get; set; }

        public string Campo_2 { get; set; }

        public string Campo_3 { get; set; }

        public string Concepto { get; set; }
    }
}
