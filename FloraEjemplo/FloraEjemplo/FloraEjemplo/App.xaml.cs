using FloraEjemplo.Services;
using FloraEjemplo.Views;
using Plugin.Connectivity;
using System;
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

            //Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            //{
            //    Task.Run(async () =>
            //    {
            //        //var time = await RequestTimeAsync();
            //        // do something with time...
            //        //var result = await apiServices.CheckConnection();

            //    });
            //    return true;
            //});
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
