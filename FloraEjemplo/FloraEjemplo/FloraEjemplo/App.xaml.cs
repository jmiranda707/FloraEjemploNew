using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using FloraEjemplo.ViewModels;
using FloraEjemplo.Views;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
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

        //event ConnectivityChangedEventHandler ConnectivityChanged;

        //public delegate void ConnectivityChangedEventHandler(object sender, ConnectivityChangedEventArgs e);

        public App()
        {
            InitializeComponent();
            apiServices = new ApiServices();
            MainPage = new NavigationPage(new ListaClientesMD());
        }


        protected override void OnStart()
        {


        }

        protected override void OnSleep()
        {
            Device.StartTimer(TimeSpan.FromSeconds(150), () =>
            {
                Task.Run(async () =>
                {
                    var connection = await apiServices.CheckConnection();
                    if (connection.IsSuccess)
                    {
                        //ConsultaTablas();
                        var cambiosPendientes = await apiServices.CheckChanges();
                        if (cambiosPendientes.Codigo == 201)
                        {
                            MessagingCenter.Send<App>(this, "EjecutaLista");
                        }
                    }
                });
                return true;
            });
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Device.StartTimer(TimeSpan.FromSeconds(160), () =>
            {
                Task.Run(async () =>
                {
                    var connection = await apiServices.CheckConnection();
                    if (connection.IsSuccess)
                    {
                        //ConsultaTablas();
                        var cambiosPendientes = await apiServices.CheckChanges();
                        if (cambiosPendientes.Codigo == 201)
                        {
                            MessagingCenter.Send<App>(this, "EjecutaLista");
                        }
                    }
                });
                return true;
            });
        }
    }
}
//public class ConnectivityChangedEventArgs : EventArgs
//{
//    public bool IsConnected { get; set; }
//}
