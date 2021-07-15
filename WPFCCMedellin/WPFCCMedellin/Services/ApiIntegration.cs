using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services.Object;

namespace WPFCCMedellin.Services
{
    public class ApiIntegration
    {
        private HttpWebRequest client;

        private string basseAddress;
        private string basseAddressPDF;

        private ASCIIEncoding encoding;

        private Stream newStream;

        static string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        static string CID = "1CERO1";
        static string CLAVE = "1C3R0120Z1";
        static string cadena = string.Concat(CLAVE, fecha, CID);

        public ApiIntegration()
        {
            encoding = new ASCIIEncoding();

            basseAddress = Utilities.GetConfiguration("basseAddressIntegration");
            basseAddressPDF = Utilities.GetConfiguration("basseAddressPDF");
        }

        private string GenerateMD5()
        {
            string cadena = string.Concat(CLAVE, fecha, CID);


            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(cadena);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public async Task<ResponseApi> GetData(object data, string controlador)
        {
            try
            {
                RequestCCM requestCCM = new RequestCCM
                {
                    security = new Security
                    {
                        CID = CID,
                        fecha = fecha,
                        token = GenerateMD5()
                    },
                    request = data
                };
                var client = new RestClient(string.Concat(basseAddress, Utilities.GetConfiguration(controlador)));
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                AdminPayPlus.SaveErrorControl($"Datos enviados: {controlador}", JsonConvert.SerializeObject(requestCCM), EError.Api, ELevelError.Medium);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(requestCCM), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                AdminPayPlus.SaveErrorControl($"Response getData: {controlador}", response.Content, EError.Aplication, ELevelError.Medium);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseApi
                    {
                        CodeError = 300,
                        Message = response.ErrorMessage
                    };
                }
                return new ResponseApi
                {
                    CodeError = 200,
                    Data = response.Content
                };
            }
            catch (Exception ex)
            {
                return new ResponseApi
                {
                    CodeError = 300,
                    Message = ex.Message
                };
            }
        }

        public async Task<Transaction> NotifycTransaction(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    string primerNombre = string.Empty;
                    string segundoNombre = string.Empty;

                    string primerApellido = string.Empty;
                    string segundoApellido = string.Empty;

                    bool first = true;

                    if (transaction.payer.NAME != null)
                    {
                        string[] nombresSeparados = transaction.payer.NAME.Split(' ');                        

                        foreach (var item in nombresSeparados)
                        {
                            if (first)
                            {
                                primerNombre = item;
                                first = false;
                            }
                            else
                            {
                                segundoNombre += item + " ";
                            }
                        } 
                    }

                    if (transaction.payer.LAST_NAME != null)
                    {
                        string[] apellidosSeparados = transaction.payer.LAST_NAME.Split(' ');

                        first = true;

                        foreach (var item in apellidosSeparados)
                        {
                            if (first)
                            {
                                primerApellido = item;
                                first = false;
                            }
                            else
                            {
                                segundoApellido += item + " ";
                            }
                        } 
                    }

                    var response = await GetData(new RequestPayment
                    {
                        AutorizaEnvioEmail = "NO",
                        AutorizaEnvioSMS = "NO",
                        CodigoDepartamentoComprador = transaction.payer.codDepartamento,
                        CodigoMunicipioComprador = transaction.payer.codMunicipio,
                        CodigoPaisComprador = int.Parse(Utilities.GetConfiguration("CodeCountryBuyer")),
                        DireccionComprador = transaction.payer.ADDRESS ?? string.Empty,
                        EmailComprador = transaction.payer.EMAIL ?? string.Empty,
                        IdentificacionComprador = transaction.payer.IDENTIFICATION,
                        MunicipioComprador = transaction.payer.municipio,
                        NombreComprador = string.Concat(transaction.payer.NAME, " ", transaction.payer.LAST_NAME),
                        PlataformaCliente = Utilities.GetConfiguration("ClientPlataform"),
                        PrimerApellidoComprador = primerApellido,
                        PrimerNombreComprador = primerNombre,
                        ReferenciaPago = transaction.IdTransactionAPi.ToString(),
                        SegundoApellidoComprador = segundoApellido,
                        SegundoNombreComprador = segundoNombre,
                        TelefonoComprador = transaction.payer.PHONE.ToString(),
                        TipoComprador = transaction.payer.TYPE_PAYER,
                        TipoIdentificacionComprador = transaction.payer.TYPE_IDENTIFICATION,
                        ValorCompra = Decimal.ToInt32(transaction.Amount),
                        Certificados = transaction.Products,
                        IdCliente = AdminPayPlus.DataConfiguration.ID_PAYPAD.Value
                    }, "SendPay");

                    if (response.CodeError == 200 && response.Data != null)
                    {
                        var result = JsonConvert.DeserializeObject<ResponsePay>(response.Data.ToString());

                        if (result.response != null && int.Parse(result.response.IdCompra) > 0)
                        {
                            transaction.consecutive = result.response.IdCompra;

                            return transaction;
                        }
                    }
                    else
                    {
                        AdminPayPlus.SaveErrorControl("SendPay : " + transaction.IdTransactionAPi.ToString(), "Error consumiendo servicio response : " + response, EError.Customer, ELevelError.Medium);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
            transaction.State = ETransactionState.ErrorService;
            return transaction;
        }

        public async Task<List<string>> DownloadCertificates(Transaction transaction)
        {
            try
            {
                List<string> pathCertificates = new List<string>();
                int countCertificates = 0;
                if (int.Parse(transaction.consecutive) > 0 && transaction.Products.Count > 0)
                {
                    foreach (var product in transaction.Products)
                    {
                        for (int i = 0; i < int.Parse(product.NumeroCertificados); i++)
                        {
                            countCertificates += 1;

                            if (!string.IsNullOrEmpty(transaction.consecutive))
                            {

                                StringBuilder sb;
                                using (SHA1Managed sha1 = new SHA1Managed())
                                {
                                    var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(transaction.consecutive, "_", transaction.IdTransactionAPi.ToString())));
                                    sb = new StringBuilder(hash.Length * 2);
                                    foreach (byte b in hash)
                                    {
                                        sb.Append(b.ToString("x2"));
                                    }
                                }

                                var nameFile = $"{transaction.consecutive}-{transaction.consecutive}" +
                                    $"-{transaction.Enrollment}-{product.MatriculaEst ?? "0"}-{product.IdCertificado}-{transaction.Tpcm}-{(i + 1).ToString()}";

                                var patchFile = string.Concat(basseAddressPDF,
                                    Utilities.GetConfiguration("GetCertifiedString"),
                                    "?idcompra=", transaction.consecutive,
                                    "&IdCertificado=", product.IdCertificado,
                                    "&matricula=", transaction.Enrollment,
                                    "&matriculaest=", product.MatriculaEst ?? "0",
                                    "&tpcm=", transaction.Tpcm,
                                    "&copia=", (i + 1),
                                    "&hs=", sb.ToString());

                                string path = DownloadFile(patchFile, nameFile);
                                if (!string.IsNullOrEmpty(path))
                                {
                                    pathCertificates.Add(path);
                                }
                                else
                                {
                                    AdminPayPlus.SaveErrorControl("name  Certificado: " + nameFile, "Error descargando certificado : " + path, EError.Customer, ELevelError.Medium);
                                }
                            }
                        }
                    }
                }

                if (pathCertificates.Count == countCertificates)
                {
                    return pathCertificates;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
            return null;
        }

        internal async Task<Transaction> NotifycCancelTransaction(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    var response = await GetData(new CancelPayment
                    {
                        IdCliente = AdminPayPlus.DataConfiguration.ID_PAYPAD.ToString(),
                        IdCompra = transaction.idCompra,
                        ValorCompra = transaction.valorCompra,
                        ReferenciaPago = transaction.referenciaPago,
                        PlataformaCliente = Utilities.GetConfiguration("ClientPlataform"),
                        Observaciones = transaction.observaciones
                    }, "BuyCancel");

                    var result = JsonConvert.DeserializeObject<ResponsePay>(response.Data.ToString());

                    if (response.CodeError == 200 && result.response.mensaje == "")
                    {
                        transaction.State = ETransactionState.Cancel;
                    }
                    else
                    {
                        transaction.message = result.response.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
            return transaction;
        }

        public string DownloadFile(string patchFile, string nameFile)
        {
            try
            {
                if (!string.IsNullOrEmpty(patchFile) && !string.IsNullOrEmpty(nameFile))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        bool stateDownload = false;
                        int countIntents = 0;
                        while (!stateDownload)
                        {
                            var response = webClient.DownloadData(patchFile);
                            var contentType = webClient.ResponseHeaders["Content-Type"];

                            if (response != null && contentType != null &&
                                contentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                stateDownload = true;
                                var path = Utilities.SaveFile(nameFile, Utilities.GetConfiguration("DirectoryFile"), response);
                                if (!string.IsNullOrEmpty(path))
                                {
                                    return path;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                countIntents++;
                                if (countIntents >= int.Parse(Utilities.GetConfiguration("IntentsDownload").ToString()))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
            return string.Empty;
        }
    }
}
