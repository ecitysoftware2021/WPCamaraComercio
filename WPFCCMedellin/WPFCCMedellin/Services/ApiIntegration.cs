using Newtonsoft.Json;
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

        private ASCIIEncoding encoding;

        private Stream newStream;

        public ApiIntegration()
        {
            encoding = new ASCIIEncoding();

            basseAddress = Utilities.GetConfiguration("basseAddressIntegration");
        }

        public async Task<ResponseApi> GetData(object requestData, string controller)
        {
            try
            {
                client = (HttpWebRequest)WebRequest.Create(new Uri(string.Concat(basseAddress, Utilities.GetConfiguration(controller))));
                var request = JsonConvert.SerializeObject(requestData);
                client.Accept = "application/json";
                client.ContentType = "application/json";
                client.Method = "POST";

                newStream = client.GetRequestStream();
                newStream.Write(encoding.GetBytes(request), 0, encoding.GetBytes(request).Length);
                newStream.Close();

                var response = (new StreamReader(client.GetResponse().GetResponseStream())).ReadToEnd();

                return new ResponseApi
                {
                    CodeError = 200,
                    Data = response
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
                    var response = await GetData(new RequestPayment
                    {
                        AutorizaEnvioEmail = "NO",
                        AutorizaEnvioSMS = "NO",
                        CodigoDepartamentoComprador = int.Parse(Utilities.GetConfiguration("CodeDepartmentBuyer")),
                        CodigoMunicipioComprador = int.Parse(Utilities.GetConfiguration("CodeTownBuyer")),
                        CodigoPaisComprador = int.Parse(Utilities.GetConfiguration("CodeCountryBuyer")),
                        DireccionComprador = transaction.payer.ADDRESS ?? string.Empty,
                        EmailComprador = transaction.payer.EMAIL ?? string.Empty,
                        IdentificacionComprador = transaction.payer.IDENTIFICATION,
                        MunicipioComprador = string.Empty,
                        NombreComprador = string.Concat(transaction.payer.NAME," ", transaction.payer.LAST_NAME),
                        PlataformaCliente = Utilities.GetConfiguration("ClientPlataform"),
                        PrimerApellidoComprador = transaction.payer.LAST_NAME,
                        PrimerNombreComprador = transaction.payer.NAME,
                        ReferenciaPago = transaction.IdTransactionAPi.ToString(),
                        SegundoApellidoComprador = string.Empty,
                        SegundoNombreComprador = string.Empty,
                        TelefonoComprador = transaction.payer.PHONE.ToString(),
                        TipoComprador = transaction.payer.TYPE_PAYER,
                        TipoIdentificacionComprador = transaction.payer.TYPE_IDENTIFICATION,
                        ValorCompra = Decimal.ToInt32(transaction.Amount),
                        Certificados = transaction.Products,
                        IdCliente = Utilities.GetConfiguration("IdClient")
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
                                    $"-{transaction.Enrollment}-{product.MatriculaEst ?? "0"}-{transaction.Tpcm}-{(i + 1).ToString()}";

                                var patchFile = string.Concat(basseAddress,
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
                if (transaction != null && !string.IsNullOrEmpty(transaction.consecutive))
                {
                    var response = await GetData(new CancelPayment
                    {
                        IdCliente = Utilities.GetConfiguration("IdClient"),
                        IdCompra = int.Parse(transaction.consecutive),
                        ValorCompra = transaction.Amount,
                        ReferenciaPago = transaction.IdTransactionAPi.ToString(),
                        PlataformaCliente = Utilities.GetConfiguration("ClientPlataform"),
                        Observaciones = string.Empty
                    }, "BuyCancel");

                    if (response.CodeError == 200)
                    {
                        transaction.State = ETransactionState.Cancel;
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
