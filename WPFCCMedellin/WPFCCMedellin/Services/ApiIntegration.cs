using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
        private HttpClient client;

        private string basseAddress;

        public ApiIntegration()
        {
            basseAddress = Encryptor.Decrypt(Utilities.GetConfiguration("basseAddressIntegration", true));
        }

        public async Task<ResponseApi> GetData(object requestData, string controller)
        {
            try
            {
                client = new HttpClient();
                var request = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                client.BaseAddress = new Uri(basseAddress);
                var url = Utilities.GetConfiguration(controller);

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                return new ResponseApi
                {
                    CodeError = 200,
                    Data = result
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
                    var response = await GetData(new RequestApi
                    {
                        Data = new RequestPayment
                        {
                            AutorizaEnvioEmail = "NO",
                            AutorizaEnvioSMS = "NO",
                            CodigoDepartamentoComprador = int.Parse(Utilities.GetConfiguration("CodeDepartmentBuyer")),
                            CodigoMunicipioComprador = int.Parse(Utilities.GetConfiguration("CodeTownBuyer")),
                            CodigoPaisComprador = int.Parse(Utilities.GetConfiguration("CodeCountryBuyer")),
                            DireccionComprador = transaction.payer.ADDRESS,
                            EmailComprador = transaction.payer.EMAIL,
                            IdentificacionComprador = transaction.payer.IDENTIFICATION,
                            MunicipioComprador = string.Empty,
                            NombreComprador = string.Concat(transaction.payer.NAME, transaction.payer.LAST_NAME),
                            PlataformaCliente = Utilities.GetConfiguration("ClientPlataform"),
                            PrimerApellidoComprador = transaction.payer.LAST_NAME,
                            PrimerNombreComprador = transaction.payer.NAME,
                            ReferenciaPago = transaction.IdTransactionAPi.ToString(),
                            SegundoApellidoComprador = string.Empty,
                            SegundoNombreComprador = string.Empty,
                            TelefonoComprador = transaction.payer.PHONE.ToString(),
                            TipoComprador = transaction.payer.TYPE_PAYER,
                            TipoIdentificacionComprador = transaction.payer.TYPE_IDENTIFICATION,
                            ValorCompra = transaction.Amount,
                            Certificados = transaction.Products,
                        },
                    }, "SendPay");

                    if (response.CodeError == 200)
                    {
                        var result = JsonConvert.DeserializeObject<ResponsePay>(response.Data.ToString());

                        if (result.IsSuccess)
                        {
                            transaction.consecutive = result.Result;
                            transaction.State = ETransactionState.Success;
                            return transaction;
                        }
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

                foreach (var product in transaction.Products)
                {
                    var certificate = new CLSDatosCertificado
                    {
                        IdCertificado = product.IdCertificado,
                        idcompra = transaction.consecutive,
                        matricula = transaction.Enrollment,
                        matriculaest = product.MatriculaEst,
                        referenciaPago = transaction.IdTransactionAPi.ToString(),
                        tpcm = transaction.Tpcm
                    };

                    for (int i = 0; i < int.Parse(product.NumeroCertificados); i++)
                    {
                        countCertificates += 1;
                        certificate.copia = (i + 1).ToString();

                        var response = await GetData(new RequestApi
                        {
                            Data = certificate
                        }, "GetCertifiedString");

                        if (response.CodeError == 200 && response.Data != null)
                        {
                            var result = JsonConvert.DeserializeObject<ResponsePay>(response.Data.ToString());

                            if (!string.IsNullOrEmpty(result.Result))
                            {
                                var nameFile = $"{transaction.consecutive}-{certificate.IdCertificado}" +
                                    $"-{certificate.matricula}-{certificate.matriculaest}-{transaction.Tpcm}-{(i + 1).ToString()}";
                                string path = DownloadFile(result.Result, nameFile);
                                if (!string.IsNullOrEmpty(path))
                                {
                                    pathCertificates.Add(path);
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

        public string DownloadFile(string patchFile, string nameFile)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                var response = myWebClient.DownloadData(patchFile);
                if (response != null)
                {
                    var path = Utilities.SaveFile(nameFile, Utilities.GetConfiguration("DirectoryFile"), response);
                    if (!string.IsNullOrEmpty(path))
                    {
                        return path;
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
