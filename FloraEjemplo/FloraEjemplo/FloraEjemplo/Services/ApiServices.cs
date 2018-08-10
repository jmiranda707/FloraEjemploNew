using FloraEjemplo.Models;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FloraEjemplo.Services
{
    public class ApiServices
    {
        public async Task<Response> CheckConnection()
        {
            //verifica si esta activa la conexion a internet
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Por favor, revisa tu configuración de internet",
                };
            }
            //hace ping a google para saber si hay internet
            var isReachable = await CrossConnectivity.Current.IsRemoteReachable(
                "google.com");
            if (!isReachable)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "No tienes Conexion a Internet",
                };
            }

            return new Response
            {
                IsSuccess = true,
                Message = "OK",
            };
        }

        public async Task<Response> Get<T>(
            string controller)
        {
            try
            {
                var uuid = Application.Current.Properties["uuid"];

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                httpClient.DefaultRequestHeaders.Add("X-Odoo-dbfilter", "kidigram");
                httpClient.DefaultRequestHeaders.Add("UUID", uuid.ToString());
                HttpResponseMessage response = await httpClient.GetAsync("http://efrain1234-001-site1.ftempurl.com" + controller);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = response.Content.ReadAsStringAsync().Result;
                var json = JsonConvert.DeserializeObject<T>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = json,
                };
            }
            catch (System.Exception error)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = error.Message,
                };
            }
        }
    }
}
