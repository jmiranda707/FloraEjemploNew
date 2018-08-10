using FloraEjemplo.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerContacto : ContentPage
    {
        public VerContacto(Cliente modelo)
        {
            InitializeComponent();
            BindingContext = modelo;
            regresarbtn.Clicked += Regresarbtn_Clicked;
        }

        private void Regresarbtn_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopModalAsync();
        }

    }
}