﻿using FloraEjemplo.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListaClientes : ContentPage
    {
        private double width = 0;
        private double height = 0;

        public ListaClientes()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            ListClientes.ItemSelected += ListClientes_ItemSelected;
        }
        async void ListClientes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as Cliente2;
            Application.Current.Properties["Id"] = item.Id.ToString();
            await Application.Current.SavePropertiesAsync();
            string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Editar", "Eliminar", "Ver");
            if (action == "Eliminar")
            {
                //using (var contexto = new DataContext())
                //{
                //    Cliente modelo = (Cliente)e.SelectedItem;
                //    contexto.Eliminar(modelo);
                //}
                Delete();
            }
            else if (action == "Editar")
            {
                await Navigation.PushAsync(new EditarClienteMD());
            }
            else if (action == "Ver")
            {
                await Navigation.PushModalAsync(new VerClienteMD());
            }
        }
        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<ListaClientes>(this, "preset");
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
        private void Delete()
        {
            var idCliete = Application.Current.Properties["Id"] as string;
            if (idCliete == string.Empty) return;
            EnviarDocumentoDelete(idCliete);
        }
        public async void EnviarDocumentoDelete(string Id)
        {
            string urlValidacion = string.Empty;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.DeleteAsync("http://efrain1234-001-site1.ftempurl.com/api/EliminarCliente/" + Id);

            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert(
                    response.IsSuccessStatusCode.ToString(),
                    response.RequestMessage.ToString(),
                    "Aceptar");
            }
            await Application.Current.MainPage.DisplayAlert(
                    "Hecho",
                    "Cliente eliminado",
                    "Aceptar");
        }
    }
}