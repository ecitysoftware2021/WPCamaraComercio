﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Objects;

namespace WPCamaraComercio.Service
{
    public class Api
    {
        #region References

        private HttpClient client;
        private HttpResponseMessage response;
        private Response result;
        private RequestAuth requestAuth;
        private string basseAddress;
        private string User4Told;
        private string Password4Told;

        #endregion

        #region LoadMethod

        public Api()
        {
            basseAddress = Utilities.GetConfiguration(nameof(basseAddress));
            client = new HttpClient();
            client.BaseAddress = new Uri(basseAddress);
            ReadKeys();
        }

        #endregion

        public async Task<bool> SecurityToken()
        {
            try
            {
                var request = JsonConvert.SerializeObject(requestAuth);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                client = new HttpClient();
                client.BaseAddress = new Uri(Utilities.GetConfiguration("basseAddress"));
                var url = Utilities.GetConfiguration("");
                var authentication = Encoding.ASCII.GetBytes(User4Told + ":" + Password4Told);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authentication));
                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var result = await response.Content.ReadAsStringAsync();
                if (result != null)
                {
                    var requestresponse = JsonConvert.DeserializeObject<ResponseAuth>(result);
                    if (requestresponse != null)
                    {
                        if (requestresponse.CodeError == 200)
                        {
                            Utilities.TOKEN = requestresponse.Token;
                            Utilities.CorrespondentId = (int)requestresponse.User;
                            Utilities.Session = (int)requestresponse.Session;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ReadKeys()
        {
            string[] text = File.ReadAllLines(@"C:\Users\Sebastian Betancur\Desktop\keys.txt");
            if (text.Length > 0)
            {
                string[] line1 = text[0].Split(';');
                User4Told = line1[0].Split(':')[1];
                Password4Told = line1[1].Split(':')[1];

                string[] line2 = text[1].Split(';');
                requestAuth = new RequestAuth
                {
                    User = line2[0].Split(':')[1],
                    Password = line2[1].Split(':')[1],
                    Type = int.Parse(line2[2].Split(':')[1])
                };
            }
        }
    }
}