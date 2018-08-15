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

            //Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            //{
            //    Task.Run(async () =>
            //    {
            //        //var time = await RequestTimeAsync();
            //        // do something with time...
            //        var connection = await apiServices.CheckConnection();
            //        if (connection.IsSuccess)
            //        {
            //            ConsultaTablas();
            //        }
            //    });
            //    return true;
            //});
        }

        async void ConsultaCambios()
        {
            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)
            {
                //ConsultaTablas();
            }
        }

        //async void ConsultaTablas()
        //{
        //    using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
        //    {
        //        List<ClienteOriginal> modelo = new List<ClienteOriginal>(contexto.Consultar());
        //        List<ClienteTracking> modelo2 = new List<ClienteTracking>(contexto.ConsultarClienteRegistro());

        //        if (modelo.Count != modelo2.Count)
        //        {
        //            List<ClienteTracking> modeloRegistro = new List<ClienteTracking>(contexto.ConsultarCambios());
        //            var json = JsonConvert.SerializeObject(modeloRegistro);
        //            HttpClient client = new HttpClient();
        //            //client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            HttpResponseMessage response = await client.PutAsync("http://efrain1234-001-site1.ftempurl.com/api/SyncIn", new StringContent(json, Encoding.UTF8, "application/json"));
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                await Application.Current.MainPage.DisplayAlert(
        //                    response.IsSuccessStatusCode.ToString(),
        //                    response.RequestMessage.ToString(),
        //                    "Aceptar");
        //            }
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
                    //var time = await RequestTimeAsync();
                    // do something with time...
                    var connection = await apiServices.CheckConnection();
                    if (connection.IsSuccess)
                    {
                        //ConsultaTablas();
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
