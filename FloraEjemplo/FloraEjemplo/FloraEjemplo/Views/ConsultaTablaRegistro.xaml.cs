using FloraEjemplo.Data;
using FloraEjemplo.Models;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConsultaTablaRegistro : ContentPage
	{
        private double width = 0;
        private double height = 0;

        public ConsultaTablaRegistro ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            ListClientes.ItemSelected += ListClientes_ItemSelected;
		}

        async void ListClientes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as ClienteTrackingModel;

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<ClienteModel> modelo = new List<ClienteModel>(contexto.Consultar());
                var TablaClientes = modelo;
                List<ClienteTrackingModel> modelo2 = new List<ClienteTrackingModel>(contexto.ConsultarClienteRegistro());
                var TablaClientesRegistro = modelo2;

                if (modelo.Count != modelo2.Count)
                {
                    await Application.Current.MainPage.DisplayAlert("Transaccion", "No hay cambios en la tabla", "Ok");
                }

                var transaccion = item.Edad.ToString();
                await Application.Current.MainPage.DisplayAlert("Transaccion", transaccion, "Ok");

            }
        }
        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<ConsultaTablaRegistro>(this, "preset");
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
                        Hamb.IsVisible = true;
                    }
                    else
                    {
                        Hamb.IsVisible = true;
                    }
                }
                else
                {
                    if (width > height)
                    {
                        Hamb.IsVisible = false;
                    }
                    else
                    {
                        Hamb.IsVisible = true;
                    }
                }
            }
        }
    }
}