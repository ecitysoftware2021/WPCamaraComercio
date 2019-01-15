﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPCamaraComercio.Models
{
    class DetailEstablish
    {
        public string Establish { get; set; }

        public decimal Amount { get; set; }

        public Details Details { get; set; }

        public string Certificate { get; set; }

        public EstablishCertificate EstablishCertificate { get; set; }
    }
}
