using FloraEjemplo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Agenda : ContentPage
    {
        public Agenda()
        {
            InitializeComponent();
            this.BindingContext = new AgendaViewModel();
            btnNuevo.Clicked += BtnNuevo_Clicked;
            btnGuardar.Clicked += BtnGuardar_Clicked;
            btnCancelar.Clicked += BtnCancelar_Clicked;
        }
        //Metodo Cancelar, cancela el modal
        private void BtnCancelar_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private void BtnGuardar_Clicked(object sender, System.EventArgs e)
        {
            /*---------------------Mejorar-----------------------------------*/
            //Application.Current.MainPage.Navigation.PushAsync(new ListadoView());
            Application.Current.MainPage.Navigation.PushAsync(new Agenda());
            Application.Current.MainPage.Navigation.PopAsync();


        }
        //Borrar contenido del formulario
        private void BtnNuevo_Clicked(object sender, System.EventArgs e)
        {
            this.Nombre.Text = "";
            Edad.Text = "";
            Telefono.Text = "";
            Mail.Text = "";
            Saldo.Text = "";
            Usuario.Text = "";
        }
    }
}