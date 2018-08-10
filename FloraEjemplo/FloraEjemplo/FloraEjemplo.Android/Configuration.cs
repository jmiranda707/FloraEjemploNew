using FloraEjemplo.Interfaces;
using SQLite.Net.Interop;
using Xamarin.Forms;

[assembly: Dependency(typeof(FloraEjemplo.Droid.Configurations.Configuration))]
namespace FloraEjemplo.Droid.Configurations
{
    public class Configuration : IConfiguracion
    {
        private string Directorio;

        private ISQLitePlatform Plataforma;

        string IConfiguracion.directorio
        {
            get
            {
                if (string.IsNullOrEmpty(Directorio))
                {
                    Directorio = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                }
                return Directorio;
            }
        }

        ISQLitePlatform IConfiguracion.plataforma
        {
            get
            {
                if (Plataforma == null)
                {
                    Plataforma = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
                }
                return Plataforma;
            }

        }
    }
}