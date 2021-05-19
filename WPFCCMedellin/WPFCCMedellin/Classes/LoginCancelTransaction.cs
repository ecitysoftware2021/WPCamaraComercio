using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Classes
{
    public class LoginCancelTransaction
    {
        public class UserSession2
        {
            public int USER_ID { get; set; }

            public int CUSTOMER_ID { get; set; }

            public string IDENTIFICATION { get; set; }

            public string USERNAME { get; set; }

            public string PASSWORD { get; set; }

            public string EMAIL { get; set; }

            public string PHONE { get; set; }

            public bool STATE { get; set; }

            public List<Role2> Roles { get; set; }

            public Nullable<bool> SEND_MAIL { get; set; }

            public byte[] IMAGE { get; set; }

            public string NAME { get; set; }

            public DateTime RETREAT_DATE { get; set; }
            public string COIN_RESPONSE { get; set; }
        }

        public class RequestAuthentication2
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public int Type { get; set; }
        }

        public class Response2
        {
            public int CodeError { get; set; }

            public string Message { get; set; }

            public object Data { get; set; }
        }

        public class UserViewModel2
        {
            public int USER_ID { get; set; }

            public int CUSTOMER_ID { get; set; }

            public int ROLE_ID { get; set; }

            public string IDENTIFICATION { get; set; }

            public string NAME { get; set; }

            public string USERNAME { get; set; }

            public string PASSWORD { get; set; }

            public string EMAIL { get; set; }

            public byte[] IMAGE { get; set; }

            public string PHONE { get; set; }

            public bool STATE { get; set; }

            public string CUSTOMER_NAME { get; set; }

            public string OFFICE_NAME { get; set; }

            public string ROL_NAME { get; set; }

        }

        public class Role2
        {
            public int ROLE_ID { get; set; }

            public string DESCRIPTION { get; set; }

            public bool STATE { get; set; }

            public virtual ICollection<UserSession2> Users { get; set; }
        }

        public class ResponseAuth2
        {
            public int CodeError { get; set; }
            public string Message { get; set; }
            public string Token { get; set; }
            public object Session { get; set; }
            public object User { get; set; }
        }

        public class RequestAuth2
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public int Type { get; set; }

        }
    }
}