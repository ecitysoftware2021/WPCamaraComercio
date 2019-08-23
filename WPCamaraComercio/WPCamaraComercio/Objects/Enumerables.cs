using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Objects
{
    public enum ELogType
    {
        General = 0,
        Error = 1,
        Device = 2
    }
    public enum EError
    {
        Printer = 1,
        Nopapper = 2,
        Device = 3,
        Aplication = 5,
        Api = 6,
        Customer = 7,
        Internet = 8
    }
    public enum ELevelError
    {
        Mild = 3,
        Medium = 2,
        Strong = 1,
    }
}
