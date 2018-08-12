
using FloraEjemplo.Data;
using FloraEjemplo.Models;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class AddClienteViewModel
    {
        #region Attributes
        ListClientesViewModel listaClientes; 
        #endregion

        #region Properties
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public double Saldo { get; set; }
        public string Usuario { get; set; }
        public string Estado { get; set; }
        #endregion

        #region Constructors
        public AddClienteViewModel()
        {
            listaClientes = new ListClientesViewModel();
        } 
        #endregion

        #region Commands
        public ICommand BackToolCommand
        {
            get
            {
                return new RelayCommand(BackTool);
            }
        }
        public ICommand EditToolCommand
        {
            get
            {
                return new RelayCommand(EditTool);
            }
        }
        public ICommand SaveToolCommand
        {
            get
            {
                return new RelayCommand(SaveTool);
            }
        }
        public ICommand CloseToolCommand
        {
            get
            {
                return new RelayCommand(CloseTool);
            }
        }
        public ICommand PostCommand
        {
            get
            {
                return new RelayCommand(Post);
            }
        }
        public ICommand VolverCommand
        {
            get
            {
                return new RelayCommand(Volver);
            }
        }
        #endregion

        #region Methods
        private void Post()
        {

            #region guardo primero en local
            Cliente2 modelo = new Cliente2
            {
                Nombre = Nombre,
                Edad = Edad,
                Telefono = Telefono,
                Mail = Mail,
                Saldo = Saldo,
                FechaCreacion = "",
                FechaCreacionUtc= "",
                FechaCreacionLocal = DateTime.Now.ToString(),
                FechaCreacionUtcLocal = DateTime.UtcNow.ToString(),
                FechaModificacionLocal = DateTime.Now,
                FechaModificacionUtcLocal = DateTime.UtcNow,
                FechaModificacion = DateTime.Now.ToString(),                //servidor
                FechaModificacionUtc = DateTime.UtcNow.ToString(),         //servidor
                Proceso = 0,
                Usuario = Usuario,
                Estado = "Activo",
                EstadoLocal = "Activo",
                Id = "",
                Numero = 0,
                Sincronizado = false,
            };

            using (var contexto = new DataContext()) //aqui inserto en mi bdLocal
            {
                contexto.Insertar(modelo);
            }

            Application.Current.MainPage.DisplayAlert("Mensaje", "Datos Guardados Localmente", "Entendido");
            #endregion


            Cliente Customer = new Cliente
            {
                Numero = 0,
                Id = string.Empty,
                Nombre = this.Nombre,
                Edad = this.Edad,
                Telefono = this.Telefono,
                Mail = this.Mail,
                Saldo = this.Saldo,
                FechaCreacion = DateTime.Now,
                FechaCreacionUtc = DateTime.UtcNow,
                FechaModificacion = DateTime.Now,
                FechaModificacionUtc = DateTime.UtcNow,
                Proceso = 0,
                Usuario = this.Usuario,
                Estado = "ACTIVO"
            };
            var jsonCliente = JsonConvert.SerializeObject(Customer);
            EnviarDocumentoPost(jsonCliente);
        }
        public async void EnviarDocumentoPost(string json)
        {
            string urlValidacion = string.Empty;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsync("http://efrain1234-001-site1.ftempurl.com/api/NuevoCliente/", new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                urlValidacion = response.Headers.Location.ToString();
            }
            else
            {
                urlValidacion = response.Headers.Location.ToString();
            }

            listaClientes.LoadData(); //para actualizar mi lista de clientes en el home
            await Application.Current.MainPage.Navigation.PopAsync();
            this.Nombre = string.Empty;
            this.Edad = 0;
            this.Telefono = string.Empty;
            this.Mail = string.Empty;
            this.Saldo = 0;
            this.Usuario = string.Empty;
            this.Estado = string.Empty;
    }

        async void Volver()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        async void CloseTool()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        void SaveTool()
        {
            Post();
        }
        async void EditTool()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Hola",
                "EditTool",
                "Aceptar");
        }
        async void BackTool()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        #endregion
    }
}
