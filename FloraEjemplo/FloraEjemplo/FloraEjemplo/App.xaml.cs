using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using FloraEjemplo.Views;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FloraEjemplo
{
    public partial class App : Application
    {
        public static MasterDetailPage MasterD { get; set; }

        private ApiServices apiServices;

        public App()
        {
            InitializeComponent();
            apiServices = new ApiServices();
            //ConsultaCambios();
            MainPage = new NavigationPage(new ListaClientesMD());

            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                Task.Run(async () =>
                {
                    //var time = await RequestTimeAsync();
                    // do something with time...
                    var connection = await apiServices.CheckConnection();
                    if (connection.IsSuccess)
                    {
                        //ConsultaTablas();
                        var cambiosPendientes = await apiServices.CheckChanges();
                    }
                });
                return true;
            });
        }

        //async void ConsultaCambios()
        //{
        //    var connection = await apiServices.CheckConnection();
        //    if (connection.IsSuccess)
        //    {
        //        //ConsultaTablas();
        //    }
        //}

        //async void ConsultaTablas()
        //{
        //    using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
        //    {
        //        List<ClienteModel> clienteModel = new List<ClienteModel>(contexto.Consultar());
        //        List<ClienteTrackingModel> clienteTrackingModel = new List<ClienteTrackingModel>(contexto.ConsultarClienteRegistro());
        //        if (clienteTrackingModel.Count != 0)
        //        {
        //            List<ClienteTrackingModel> modeloRegistro = new List<ClienteTrackingModel>(contexto.ConsultarCambios());
        //            var json = JsonConvert.SerializeObject(modeloRegistro);
        //            HttpClient client = new HttpClient();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            HttpResponseMessage response = await client.PostAsync("http://efrain1234-001-site1.ftempurl.com/api/SyncIn", new StringContent(json, Encoding.UTF8, "application/json"));
        //            if (!response.IsSuccessStatusCode)
        //            {
                        
        //            }

        //            string jsonValidacion = response.Content.ReadAsStringAsync().Result;
        //        }
        //    }
        //}

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Task.Run(async () =>
                {
                    var connection = await apiServices.CheckConnection();
                    if (connection.IsSuccess)
                    {
                        var cambiosPendientes = await apiServices.CheckChanges();
                    }
                });
                return true;
            });
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }
}
