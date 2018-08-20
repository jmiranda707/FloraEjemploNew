﻿using FloraEjemplo.Data;
using FloraEjemplo.Models;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FloraEjemplo.Services
{
    public class ApiServices
    {
        DataContext dataContext;
        private DateTimeOffset _lastSync = DateTimeOffset.MinValue;

        public ApiServices()
        {
            dataContext = new DataContext();
        }

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
            //var isReachable = await CrossConnectivity.Current.IsRemoteReachable(
            //    "www.google.com.ve");
            var isReachable = IsReachableUri();
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
                    //var respuesta = response.Headers.Location.ToString();
                    if (!response.IsSuccessStatusCode)
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = response.RequestMessage.ToString(),
                            Codigo = 500
                        };
                    }

                    string jsonValidacion = response.Content.ReadAsStringAsync().Result;
                    var jsonRecibe = JsonConvert.DeserializeObject<List<ResponseChanges>>(jsonValidacion);

                    dataContext.DeleteAllClienteRegistro();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Cambios pendientes enviados",
                        Result = jsonValidacion,
                        Codigo = 201
                    };
                }
                else
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Sin cambios pendientes",
                        Codigo = 200
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

                Application.Current.Properties["LastUpdated"] = DateTime.Now;
                await Application.Current.SavePropertiesAsync();
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

        public async Task<Response> Sincronizacion()
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

        public static bool IsReachableUri()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com");
            request.Timeout = 15000;
            request.Method = "HEAD"; // As per Lasse's comment
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
