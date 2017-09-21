﻿using Client.Desktop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client.Desktop.ApiOperations
{
    class Authorization
    {
        const string baseUrl = "http://localhost:5000/api";

        public ResponseJWTFormat AuthenticateUser(string login, string password)
        {
            string endpoint = baseUrl + "/token/login";
            string method = "POST";
            string json = JsonConvert.SerializeObject(new
            {
                login = login,
                password = password
            });

            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";
            try
            {
                string response = wc.UploadString(endpoint, method, json);
                return JsonConvert.DeserializeObject<ResponseJWTFormat>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ResponseJWTFormat RegisterAccount(string login, string password)
        {
            string endpoint = baseUrl + "/token/registration";
            string method = "POST";
            string json = JsonConvert.SerializeObject(new
            {
                login = login,
                password = password
            });

            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";
            try
            {
                string response = wc.UploadString(endpoint, method, json);
                return JsonConvert.DeserializeObject<ResponseJWTFormat>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}