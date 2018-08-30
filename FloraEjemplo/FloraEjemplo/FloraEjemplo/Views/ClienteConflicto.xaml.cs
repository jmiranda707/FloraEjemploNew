using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ClienteConflicto : ContentPage
	{
        private double width = 0;
        private double height = 0;
        ApiServices apiServices;

        public ClienteConflicto ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            apiServices = new ApiServices();
            ListClientes.ItemSelected += ListClientes_ItemSelected;
        }
        async void ListClientes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as ClientsConflicts;
            Application.Current.Properties["Id"] = item.Id.ToString();
            Application.Current.Properties["Correo"] = item.Mail.ToString();
            Application.Current.Properties["Numero"] = item.Numero.ToString();
            await Application.Current.SavePropertiesAsync();
            var numero = item.Numero.ToString();
            var version = Application.Current.Properties["VersionNew"] as string;
            string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Eliminar");
            if (action == "Eliminar")
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await Application.Current.MainPage.DisplayAlert(
                        "Alerta!",
                        "Desea eliminar el cliente seleccionado?",
                        "Si", "No");

                    if (result)
                    {
                        //Eliminamos en local ClienteModel
                        using (var contexto = new DataContext())
                        {
                            ClientsConflicts modelo = (ClientsConflicts)e.SelectedItem;
                            contexto.EliminarClienteConflicto(modelo);
                        }
                        var connection = await apiServices.CheckConnection();
                    }
                });

                await Application.Current.MainPage.DisplayAlert(
                                   "Hecho",
                                   "Cliente eliminado",
                                   "Aceptar");

                MessagingCenter.Send<ClienteConflicto>(this, "ListaConflicto");
            }
        }
        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<ClienteConflicto>(this, "preset");
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    if (width > height)
                    {
                        barraNavegacion.HeightRequest = 300;
                        grilla.HeightRequest = 300;
                        Hamb.IsVisible = true;
                    }
                    else
                    {
                        barraNavegacion.HeightRequest = 180;
                        grilla.HeightRequest = 180;
                        Hamb.IsVisible = true;
                    }
                }
                else
                {
                    if (width > height)
                    {
                        barraNavegacion.HeightRequest = 130;
                        grilla.HeightRequest = 130;
                        Hamb.IsVisible = false;
                    }
                    else
                    {
                        barraNavegacion.HeightRequest = 130;
                        grilla.HeightRequest = 130;
                        Hamb.IsVisible = true;
                    }
                }
            }
        }
    }
}