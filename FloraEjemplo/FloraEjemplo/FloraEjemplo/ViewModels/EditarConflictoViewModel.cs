﻿
using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
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
    public class EditarConflictoViewModel : INotifyPropertyChanged
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
        bool isEnabled;
        DataContext dataContext;
        ListClientesViewModel listviewmodel;
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
        public EditarConflictoViewModel()
        {
            this.IsEnabled = true;
            apiServices = new ApiServices();
            dataContext = new DataContext();
            listviewmodel = new ListClientesViewModel();
            LoadData();
        }
        #endregion

        #region Methods
        public async void LoadData()//Cargamos datos sel cliente seleccionado
        {
            this.IsEnabled = true;
            var correoCliente = (Application.Current.Properties["Correo"] as string);

            using (var contexto = new DataContext())
            {
                var Lis = contexto.ConsultarCorreoClienteConflicto(correoCliente);

                this.Nombre = Lis.Nombre;
                this.Edad = Lis.Edad;
                this.Mail = Lis.Mail;
                this.Telefono = Lis.Telefono;
                this.Saldo = Lis.Saldo;
                this.Usuario = Lis.Usuario;
            }
            await Application.Current.MainPage.DisplayAlert("Mensaje", "Data Cargada desde BD Local", "ok");
        }
        async void Put()//Validamos antes de enviar
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
            else if (Edad < 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Campo Edad no admite valores negativos", "Aceptar");
                return;
            }
            else if (Edad > 99)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Campo Edad no debe ser mayor a 99 años", "Aceptar");
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
            //Verificamos conexion para elegir metodo
            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)
            {
                PutWithConn();
            }
            else
            {
                PutWithoutConn();

                await Application.Current.MainPage.DisplayAlert(
               "Mensaje",
               "Actualizado Localmente",
               "Ok");

                this.IsEnabled = true;

                MessagingCenter.Send<EditarConflictoViewModel>(this, "EjecutaLista");
            }
        }
        async void PutWithoutConn()//Put sin conexion
        {
            this.IsEnabled = false;
            var correo = (Application.Current.Properties["Correo"] as string);
            var numero = (Application.Current.Properties["Numero"]);
            var version = Application.Current.Properties["VersionNew"] as string;
            var dispositivo = Application.Current.Properties["device"] as string;
            using (var contexto = new DataContext())
            {
                var aCTIVO = "ACTIVO";
                var insertar = "INSERTAR";
                var consulta = contexto.Consultar(this.Mail);
                var cliente = contexto.ConsultarCorreoClienteConflicto(correo);

                if (consulta != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Usuario ya existe",
                        "Acceptar");

                    return;
                }

                var cliente2 = contexto.ConsultarCorreoTracking(this.Mail);

                if (cliente2 == null)
                {
                    //Insertamos en tabla registro
                    ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                    {
                        //Numero = Convert.ToInt32(cliente2.Numero),
                        Nombre = Nombre,
                        Edad = Edad,
                        Telefono = Telefono,
                        Mail = Mail,
                        Saldo = Saldo,
                        Proceso = 1,
                        Usuario = Usuario,
                        FechaCreacion = DateTime.Now,
                        FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                        FechaModificacion = DateTime.Now,
                        FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                        Estado = aCTIVO,
                        Transaccion = insertar,
                        Version = version,
                        Dispositivo = dispositivo
                    };

                    contexto.InsertarClienteRegistro(modeloClienteRegistro);

                }
                else
                {
                    if (!string.IsNullOrEmpty(cliente2.Id.ToString()) && !string.IsNullOrEmpty(cliente2.Mail.ToString()))
                    {
                        await Application.Current.MainPage.DisplayAlert(
                           "Error",
                           "Usuario ya existe",
                           "Acceptar");

                        return;
                    }
                }
                //Creamos el modelo y actualizamos en el local
                ClienteModel modelo = new ClienteModel
                {
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    Proceso = 0,
                    Usuario = Usuario,
                    FechaCreacion = DateTime.Now,
                    FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                    Estado = aCTIVO,
                    Transaccion = insertar,
                };

                contexto.Insertar(modelo);

                ClientsConflicts modelo2 = new ClientsConflicts
                {
                    Numero = cliente.Numero,
                    Nombre = cliente.Nombre,
                    Edad = cliente.Edad,
                    Telefono = cliente.Telefono,
                    Mail = cliente.Mail,
                    Saldo = cliente.Saldo,
                    FechaCreacion = cliente.FechaCreacion,
                    FechaCreacionUtc = cliente.FechaCreacionUtc,
                    FechaModificacion = cliente.FechaModificacion,
                    FechaModificacionUtc = cliente.FechaModificacionUtc,
                    Proceso = cliente.Proceso,
                    Usuario = cliente.Usuario,
                    Estado = cliente.Estado,
                    Version = cliente.Version,
                    Dispositivo = cliente.Dispositivo,
                    Transaccion = cliente.Transaccion
                };

                contexto.EliminarClienteConflicto(modelo2);
            }

            await Application.Current.MainPage.DisplayAlert(
                       "Exito",
                       "Hecho",
                       "Aceptar");

            this.Nombre = string.Empty;
            this.Edad = 0;
            this.Telefono = string.Empty;
            this.Mail = string.Empty;
            this.Saldo = 0;
            this.Usuario = string.Empty;
            this.Estado = string.Empty;

            //Ejecuamos Metodo para refrescar listview de Listas principales
            MessagingCenter.Send<EditarConflictoViewModel>(this, "ListaConflicto");

            MessagingCenter.Send<EditarConflictoViewModel>(this, "EjecutaLista");

            await Application.Current.MainPage.Navigation.PopAsync();

            this.IsEnabled = true;
        }
        async void PutWithConn()//Put con conexión
        {
            this.IsEnabled = false;
            var correo = (Application.Current.Properties["Correo"] as string);
            var numero = (Application.Current.Properties["Numero"]);
            var aCTIVO = "ACTIVO";
            var insertar = "INSERTAR";
            //Verificamos conexion
            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)//Si hay conexion
            {
                using (var contexto = new DataContext())
                {
                    var cliente = contexto.ConsultarCorreoClienteConflicto(correo);

                    if (cliente == null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                           "Hola",
                           "Este cliente no existe!",
                           "Aceptar");

                        this.Nombre = string.Empty;
                        this.Edad = 0;
                        this.Telefono = string.Empty;
                        this.Mail = string.Empty;
                        this.Saldo = 0;
                        this.Usuario = string.Empty;
                        this.Estado = string.Empty;

                        await Application.Current.MainPage.Navigation.PopAsync();

                        return;
                    }
                  }

                //var id = Application.Current.Properties["Id"] as string;
                //Preparamos modelo
                ClienteModel Customer = new ClienteModel
                {
                    Id = "",
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
                    Transaccion = insertar
                };

                var jsonCliente = JsonConvert.SerializeObject(Customer);//serializamos el modelo

                EnviarDocumentoPut(jsonCliente);
            }
            else
            {
                PutWithoutConn();
            }
        }
        public async void EnviarDocumentoPut(string json)//Enviando con conexion
        {
            try
            {
                this.IsEnabled = false;
                var aCTIVO = "ACTIVO";
                var insertar = "INSERTAR";
                var version = Application.Current.Properties["VersionNew"] as string;
                var dispositivo = Application.Current.Properties["device"] as string;
                var respuestaOcupado = "http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/-109";
                var noExisteId = "http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/-103";
                var yaRegistrado = "http://efrain1234-001-site1.ftempurl.com/api/Cliente/-107";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.PutAsync("http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/", new StringContent(json, Encoding.UTF8, "application/json"));
                HttpResponseMessage response = await client.PostAsync("http://efrain1234-001-site1.ftempurl.com/api/NuevoCliente/", new StringContent(json, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)//Si no fue exitoso, cargamos en tabla registro
                {
                    await Application.Current.MainPage.DisplayAlert(
                        response.IsSuccessStatusCode.ToString(),
                        response.RequestMessage.ToString(),
                        "Aceptar");

                    PutWithoutConn();

                    this.IsEnabled = true;

                    return;
                }
                var header = response.Headers.Location.ToString();

                if (respuestaOcupado == header)//Si el servidor esta ocupado
                {
                    var correo = (Application.Current.Properties["Correo"] as string);
                    var numero = (Application.Current.Properties["Numero"]);
                    using (var contexto = new DataContext())
                    {
                        //Actualizamos en tabla registro para enviar luego
                        var Consulta = contexto.Consultar(correo);
                        var cliente = contexto.ConsultarCorreoClienteConflicto(correo);

                        //Insertamos en Cliente
                        ClienteModel modelo = new ClienteModel
                        {
                            Nombre = Nombre,
                            Edad = Edad,
                            Telefono = Telefono,
                            Mail = Mail,
                            Saldo = Saldo,
                            Proceso = 0,
                            Usuario = Usuario,
                            FechaCreacion = DateTime.Now,
                            FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                            FechaModificacion = DateTime.Now,
                            FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                            Id = "",
                            Estado = aCTIVO,
                            Transaccion = insertar,
                            Version = version,
                            Dispositivo = dispositivo
                        };

                        contexto.Insertar(modelo);

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
                            Id = "",
                            Estado = aCTIVO,
                            Transaccion = insertar,
                            Version = version,
                            Dispositivo = dispositivo
                        };

                        contexto.InsertarClienteRegistro(modeloClienteRegistro);

                        await Application.Current.MainPage.DisplayAlert(
                            "Hola",
                            "Actualizado en local, servidor ocupado " + header,
                            "Aceptar");

                        MessagingCenter.Send<EditarConflictoViewModel>(this, "EjecutaLista");

                        return;
                    }
                }

                if (header == yaRegistrado)//Id ya registrado
                {
                    await Application.Current.MainPage.DisplayAlert(
                         "Hola",
                         "Correo electrónico en existencia, por favor utilice otra cuenta de correo",
                         "Aceptar");

                    this.IsEnabled = true;

                    return;
                }

                var correo2 = (Application.Current.Properties["Correo"] as string);
                using (var contexto = new DataContext())
                {
                    //var cliente = contexto.Consultar(correo2);
                    var cliente = contexto.ConsultarCorreoClienteConflicto(correo2);

                    //Insertamos en Cliente
                    ClienteModel modelo = new ClienteModel
                    {
                        Nombre = Nombre,
                        Edad = Edad,
                        Telefono = Telefono,
                        Mail = Mail,
                        Saldo = Saldo,
                        Proceso = 0,
                        Usuario = Usuario,
                        FechaCreacion = DateTime.Now,
                        FechaCreacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                        FechaModificacion = DateTime.Now,
                        FechaModificacionUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+hh:mm"),
                        Id = "",
                        Estado = aCTIVO,
                        Transaccion = insertar,
                        Version = version,
                        Dispositivo = dispositivo
                    };

                    contexto.Insertar(modelo);

                    var numero = (Application.Current.Properties["Numero"]);

                    ClientsConflicts modelo2 = new ClientsConflicts
                    {
                        Numero = cliente.Numero,
                        Nombre = cliente.Nombre,
                        Edad = cliente.Edad,
                        Telefono = cliente.Telefono,
                        Mail = cliente.Mail,
                        Saldo = cliente.Saldo,
                        FechaCreacion = cliente.FechaCreacion,
                        FechaCreacionUtc = cliente.FechaCreacionUtc,
                        FechaModificacion = cliente.FechaModificacion,
                        FechaModificacionUtc = cliente.FechaModificacionUtc,
                        Proceso = cliente.Proceso,
                        Usuario = cliente.Usuario,
                        Estado = cliente.Estado,
                        Version = cliente.Version,
                        Dispositivo = cliente.Dispositivo,
                        Id = cliente.Id,
                        Transaccion = cliente.Transaccion
                    };

                    contexto.EliminarClienteConflicto(modelo2);
                }

                //var connection = await apiServices.CheckConnection();

                await Application.Current.MainPage.DisplayAlert(
                        response.IsSuccessStatusCode.ToString(),
                        "Actualizado" + header,
                        "Aceptar");

                this.Nombre = string.Empty;
                this.Edad = 0;
                this.Telefono = string.Empty;
                this.Mail = string.Empty;
                this.Saldo = 0;
                this.Usuario = string.Empty;
                this.Estado = string.Empty;

                //Ejecuamos Metodo para refrescar listview de Listas principales
                MessagingCenter.Send<EditarConflictoViewModel>(this, "ListaConflicto");

                MessagingCenter.Send<EditarConflictoViewModel>(this, "EjecutaLista");

                await Application.Current.MainPage.Navigation.PopAsync();

                this.IsEnabled = true;

            }

            catch (Exception error)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    error.Message,
                    "Aceptar");

                this.IsEnabled = true;

                PutWithoutConn();
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
