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

        private ListClientesViewModel listaClientes;

        event ConnectivityChangedEventHandler ConnectivityChanged;

        public delegate void ConnectivityChangedEventHandler(object sender, ConnectivityChangedEventArgs e);

        public App()
        {
            InitializeComponent();
            apiServices = new ApiServices();
            listaClientes = new ListClientesViewModel();
            MainPage = new NavigationPage(new ListaClientesMD());
            //CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
            //{
            //    var cambiosPendientes = await apiServices.CheckChanges();
            //    if (cambiosPendientes.Codigo == 201)
            //    {
            //        MessagingCenter.Send<App>(this, "EjecutaLista");
            //    }
            //};
            //Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            //{
            //    Task.Run(async () =>
            //    {
            //        var connection = await apiServices.CheckConnection();
            //        if (connection.IsSuccess)
            //        {
            //            //ConsultaTablas();
            //            var cambiosPendientes = await apiServices.CheckChanges();
            //            if (cambiosPendientes.Codigo == 201)
            //            {
            //                MessagingCenter.Send<App>(this, "EjecutaLista");
            //            }
            //        }
            //    });
            //    return true;
            //});
        }


    protected override void OnStart()
        {
            // Handle when your app starts
            //CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            //{
            //    Task.Run(async () =>
            //    {
            //        var connection = await apiServices.CheckConnection();
            //        if (connection.IsSuccess)
            //        {
            //            //ConsultaTablas();
            //            var cambiosPendientes = await apiServices.CheckChanges();
            //            if (cambiosPendientes.Codigo == 201)
            //            {
            //                MessagingCenter.Send<App>(this, "EjecutaLista");
            //            }
            //        }
            //    });
            //};
        }

        protected override void OnSleep()
        {
            //CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            //{
            //    Task.Run(async () =>
            //    {
            //        var connection = await apiServices.CheckConnection();
            //        if (connection.IsSuccess)
            //        {
            //            //ConsultaTablas();
            //            var cambiosPendientes = await apiServices.CheckChanges();
            //            if (cambiosPendientes.Codigo == 201)
            //            {
            //                MessagingCenter.Send<App>(this, "EjecutaLista");
            //            }
            //        }
            //    });
            //};
            //Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            //{
            //    Task.Run(async () =>
            //    {
            //        var connection = await apiServices.CheckConnection();
            //        if (connection.IsSuccess)
            //        {
            //            var cambiosPendientes = await apiServices.CheckChanges();
            //            if (cambiosPendientes.Codigo == 201)
            //            {
            //                MessagingCenter.Send<App>(this, "EjecutaLista");
            //            }
            //        }
            //    });
            //    return true;
            //});
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }
}
public class ConnectivityChangedEventArgs : EventArgs
{
    public bool IsConnected { get; set; }
}
