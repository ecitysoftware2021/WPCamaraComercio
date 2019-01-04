using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Models
{
    public class DetailMerchant
    {
        public string CertificateName { get; set; }

        public decimal Amount { get; set; }

        public EstablishCertificate EstablishCertificate { get; set; }
    }
}
