
using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class AddClienteViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Attributes
        ListClientesViewModel listaClientes;
        bool isEnabled;
        #endregion

        #region Properties
        private ApiServices apiServices;
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public long Saldo { get; set; }
        public string Usuario { get; set; }
        public string Estado { get; set; }
        public string Transaccion { get; set; }
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        #endregion

        #region Constructors
        public AddClienteViewModel()
        {
            apiServices = new ApiServices();
            listaClientes = new ListClientesViewModel();
            this.Transaccion = "Insertar";
            this.IsEnabled = true;
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
        //Validamos antes de enviar
        private async void Post()
        {
            this.IsEnabled = true;
            const string passwordRegex = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$";
            double m;
            int n;

            var emailP = "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&\'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$";
            var minLimit = 3;
            var maxLimit = 30;

            //var isNumericApellido = int.TryParse(apellido, out n);
            var isNumericNombre = int.TryParse(this.Nombre, out n);
            var isNumericTelefono = double.TryParse(Telefono, out m);

            if (string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Telefono) ||
                string.IsNullOrEmpty(Mail) || string.IsNullOrEmpty(Usuario) || this.Edad == 0 || this.Saldo == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No debe dejar campos vacios", "Aceptar");
                return;
            }
            else if (Nombre.Length < minLimit || Nombre.Length > maxLimit)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Campo Nombre no debe ser menor a 3 caracteres o mayor a 30 caracteres", "Aceptar");
                return;
            }
            else if (isNumericNombre)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese solo letras en campo Nombre", "Aceptar");
                return;
            }
            else if (Telefono.Length < 10 || Telefono.Length > 13)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Campo Telefono no debe ser menor a 10 carcateres o mayor a 13 caracteres", "Aceptar");
                return;
            }
            else if (!isNumericTelefono)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese solo números en campo Telefono", "Aceptar");
                return;
            }
            else if (!Regex.IsMatch(this.Mail, emailP))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un correo valido", "Aceptar");
                return;
            }

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                var correoIngresado = this.Mail;
                var mail = contexto.Consultar(correoIngresado);
                if (mail != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Mensaje",
                        "Correo electronico ya registrado, intente utilizando otra cuenta de correo",
                        "Entendido");
                    return;
                }
                //var usuarioIngresado = this.Usuario;
                //var user = contexto.ConsultarUsuario(usuarioIngresado);
                //if (user != null)
                //{
                //    await Application.Current.MainPage.DisplayAlert(
                //        "Mensaje",
                //        "Usuario ya registrado, intente usuando otro usuario",
                //        "Entendido");
                //    return;
                //}
            }
            //Validamos conexion
            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)
            {
                PostWithConn();
            }
            else
            {
                PostWitoutConn();
            }
        }
        //Post sin conexión
        async void PostWitoutConn()
        {
            this.IsEnabled = false;
            var aCTIVO = "ACTIVO";
            var insertar = "INSERTAR";
            var version = Application.Current.Properties["Version"] as string;
            var dispositivo = Application.Current.Properties["device"] as string;

            //Almacenamos en Tabla ClienteModel
            ClienteModel modelo = new ClienteModel
            {
                Nombre = Nombre,
                Edad = Edad,
                Telefono = Telefono,
                Mail = Mail,
                Saldo = Saldo,
                FechaCreacion = DateTime.Now,
                FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                FechaModificacion = DateTime.Now,
                FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                Proceso = 0,
                Usuario = Usuario,
                Estado = aCTIVO,
                Id = "",
                Transaccion = insertar
            };
            //aqui inserto en mi bdLocal
            using (var contexto = new DataContext()) 
            {
                contexto.Insertar(modelo);
            }

            //Almacenamos en Tabla ClienteTrackingModel
            ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
            {
                Nombre = Nombre,
                Edad = Edad,
                Telefono = Telefono,
                Mail = Mail,
                Saldo = Saldo,
                FechaCreacion = DateTime.Now,
                FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                FechaModificacion = DateTime.Now,
                FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                Proceso = 1,
                Usuario = Usuario,
                Estado = aCTIVO,
                Id = "",
                Numero = 0,
                Transaccion = insertar,
                Version = version,
                Dispositivo = dispositivo
            };
            //aqui inserto en mi bdLocal Tabla Seguimiento
            using (var contexto = new DataContext()) 
            {
                contexto.InsertarClienteRegistro(modeloClienteRegistro);
            }

            MessagingCenter.Send<AddClienteViewModel>(this, "EjecutaLista");

            this.IsEnabled = true;

            await Application.Current.MainPage.Navigation.PopAsync();
            
        }
        //Post con conexion
        async void PostWithConn()
        {
            this.IsEnabled = false;
            var aCTIVO = "ACTIVO";
            var insertar = "INSERTAR";
            //Almacenamos en Tabla ClienteModel
            ClienteModel modelo = new ClienteModel
            {
                Nombre = Nombre,
                Edad = Edad,
                Telefono = Telefono,
                Mail = Mail,
                Saldo = Saldo,
                FechaCreacion = DateTime.Now,
                FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                FechaModificacion = DateTime.Now,
                FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                Proceso = 0,
                Usuario = Usuario,
                Estado = aCTIVO,
                Id = "",
                Transaccion = insertar
            };
            using (var contexto = new DataContext()) //aqui inserto en mi bdLocal
            {
                contexto.Insertar(modelo);
            }
            
            //Verificamos conexion
            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)
            {
                //Creamos modelo
                ClienteModel Customer = new ClienteModel
                {
                    Id = string.Empty,
                    Nombre = this.Nombre,
                    Edad = this.Edad,
                    Telefono = this.Telefono,
                    Mail = this.Mail,
                    Saldo = this.Saldo,
                    FechaCreacion = DateTime.Now,
                    FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    Proceso = 0,
                    Usuario = this.Usuario,
                    Estado = aCTIVO,
                    Transaccion = insertar,
                    Numero = 0
                };
                //Serializamos
                var jsonCliente = JsonConvert.SerializeObject(Customer);
                //Enviamos al metodo EnviarDocumentoPost()
                EnviarDocumentoPost(jsonCliente);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Mensaje", "Datos Guardados Localmente", "Entendido");
                this.IsEnabled = true;
            }
        }
        public async void EnviarDocumentoPost(string json)
        {
            try
            {
                this.IsEnabled = false;
                var version = Application.Current.Properties["Version"] as string;
                var dispositivo = Application.Current.Properties["device"] as string;
                var aCTIVO = "ACTIVO";
                var insertar = "INSERTAR";
                string urlValidacion = string.Empty;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync("http://efrain1234-001-site1.ftempurl.com/api/NuevoCliente/", new StringContent(json, Encoding.UTF8, "application/json"));
                var yaRegistrado = "http://efrain1234-001-site1.ftempurl.com/api/Cliente/-107";
                var respuestaOcupado = "http://efrain1234-001-site1.ftempurl.com/api/Cliente/-109";
                var header = response.Headers.Location.ToString();
                if (response.IsSuccessStatusCode)
                {
                    //Servidor ocupado
                    if (respuestaOcupado == response.Headers.Location.ToString())
                    {
                        //Almacenamos en Tabla ClienteTrackingModel
                        ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                        {
                            Nombre = Nombre,
                            Edad = Edad,
                            Telefono = Telefono,
                            Mail = Mail,
                            Saldo = Saldo,
                            FechaCreacion = DateTime.Now,
                            FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                            FechaModificacion = DateTime.Now,
                            FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                            Proceso = 1,
                            Usuario = Usuario,
                            Estado = aCTIVO,
                            Id = "",
                            Numero = 0,
                            Transaccion = insertar,
                            Version = version,
                            Dispositivo = dispositivo
                        };
                        using (var contexto = new DataContext()) //aqui inserto en mi bdLocal
                        {
                            contexto.InsertarClienteRegistro(modeloClienteRegistro);
                        }
                    }//Correo electronico ya registrado en DB
                    else if (yaRegistrado == response.Headers.Location.ToString())
                    {
                        await Application.Current.MainPage.DisplayAlert(
                         "Hola",
                         "Correo electrónico en existencia, por favor utilice otra cuenta de correo",
                         "Aceptar");

                        this.IsEnabled = true;

                        return;
                    }
                }
                else
                {
                    //Algun error en el servidor almacenamos en Tabla segimiento
                    await Application.Current.MainPage.DisplayAlert(
                    "Error " + response.IsSuccessStatusCode.ToString(),
                    response.RequestMessage.ToString(),
                    "Aceptar");

                    PostWitoutConn();

                    this.IsEnabled = true;

                    return;
                }
                //IsSuccessStatusCode
                var result = response.Content.ReadAsStringAsync().Result;
                this.Nombre = string.Empty;
                this.Edad = 0;
                this.Telefono = string.Empty;
                this.Mail = string.Empty;
                this.Saldo = 0;
                this.Usuario = string.Empty;
                this.Estado = string.Empty;

                await Application.Current.MainPage.DisplayAlert(
                     "Hola",
                     "Usuario agregado " + response.Headers.Location.ToString(),
                     "Aceptar");

                MessagingCenter.Send<AddClienteViewModel>(this, "EjecutaLista");

                this.IsEnabled = true;

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception error)
            {
                //Algun error almacenamos en Tabla segimiento

                await Application.Current.MainPage.DisplayAlert(
                   "Error",
                   error.Message,
                   "Aceptar");

                this.IsEnabled = false;

                PostWitoutConn();
            }
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
