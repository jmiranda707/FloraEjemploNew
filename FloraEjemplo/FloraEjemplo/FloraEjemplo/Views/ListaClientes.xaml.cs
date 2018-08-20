using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using System;
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
        private ApiServices apiServices;

        public ListaClientes()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            ListClientes.ItemSelected += ListClientes_ItemSelected;
            apiServices = new ApiServices();
        }
        async void ListClientes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as ClienteModel;
            Application.Current.Properties["Id"] = item.Id.ToString();
            Application.Current.Properties["Correo"] = item.Mail.ToString();
            Application.Current.Properties["Numero"] = item.Numero.ToString(); 
            //Application.Current.Properties["ClientId"] = item.ClientId.ToString(); 
            await Application.Current.SavePropertiesAsync();
            var numero = Application.Current.Properties["Numero"];
            //var clientId = Application.Current.Properties["ClientId"];
            string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Editar", "Eliminar", "Ver");
            if (action == "Eliminar")
            {
                var connection = await apiServices.CheckConnection();
                //Si hay conexion
                if (connection.IsSuccess)
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
                        return;
                    }
                    using (var contexto = new DataContext())
                    {
                        ClienteModel modelo = (ClienteModel)e.SelectedItem;
                        contexto.Eliminar(modelo);
                    }

                    await Application.Current.MainPage.DisplayAlert(
                            "Hecho",
                            "Cliente eliminado",
                            "Aceptar");

                    MessagingCenter.Send<ListaClientes>(this, "EjecutaLista");
                }
                else
                {
                    //Si no hay conexion
                    using (var contexto = new DataContext())
                    {
                        ClienteModel modelo = (ClienteModel)e.SelectedItem;
                        //Borramos en ClienteModel
                        contexto.Eliminar(modelo);
                        var activo = "ELIMINADO";
                        var actualizaEstado = "ACTUALIZAR_ESTADO";
                        //Borramos en cliente tracking
                        ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                        {
                            //ClientId = Convert.ToInt32(clientId),
                            Numero = Convert.ToInt32(numero),
                            Nombre = modelo.Nombre.ToString(),
                            Edad = modelo.Edad,
                            Telefono = modelo.Telefono.ToString(),
                            Mail = modelo.Mail.ToString(),
                            Saldo = modelo.Saldo,
                            Proceso = 1,
                            Usuario = modelo.Usuario,
                            FechaCreacion = modelo.FechaCreacion,
                            FechaCreacionUtc = modelo.FechaCreacionUtc,
                            FechaModificacion = DateTime.Now,
                            FechaModificacionUtc = DateTime.UtcNow,
                            Id = modelo.Id,
                            Estado = activo,
                            Transaccion = actualizaEstado
                        };
                        contexto.InsertarClienteRegistro(modeloClienteRegistro);
                        await Application.Current.MainPage.DisplayAlert(
                            "Hecho",
                            "Cliente eliminado localmente",
                            "Aceptar");
                        MessagingCenter.Send<ListaClientes>(this, "EjecutaLista");
                    }
                }
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
            
        }
    }
}