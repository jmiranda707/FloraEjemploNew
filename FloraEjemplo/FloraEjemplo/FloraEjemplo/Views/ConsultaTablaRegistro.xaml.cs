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
		public ConsultaTablaRegistro ()
		{
			InitializeComponent ();
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
    }
}