using Android.App;
using FloraEjemplo.Interfaces;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(FloraEjemplo.Droid.CloseApplication))]
namespace FloraEjemplo.Droid
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            var activity = (Activity)Forms.Context;
            activity.FinishAffinity();
        }
    }
}