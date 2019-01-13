using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.Classes;

namespace WPCamaraComercio.Objects
{
    public class ObjectsApi
    {
        public class RequestApi
        {
            public RequestApi()
            {
                User = Utilities.CorrespondentId;
                Session = Utilities.Session;
            }

            public int Session { get; set; }
            public int User { get; set; }
            public object Data { get; set; }
        }

        public class ResponseApi
        {
            public int CodeError { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }

    }
}
