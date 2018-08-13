using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using FloraEjemplo.Views;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
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
            MainPage = new NavigationPage(new ListaClientesMD());

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Task.Run(async () =>
                {
                    //var time = await RequestTimeAsync();
                    // do something with time...
                    var connection = await apiServices.CheckConnection();
                    if (!connection.IsSuccess)
                    {
                        ConsultaTablas();
                    }
                });
                return true;
            });
        }

        async void ConsultaTablas()
        {
            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<Cliente2> modelo = new List<Cliente2>(contexto.Consultar());
                var TablaClientes = modelo;
                List<ClienteRegistro> modelo2 = new List<ClienteRegistro>(contexto.ConsultarClienteRegistro());
                var TablaClientesRegistro = modelo2;

                if (modelo.Count != modelo2.Count)
                {
                    List<ClienteRegistro> modeloRegistro = new List<ClienteRegistro>(contexto.ConsultarCambios());
                    var Cambios = modeloRegistro;
                }
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
