using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFCCMedellin.Classes;

namespace WPFCCMedellin.Services.Object
{
    public class DataPayPlus
    {
        public bool State { get; set; }

        public bool StateAceptance { get; set; }

        public bool StateDispenser { get; set; }

        public string Message { get; set; }

        public bool StateBalanece { get; set; }

        public bool StateUpload { get; set; }

        public bool StateUpdate { get; set; }

        public bool StateDiminish { get; set; }

        public object ListImages { get; set; }

        public int ContTransactionsNotific { get; set; }

        public int ContTransactions { get; set; }
        public PaypadConfiguration PayPadConfiguration { get; set; }
    }

    public class PaypadConfiguration
    {
        public int paypaD_ID { get; set; }
        public string bilL_PORT { get; set; }
        public string coiN_PORT { get; set; }
        public string unifieD_PORT { get; set; }
        public string printeR_PORT { get; set; }
        public int printeR_BAUD_RATE { get; set; }
        public string dispenseR_CONFIGURATION { get; set; }
        public string localdB_PATH { get; set; }
        public string publicitY_TIMER { get; set; }
        public string generiC_TIMER { get; set; }
        public string modaL_TIMER { get; set; }
        public string inactivitY_TIMER { get; set; }
        public string extrA_DATA { get; set; }
        public ExtraData ExtrA_DATA { get; set; }
        public bool enablE_CARD { get; set; }
        public bool enablE_VALIDATE_PERIPHERALS { get; set; }
        public bool iS_UNIFIED { get; set; }
        public bool iS_PRODUCTION { get; set; }
        public string keyS_PATH { get; set; }
        public string imageS_PATH { get; set; }
        public string scanneR_PORT { get; set; }

        public void DeserializarExtraData()
        {
            try
            {
                ExtrA_DATA = JsonConvert.DeserializeObject<ExtraData>(extrA_DATA);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }
    }

    public class ExtraData
    {
        public DataComplementary dataComplementary { get; set; }
        public DataIntegration dataIntegration { get; set; }
    }

    public class DataComplementary
    {
        public string DurationAlert { get; set; }
        public string NAME_PAYPAD { get; set; }
        public string LAST_NAME_PAYPAD { get; set; }
        public string NAME_APLICATION { get; set; }
        public string NIT { get; set; }
        public string ProductName { get; set; }
        public string DirectoryFile { get; set; }
        public string PathTypeDocument { get; set; }
        public string CuantityItemsList { get; set; }
        public string ClientPlataform { get; set; }
        public string IntentsDownload { get; set; }
        public string PrinterName { get; set; }
        public string UrlBackground { get; set; }
    }

    public class DataIntegration
    {
        public DESARROLLO desarrollo { get; set; }
        public PRODUCCION produccion { get; set; }
        public AMBIENTE ambiente { get; set; }
        public void DefinirAmbiente(bool iS_PRODUCTION)
        {
            if (iS_PRODUCTION)
            {
                ambiente.UserAPI = produccion.UserAPI;
                ambiente.PassAPI = produccion.PassAPI;
                ambiente.basseAddressIntegration = produccion.basseAddressIntegration;
            }
            else
            {
                ambiente.UserAPI = desarrollo.UserAPI;
                ambiente.PassAPI = desarrollo.PassAPI;
                ambiente.basseAddressIntegration = desarrollo.basseAddressIntegration;
            }
        }
    }

    public class DESARROLLO : AMBIENTE { }

    public class PRODUCCION : AMBIENTE { }

    public class AMBIENTE
    {
        public string UserAPI { get; set; }
        public string PassAPI { get; set; }
        public string basseAddressIntegration { get; set; }
        public string GetTokenIntegration { get; set; }
        public string ConsultFileMercantil { get; set; }
        public string LiquidateNormalRenewal { get; set; }
        public string SearchFiles { get; set; }
        public string SendTransaction { get; set; }
        public string SendPay { get; set; }
        public string GetDiscount { get; set; }
        public string ConsultSettled { get; set; }
        public string ConsultReceipt { get; set; }
        public string BuyCancel { get; set; }
        public string CodeCompany { get; set; }
        public string OperadorControl { get; set; }
        public string EmailControl { get; set; }
        public string IdControl { get; set; }
        public string NameControl { get; set; }
        public string Proyect { get; set; }
        public string TypeTransaction { get; set; }
        public string CodificationService { get; set; }
        public string PhoneControl { get; set; }
        public string MethodPayment { get; set; }
    }
}

