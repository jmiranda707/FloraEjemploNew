
using FloraEjemplo.Interfaces;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(FloraEjemplo.iOS.CloseApplication))]
namespace FloraEjemplo.iOS
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}