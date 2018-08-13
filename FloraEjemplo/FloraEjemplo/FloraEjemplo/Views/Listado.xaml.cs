using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Listado : ContentPage
    {
        public Listado()
        {
            InitializeComponent();
            this.BindingContext = new ListViewModel();
            btnAgregar.Clicked += BtnAgregar_Clicked;
            LvContactos.ItemSelected += LvContactos_ItemSelected;
        }
        //evento Agregar
        private void BtnAgregar_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new Agenda());
        }

        //Evento ItemSelected
        //Muestra opciones para eliminar, editar y ver
        async void LvContactos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Editar", "Eliminar", "Ver");
            if (action == "Eliminar")
            {
                using (var contexto = new DataContext())
                {
                    Cliente modelo = (Cliente)e.SelectedItem;
                    contexto.Eliminara(modelo);
                }
            }
            else if (action == "Editar")
            {
                Cliente modelo = (Cliente)e.SelectedItem;
                await Navigation.PushModalAsync(new ModificarContacto(modelo));
            }
            else if (action == "Ver")
            {
                Cliente modelo = (Cliente)e.SelectedItem;
                await Navigation.PushModalAsync(new VerContacto(modelo));
            }
        }
    }
}