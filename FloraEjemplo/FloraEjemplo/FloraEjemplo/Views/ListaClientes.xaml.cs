using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            await Application.Current.SavePropertiesAsync();
            var numero = item.Numero.ToString();
            var version = Application.Current.Properties["VersionNew"] as string;
            string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Editar", "Eliminar", "Ver");
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
                        var connection = await apiServices.CheckConnection();
                        //Si hay conexion
                        if (connection.IsSuccess)
                        {
                            
                            var respuestaOcupado = "http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/-109";
                            var aCTIVO = "ELIMINADO";
                            var aCTUALIZAR = "ACTUALIZAR_ESTADO";
                            ClienteModel Customer = new ClienteModel
                            {
                                Numero = item.Numero,
                                Id = item.Id,
                                Nombre = item.Nombre,
                                Edad = item.Edad,
                                Telefono = item.Telefono,
                                Mail = item.Mail,
                                Saldo = item.Saldo,
                                FechaCreacion = item.FechaCreacion,
                                FechaCreacionUtc = item.FechaCreacionUtc.ToString(),
                                FechaModificacion = DateTime.Now,
                                FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                                Proceso = item.Proceso,
                                Usuario = item.Usuario,
                                Estado = aCTIVO,
                                Transaccion = aCTUALIZAR
                            };
                            
                            var jsonCliente = JsonConvert.SerializeObject(Customer);
                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            HttpResponseMessage response = await client.PutAsync("http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/", new StringContent(jsonCliente, Encoding.UTF8, "application/json"));
                            if (response.IsSuccessStatusCode)//IsSuccessStatusCode
                            {
                                var header = response.Headers.Location.ToString();

                                if (header == respuestaOcupado)
                                {
                                    using (var contexto = new DataContext())
                                    {
                                        ClienteModel modelo = (ClienteModel)e.SelectedItem;
                                        var activo = "ELIMINADO";
                                        var actualizaEstado = "ACTUALIZAR_ESTADO";
                                        var dispositivo = Application.Current.Properties["device"] as string;
                                        //Agregamso el cliente borrado en ClienteTrackingModel
                                        ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                                        {
                                            Numero = Convert.ToInt32(numero),
                                            Nombre = modelo.Nombre.ToString(),
                                            Edad = modelo.Edad,
                                            Telefono = modelo.Telefono.ToString(),
                                            Mail = modelo.Mail.ToString(),
                                            Saldo = modelo.Saldo,
                                            Proceso = 1,
                                            Usuario = modelo.Usuario,
                                            FechaCreacion = modelo.FechaCreacion,
                                            FechaCreacionUtc = modelo.FechaCreacionUtc.ToString(),
                                            FechaModificacion = DateTime.Now,
                                            FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                                            Id = modelo.Id,
                                            Estado = activo,
                                            Transaccion = actualizaEstado,
                                            Dispositivo = dispositivo,
                                            Version = version
                                        };

                                        contexto.InsertarClienteRegistro(modeloClienteRegistro);

                                        await Application.Current.MainPage.DisplayAlert(
                                            "Hecho",
                                            "Cliente eliminado localmente",
                                            "Aceptar");

                                        MessagingCenter.Send<ListaClientes>(this, "EjecutaLista");

                                        return;
                                    }
                                }

                                //Eliminamos en local ClienteModel
                                using (var contexto = new DataContext())
                                {
                                    ClienteModel modelo = (ClienteModel)e.SelectedItem;
                                    contexto.Eliminar(modelo);
                                }
                            }
                            else
                            {//Si hubo un error al enviar al servidor, almacenamos en ClienteTrackingModel

                                await Application.Current.MainPage.DisplayAlert(
                                    response.IsSuccessStatusCode.ToString(),
                                    response.RequestMessage.ToString(),
                                    "Aceptar");

                                using (var contexto = new DataContext())
                                {
                                    ClienteModel modelo = (ClienteModel)e.SelectedItem;
                                    var activo = "ELIMINADO";
                                    var actualizaEstado = "ACTUALIZAR_ESTADO";
                                    var dispositivo = Application.Current.Properties["device"] as string;
                                    //Agregamso el cliente borrado en ClienteTrackingModel
                                    ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                                    {
                                        Numero = Convert.ToInt32(numero),
                                        Nombre = modelo.Nombre.ToString(),
                                        Edad = modelo.Edad,
                                        Telefono = modelo.Telefono.ToString(),
                                        Mail = modelo.Mail.ToString(),
                                        Saldo = modelo.Saldo,
                                        Proceso = 1,
                                        Usuario = modelo.Usuario,
                                        FechaCreacion = modelo.FechaCreacion,
                                        FechaCreacionUtc = modelo.FechaCreacionUtc.ToString(),
                                        FechaModificacion = DateTime.Now,
                                        FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                                        Id = modelo.Id,
                                        Estado = activo,
                                        Transaccion = actualizaEstado,
                                        Dispositivo = dispositivo,
                                        Version = version
                                    };

                                    contexto.InsertarClienteRegistro(modeloClienteRegistro);

                                    await Application.Current.MainPage.DisplayAlert(
                                        "Hecho",
                                        "Cliente eliminado localmente",
                                        "Aceptar");

                                    MessagingCenter.Send<ListaClientes>(this, "EjecutaLista");

                                    return;
                                }
                            }

                            await Application.Current.MainPage.DisplayAlert(
                                    "Hecho",
                                    "Cliente eliminado",
                                    "Aceptar");

                            MessagingCenter.Send<ListaClientes>(this, "EjecutaLista");
                        }
                        else
                        {//Si no hay conexion

                            using (var contexto = new DataContext())
                            {
                                
                                ClienteModel modelo = (ClienteModel)e.SelectedItem;

                                var cliente = contexto.Consultar(item.Mail.ToString());
                                var cliente2 = contexto.ConsultarCorreoTracking(item.Mail.ToString());
                                if (string.IsNullOrEmpty(cliente.Id.ToString()) && !string.IsNullOrEmpty(cliente.Mail.ToString()))
                                {
                                    contexto.EliminarClienteTracking(cliente2);
                                }
                                
                                if (cliente == null)
                                {
                                    await Application.Current.MainPage.DisplayAlert(
                                    "Error",
                                    "Este cliente no existe localmente",
                                    "Aceptar");

                                    return;
                                }

                                if (cliente2 == null)
                                {
                                    var activo = "ELIMINADO";
                                    var actualizaEstado = "ACTUALIZAR_ESTADO";
                                    var dispositivo = Application.Current.Properties["device"] as string;
                                    //Borramos en cliente tracking

                                    ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                                    {
                                        Nombre = modelo.Nombre.ToString(),
                                        Edad = modelo.Edad,
                                        Telefono = modelo.Telefono.ToString(),
                                        Mail = modelo.Mail.ToString(),
                                        Saldo = modelo.Saldo,
                                        Proceso = 1,
                                        Usuario = modelo.Usuario,
                                        FechaCreacion = modelo.FechaCreacion,
                                        FechaCreacionUtc = modelo.FechaCreacionUtc.ToString(),
                                        FechaModificacion = DateTime.Now,
                                        FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                                        Id = modelo.Id,
                                        Estado = activo,
                                        Transaccion = actualizaEstado,
                                        Dispositivo = dispositivo,
                                        Version = version
                                    };

                                    contexto.InsertarClienteRegistro(modeloClienteRegistro);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(cliente2.Id.ToString()) && !string.IsNullOrEmpty(cliente2.Mail.ToString()))
                                    {
                                        var activo = "ELIMINADO";
                                        var actualizaEstado = "ACTUALIZAR_ESTADO";
                                        var dispositivo = Application.Current.Properties["device"] as string;
                                        //Borramos en cliente tracking

                                        ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                                        {
                                            Numero = Convert.ToInt32(cliente2.Numero),
                                            Nombre = modelo.Nombre.ToString(),
                                            Edad = modelo.Edad,
                                            Telefono = modelo.Telefono.ToString(),
                                            Mail = modelo.Mail.ToString(),
                                            Saldo = modelo.Saldo,
                                            Proceso = 1,
                                            Usuario = modelo.Usuario,
                                            FechaCreacion = modelo.FechaCreacion,
                                            FechaCreacionUtc = modelo.FechaCreacionUtc.ToString(),
                                            FechaModificacion = DateTime.Now,
                                            FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                                            Id = modelo.Id,
                                            Estado = activo,
                                            Transaccion = actualizaEstado,
                                            Dispositivo = dispositivo,
                                            Version = version
                                        };

                                        contexto.ActualizarClienteRegistro(modeloClienteRegistro);

                                    }
                                }

                                //Borramos en ClienteModel
                                contexto.Eliminar(modelo);

                                await Application.Current.MainPage.DisplayAlert(
                                    "Hecho",
                                    "Cliente eliminado localmente",
                                    "Aceptar");

                                MessagingCenter.Send<ListaClientes>(this, "EjecutaLista");
                            }
                        }
                    }
                });
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
                        barraNavegacion.HeightRequest = 300;
                        grilla.HeightRequest = 300;
                        label3.FontSize = 14;
                    }
                    else
                    {
                        Hamb.IsVisible = true;
                        barraNavegacion.HeightRequest = 180;
                        grilla.HeightRequest = 180;
                        label3.FontSize = 14;
                    }
                }
                else
                {
                    if (width > height)
                    {
                        barraNavegacion.HeightRequest = 130;
                        grilla.HeightRequest = 130;
                        label3.FontSize = 18;
                        Hamb.IsVisible = false;
                    }
                    else
                    {
                        barraNavegacion.HeightRequest = 130;
                        grilla.HeightRequest = 130;
                        label3.FontSize = 18;
                        Hamb.IsVisible = true;
                    }
                }
            }
        }
    }
}