using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Objects
{
    public class PayerData
    {
        public string BuyerAddress { get; set; } 
        public string BuyerIdentification { get; set; } 
        public string LastNameBuyer { get; set; } 
        public string FirstNameBuyer { get; set; } 
        public string SecondNameBuyer { get; set; } 
        public string TypeBuyer { get; set; } 
        public string TypeIdBuyer { get; set; } 
        public string Phone { get; set; } 
        public string Email { get; set; }
        public int CodeCountryBuyer { get; set; }
        public int CodeDepartmentBuyer { get; set; }
        public int CodeTownBuyer { get; set; }
        public string FullNameBuyer { get; set; }
        public string ClientPlataform { get; set; }
    }
}
