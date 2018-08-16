using System;
using Xamarin.Forms;
using Android.Content;
using FloraEjemplo.Interfaces;
using FloraEjemplo.Droid;

[assembly: Dependency(typeof(OpenWifiSettingsHelper))]
namespace FloraEjemplo.Droid
{
    public class OpenWifiSettingsHelper : IOpenWifiSettings
    {
        /// <summary>
        /// Opens wifi settings on this Android device
        /// </summary>
        public void OpenWifiSettings()
        {
            try
            {
                using (var intent = new Intent(Android.Provider.Settings.ActionWifiSettings))
                {
                    Forms.Context.StartActivity(intent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new InvalidOperationException("Opening Wifi settings was not possible");
            }
        }
    }
}