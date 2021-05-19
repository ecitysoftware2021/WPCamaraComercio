using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WPFCCMedellin.Classes;
using WPFCCMedellin.DataModel;
using WPFCCMedellin.Services.Object;
using static WPFCCMedellin.Classes.LoginCancelTransaction;

namespace WPFCCMedellin.Services
{
    public class Api
    {
        private HttpClient client;

        private RequestAuth requestAuth;

        private static RequestApi requestApi;

        private string basseAddress;

        private int type = 1;

        private static string token;

        public Api()
        {
            if (requestAuth == null)
            {
                requestAuth = new RequestAuth();
            }

            if (requestApi == null)
            {
                requestApi = new RequestApi();
            }

            basseAddress = Utilities.GetConfiguration("basseAddress");
        }

        public async Task<ResponseAuth> GetSecurityToken(CONFIGURATION_PAYDAD config)
        {
            try
            {
                if (config != null)
                {
                    client = new HttpClient();
                    client.BaseAddress = new Uri(basseAddress);

                    requestAuth.UserName = config.USER;
                    requestAuth.Password = config.PASSWORD;
                    requestAuth.Type = this.type;

                    var request = JsonConvert.SerializeObject(requestAuth);
                    var content = new StringContent(request, Encoding.UTF8, "Application/json");
                    var url = Utilities.GetConfiguration("GetToken");

                    var authentication = Encoding.ASCII.GetBytes(config.USER_API + ":" + config.PASSWORD_API);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authentication));

                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    if (result != null)
                    {
                        var requestresponse = JsonConvert.DeserializeObject<ResponseAuth>(result);
                        if (requestresponse != null)
                        {
                            if (requestresponse.CodeError == 200)
                            {
                                token = requestresponse.Token;
                                requestApi.Session = Convert.ToInt32(requestresponse.Session);
                                requestApi.User = Convert.ToInt32(requestresponse.User);

                                return requestresponse;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<object> CallApi(string controller, object data = null)
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(basseAddress);

                requestApi.Data = data;

                var request = JsonConvert.SerializeObject(requestApi);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration(controller);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl(response.ReasonPhrase, "Error en el servicio", EError.Api, ELevelError.Medium);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponseApi>(result);

                if (responseApi.CodeError == 200)
                {
                    if (responseApi.Data == null)
                    {
                        return "OK";
                    }
                    return responseApi.Data;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static async Task<UserSession2> Login(string username, string password)
        {
            try
            {
                RequestApi requestApi2 = new RequestApi
                {
                    Session = requestApi.Session,
                    User = requestApi.User,
                    Data = new RequestAuthentication2
                    {
                        Password = password,
                        Type = 1,
                        UserName = username
                    }
                };
                ServicePointManager.Expect100Continue = false;
                var json = JsonConvert.SerializeObject(requestApi2);
                var content = new StringContent(json, Encoding.UTF8, "Application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(Utilities.GetConfiguration("basseAddress"));
                var url = "Users/Login";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.PostAsync(url, content).Result;
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<Response2>(result);
                if (responseApi.CodeError != 200)
                {
                    return null;
                }

                var user = JsonConvert.DeserializeObject<List<UserViewModel2>>(responseApi.Data.ToString()).FirstOrDefault();
                var userSession = new UserSession2
                {
                    CUSTOMER_ID = user.CUSTOMER_ID,
                    EMAIL = user.EMAIL,
                    IDENTIFICATION = user.IDENTIFICATION,
                    IMAGE = user.IMAGE,
                    NAME = user.NAME,
                    PASSWORD = user.PASSWORD,
                    PHONE = user.PHONE,
                    STATE = user.STATE,
                    USERNAME = user.USERNAME,
                    USER_ID = user.USER_ID,
                    Roles = new List<Role2>()
                    {
                        new Role2
                        {
                            DESCRIPTION = user.ROL_NAME,
                            ROLE_ID = user.ROLE_ID
                        }
                    },

                };

                return userSession;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
