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
            var item = e.SelectedItem as ClienteRegistro;

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<Cliente2> modelo = new List<Cliente2>(contexto.Consultar());
                var TablaClientes = modelo;
                List<ClienteRegistro> modelo2 = new List<ClienteRegistro>(contexto.ConsultarClienteRegistro());
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