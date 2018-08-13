using FloraEjemplo.Data;
using FloraEjemplo.Models;
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
    public partial class ModificarContacto : ContentPage
    {
        public ModificarContacto(Cliente modelo)
        {
            InitializeComponent();
            BindingContext = modelo;
            btnVolver.Clicked += BtnVolver_Clicked;
            btnModificar.Clicked += BtnModificar_Clicked;
            //se ingreso el metodo BtnModificar_Clicked dentro para hacer
            //uso del parametro (AgendaModel modelo) que viene desde listado
            //ya que trae el objeto seleccionado
            void BtnModificar_Clicked(object sender, System.EventArgs e)
            {
                using (var contexto = new DataContext())
                {
                    contexto.Actualizara(modelo);
                }
                Navigation.PopModalAsync();
                //Navigation.PushAsync(new ListadoView());
                //var listadoView = new ListadoView();
                //listadoView.Navigation.PopModalAsync();
            }
        }
        private void BtnVolver_Clicked(object sender, System.EventArgs e)
        {

            Navigation.PopModalAsync();
        }
    }
}