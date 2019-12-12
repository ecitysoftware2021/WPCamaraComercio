﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Models
{
    public class Coincidence
    {
        public string BusinessName { get; set; }

        public string Nit { get; set; }

        public string Municipality { get; set; }

        public string EstabliCoincide { get; set; }

        public string State { get; set; }

        public string Enrollment { get; set; }

        public string Tpcm { get; set; }

    }

    public class GeneralRequest
    {
        public string paramConsulta { get; set; }
        public int tipo_Busqueda { get; set; }
    }
}