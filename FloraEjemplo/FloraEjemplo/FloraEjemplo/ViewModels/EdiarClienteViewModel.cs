using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using FloraEjemplo.Views;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class EdiarClienteViewModel : INotifyPropertyChanged
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
        private ApiServices apiServices;
        string _nombre;
        int _edad;
        string _telefono;
        string _mail;
        double _saldo;
        string _usuario;
        string _estado;
        #endregion

        #region Properties
        public string Nombre
        {
            get { return _nombre; }
            set
            {
                _nombre = value;
                OnPropertyChanged("Nombre");
            }
        }
        public int Edad
        {
            get { return _edad; }
            set
            {
                _edad = value;
                OnPropertyChanged("Edad");
            }
        }
        public string Telefono
        {
            get { return _telefono; }
            set
            {
                _telefono = value;
                OnPropertyChanged("Telefono");
            }
        }
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged("Mail");
            }
        }
        public double Saldo
        {
            get { return _saldo; }
            set
            {
                _saldo = value;
                OnPropertyChanged("Saldo");
            }
        }
        public string Usuario
        {
            get { return _usuario; }
            set
            {
                _usuario = value;
                OnPropertyChanged("Usuario");
            }
        }
        public string Estado
        {
            get { return _estado; }
            set
            {
                _estado = value;
                OnPropertyChanged("Estado");
            }
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
        public ICommand PutCommand
        {
            get
            {
                return new RelayCommand(Put);
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

        #region Constructor
        public EdiarClienteViewModel()
        {
            apiServices = new ApiServices();
            LoadData();
        }
        #endregion

        #region Methods
        public async void LoadData()
        {
            var connection = await apiServices.CheckConnection();
            if (!connection.IsSuccess)
            {
                var idLocalCliente = (Application.Current.Properties["Correo"] as string);

                using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
                {
                    var Lis = contexto.Consultar(idLocalCliente);
                    this.Nombre = Lis.Nombre;
                    this.Edad = Lis.Edad;
                    this.Mail = Lis.Mail;
                    this.Telefono = Lis.Telefono;
                    this.Saldo = Lis.Saldo;
                    this.Usuario = Lis.Usuario;
                }
                await Application.Current.MainPage.DisplayAlert("Mensaje", "Data Cargada desde BD Local", "ok");
            } //from Local
            else
            {
                GetCliente(); //From Api
            }
        }
        async void GetCliente()
        {
            try
            {
                var idCliente = Application.Current.Properties["Id"] as string;
                string resultado = string.Empty;
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                HttpResponseMessage response = await httpClient.GetAsync("http://efrain1234-001-site1.ftempurl.com/api/Cliente/" + idCliente);
                var result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ha ocurrido un error enviando la solicitud",
                    "Aceptar");
                }

                resultado = response.Content.ReadAsStringAsync().Result;
                resultado = resultado.Replace("\\", "");
                resultado = resultado.Replace("/", "");
                resultado = resultado.Replace("\"[", "[");
                resultado = resultado.Replace("]\"", "]");
                var resulta = resultado;
                var json = JsonConvert.DeserializeObject<List<ClienteModel>>(resulta);
                if (json == null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                       "Error",
                       "Este cliente ha sido eliminado",
                       "Aceptar");
                    return;
                }
                this.Nombre = json[0].Nombre.ToString();
                this.Edad = Convert.ToInt32(json[0].Edad);
                this.Mail = json[0].Mail.ToString();
                this.Telefono = json[0].Telefono.ToString();
                this.Saldo = json[0].Saldo;
                this.Usuario = json[0].Usuario.ToString();
            }
            catch (System.Exception error)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    error.Message,
                    "Aceptar");
            }
        }
        async void Put()
        {
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

            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)
            {
                PutWithConn();
            }
            else
            {
                PutWithoutConn();
            }
        }
        async void PutWithoutConn()//Put sin conexion
        {
            var correo = (Application.Current.Properties["Correo"] as string);
            var numero = (Application.Current.Properties["Numero"]);
            var version = Application.Current.Properties["Version"] as string;
            var dispositivo = Application.Current.Properties["device"] as string;
            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                var aCTIVO = "ACTIVO";
                var aCTUALIZAR = "ACTUALIZAR";
                var cliente = contexto.Consultar(correo);
                ClienteModel modelo = new ClienteModel
                {
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    Proceso = 0,
                    Usuario = Usuario,
                    FechaCreacion = cliente.FechaCreacion,
                    FechaCreacionUtc = cliente.FechaCreacionUtc.ToString(),
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    Id = cliente.Id,
                    Estado = aCTIVO,
                    Transaccion = aCTUALIZAR,
                    Numero = Convert.ToInt32(numero)
                };
                contexto.Actualizar(modelo);
                //Actualizamos en tabla registro

                ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                {
                    Numero = Convert.ToInt32(numero),
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    Proceso = 1,
                    Usuario = Usuario,
                    FechaCreacion = cliente.FechaCreacion,
                    FechaCreacionUtc = cliente.FechaCreacionUtc.ToString(),
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    Id = cliente.Id,
                    Estado = aCTIVO,
                    Transaccion = aCTUALIZAR,
                    Version = version,
                    Dispositivo = dispositivo

                };
                contexto.InsertarClienteRegistro(modeloClienteRegistro);
            }
            await Application.Current.MainPage.DisplayAlert("Mensaje", "Actualizado Localmente", "Ok");
            MessagingCenter.Send<EdiarClienteViewModel>(this, "EjecutaLista");
        }
        async void PutWithConn()
        {
            var correo = (Application.Current.Properties["Correo"] as string);
            var numero = (Application.Current.Properties["Numero"]);
            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                var aCTIVO = "ACTIVO";
                var aCTUALIZAR = "ACTUALIZAR";
                var cliente = contexto.Consultar(correo);
                ClienteModel modelo = new ClienteModel
                {
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    Proceso = 0,
                    Usuario = Usuario,
                    FechaCreacion = cliente.FechaCreacion,
                    FechaCreacionUtc = cliente.FechaCreacionUtc.ToString(),
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    Id = cliente.Id,
                    Estado = aCTIVO,
                    Transaccion = aCTUALIZAR
                };
                contexto.Actualizar(modelo);
            }

            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)
            {
                var aCTIVO = "ACTIVO";
                var aCTUALIZAR = "ACTUALIZAR";
                var id = Application.Current.Properties["Id"] as string;
                ClienteModel Customer = new ClienteModel
                {
                    Numero = Convert.ToInt32(numero),
                    Id = id,
                    Nombre = this.Nombre,
                    Edad = this.Edad,
                    Telefono = this.Telefono,
                    Mail = this.Mail,
                    Saldo = this.Saldo,
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    Proceso = 0,
                    Usuario = this.Usuario,
                    Estado = aCTIVO,
                    Transaccion = aCTUALIZAR
                };
                var jsonCliente = JsonConvert.SerializeObject(Customer);
                EnviarDocumentoPut(jsonCliente);
            }
        }
        public async void EnviarDocumentoPut(string json)
        {
            var aCTIVO = "ACTIVO";
            var aCTUALIZAR = "ACTUALIZAR";
            var version = Application.Current.Properties["Version"] as string;
            var dispositivo = Application.Current.Properties["device"] as string;
            var respuestaOcupado = "http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/-109";
            string urlValidacion = string.Empty;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PutAsync("http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/", new StringContent(json, Encoding.UTF8, "application/json"));
            var header = response.Headers.Location.ToString();
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert(
                    response.IsSuccessStatusCode.ToString(),
                    response.RequestMessage.ToString(),
                    "Aceptar");

                return;
            }

            if (respuestaOcupado == header)
            {
                var correo = (Application.Current.Properties["Correo"] as string);
                var numero = (Application.Current.Properties["Numero"]);
                using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
                {
                    //Actualizamos en tabla registro
                    var cliente = contexto.Consultar(correo);
                    ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                    {
                        Numero = Convert.ToInt32(numero),
                        Nombre = Nombre,
                        Edad = Edad,
                        Telefono = Telefono,
                        Mail = Mail,
                        Saldo = Saldo,
                        Proceso = 1,
                        Usuario = Usuario,
                        FechaCreacion = cliente.FechaCreacion,
                        FechaCreacionUtc = cliente.FechaCreacionUtc.ToString(),
                        FechaModificacion = DateTime.Now,
                        FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                        Id = cliente.Id,
                        Estado = aCTIVO,
                        Transaccion = aCTUALIZAR,
                        Version = version,
                        Dispositivo = dispositivo
                    };
                    contexto.InsertarClienteRegistro(modeloClienteRegistro);
                    await Application.Current.MainPage.DisplayAlert(
                        "Hola",
                        "Actualizado en local, el servidor esta ocupado" + header,
                        "Aceptar");
                    MessagingCenter.Send<EdiarClienteViewModel>(this, "EjecutaLista");
                    return;
                }
            }
            await Application.Current.MainPage.DisplayAlert(
                    response.IsSuccessStatusCode.ToString(),
                    "Actualizado" + header,
                    "Aceptar");
            //Ejecuamos Metodo para refrescar listview de Lista principal
            MessagingCenter.Send<EdiarClienteViewModel>(this, "EjecutaLista");
            //await Application.Current.MainPage.Navigation.PopAsync();
            //Instanciamos pila de navegacion
            IReadOnlyList<Page> navStack = Application.Current.MainPage.Navigation.NavigationStack;
            //accedemos a pagina en la pila de navegacion
            Page editarCliente = navStack[navStack.Count - 1];
            await Application.Current.MainPage.Navigation.PushAsync(new ListaClientes());
            //removemos elemento de la pila de navegacion
            Application.Current.MainPage.Navigation.RemovePage(editarCliente);

            //Aqui podemos navegar hacia donde deseamos 
            
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
            Put();
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

