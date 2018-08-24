using FloraEjemplo.Data;
using FloraEjemplo.Interfaces;
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

        //private DateTimeOffset _lastSync = DateTimeOffset.MinValue;

        public SyncIn TablaSyncIn { get; set; }

        public ApiServices()
        {
            dataContext = new DataContext();
        }

        //Revisa si hay conexion a internet
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

        //Revisa si hay cambios en DB local y los envía
        public async Task<Response> CheckChanges()
        {
            try
            {
                using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
                {
                    List<ClienteTrackingModel> clienteTrackingModel = new List<ClienteTrackingModel>(contexto.ConsultarClienteRegistro());
                    if (clienteTrackingModel.Count != 0)
                    {
                        List<ClienteTrackingModel> modeloRegistro = new List<ClienteTrackingModel>(contexto.ConsultarCambios());
                        var json = JsonConvert.SerializeObject(modeloRegistro);
                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("http://efrain1234-001-site1.ftempurl.com/api/SyncRegistro/", new StringContent(json, Encoding.UTF8, "application/json"));
                        var respuesta = response.Headers.Location.ToString();
                        var respuestaOcupado = "http://efrain1234-001-site1.ftempurl.com/api/SyncRegistro/-109";
                        string jsonValidacion = response.Content.ReadAsStringAsync().Result;
                        //var jsonRecibe = JsonConvert.DeserializeObject<List<ResponseChanges>>(jsonValidacion);
                        var syncIn = JsonConvert.DeserializeObject<List<SyncIn>>(jsonValidacion);
                        Application.Current.Properties["Version"] = syncIn[0].Version.ToString();
                        await Application.Current.SavePropertiesAsync();
                        if (!response.IsSuccessStatusCode)
                        {
                            return new Response
                            {
                                IsSuccess = false,
                                Message = response.RequestMessage.ToString(),
                                Codigo = 500
                            };
                        }
                        else
                        {
                            if (respuesta == respuestaOcupado)
                            {
                                return new Response
                                {
                                    IsSuccess = false,
                                    Message = "Servidor ocupado, intente de nuevo",
                                    Result = jsonValidacion,
                                    Codigo = 109
                                };
                            }
                        }

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
            catch (Exception error)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = error.Message.ToString(),
                    Codigo = 500
                };
            }
        }

        //Obtiene lista de clientes
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
                        Result = response.RequestMessage.ToString()
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
        //
        public async Task<Response> Sincronizacion()
        {
            IDevice device = DependencyService.Get<IDevice>();
            string deviceIdentifier = device.GetIdentifier();
            var Tu_NombreUsuario = Application.Current.Properties["Usuario"] as string;
            var version = Application.Current.Properties["Version"] as string;
            var Tu_Identificador = deviceIdentifier;
            try
            {
                string resultado = string.Empty;
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                HttpResponseMessage response = await httpClient.GetAsync(
                    "http://efrain1234-001-site1.ftempurl.com/api/SyncSeleccion?Usuario=" + Tu_NombreUsuario + "&Dispositivo=" + Tu_Identificador + "&Version=" + version);
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
        //Hace ping a Google para verificar si en realidad hay conexion a internet
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
        public async Task<Response> SincronizacionVoid()
        {
            IDevice device = DependencyService.Get<IDevice>();
            string deviceIdentifier = device.GetIdentifier();
            var Tu_NombreUsuario = Application.Current.Properties["Usuario"] as string;
            var version = Application.Current.Properties["Version"] as string;
            var Tu_Identificador = deviceIdentifier;
            try
            {
                string resultado = string.Empty;
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                HttpResponseMessage response = await httpClient.GetAsync(
                    "http://efrain1234-001-site1.ftempurl.com/api/SyncObtener?Usuario="+Tu_NombreUsuario+"&Dispositivo="+Tu_Identificador+"&Version="+version);
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
