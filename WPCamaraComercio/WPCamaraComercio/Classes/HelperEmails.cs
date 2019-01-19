using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.WCFPayPad;

namespace WPCamaraComercio.Classes
{
    public class HelperEmails
    {
        private static ServicePayPadClient WCFPayPadWS = new ServicePayPadClient();

        public static void SendEmail(string mensaje)
        {
            try
            {
                WCFPayPadWS.EnviarCorreo(mensaje, ScreenControl.Sucursal, ScreenControl.IdCorrespo, "Email 101", ScreenControl.MailTo);
            }
            catch { }
        }
    }
}
