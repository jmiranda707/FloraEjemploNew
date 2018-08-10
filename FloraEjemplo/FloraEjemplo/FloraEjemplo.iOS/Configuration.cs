using FloraEjemplo.Interfaces;
using SQLite.Net.Interop;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(FloraEjemplo.iOS.Configurations.Configuration))]
namespace FloraEjemplo.iOS.Configurations
{
    public class Configuration : IConfiguracion
    {
        public string Directorio;

        public ISQLitePlatform Plataforma;

        public string directorio
        {
            get
            {
                if (string.IsNullOrEmpty(Directorio))
                {
                    var dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    Directorio = Path.Combine(dir, "..", "Library");
                }
                return Directorio;
            }
        }

        public ISQLitePlatform plataforma
        {
            get
            {
                if (Plataforma == null)
                {
                    Plataforma = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
                }
                return Plataforma;
            }
        }
    }
}