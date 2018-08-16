using FloraEjemplo.Data;
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
                    Message = "Checha tu conexión a internet",
                };
            }

            return new Response
            {
                IsSuccess = true,
                Message = "OK",
            };
        }

        public async Task<Response> CheckChanges()
        {
            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<ClienteModel> clienteModel = new List<ClienteModel>(contexto.Consultar());
                List<ClienteTrackingModel> clienteTrackingModel = new List<ClienteTrackingModel>(contexto.ConsultarClienteRegistro());
                if (clienteTrackingModel.Count != 0)
                {
                    List<ClienteTrackingModel> modeloRegistro = new List<ClienteTrackingModel>(contexto.ConsultarCambios());
                    var json = JsonConvert.SerializeObject(modeloRegistro);
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsync("http://efrain1234-001-site1.ftempurl.com/api/SyncIn", new StringContent(json, Encoding.UTF8, "application/json"));
                    if (!response.IsSuccessStatusCode)
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = response.RequestMessage.ToString(),
                        };
                    }

                    string jsonValidacion = response.Content.ReadAsStringAsync().Result;
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Cambios pendientes enviados",
                        Result = jsonValidacion
                    };
                }
                else
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Sin cambios pendientes",
                    };
                }
            }
        }

        public async Task<Response> LoadClientFronApi()
        {
            try
            {
                string resultado = string.Empty;
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                HttpResponseMessage response = await httpClient.GetAsync("http://efrain1234-001-site1.ftempurl.com/api/Cliente");
                var result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                resultado = response.Content.ReadAsStringAsync().Result;
                resultado = resultado.Replace("\\", "");
                resultado = resultado.Replace("/", "");
                resultado = resultado.Replace("\"[", "[");
                resultado = resultado.Replace("]\"", "]");

                var resulta = resultado;

                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = resultado,
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
