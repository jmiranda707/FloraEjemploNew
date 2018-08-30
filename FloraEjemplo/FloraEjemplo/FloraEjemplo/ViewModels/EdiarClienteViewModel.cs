﻿using FloraEjemplo.Data;
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
        bool isEnabled;
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
        public EdiarClienteViewModel()
        {
            this.IsEnabled = true;
            apiServices = new ApiServices();
            listviewmodel = new ListClientesViewModel();
            LoadData();
        }
        #endregion

        #region Methods
        public async void LoadData()//Cargamos datos sel cliente seleccionado
        {
            this.IsEnabled = true;
            var connection = await apiServices.CheckConnection();
            if (!connection.IsSuccess)//Si no hay conexion, cargamos los datos de la DB local
            {
                var idLocalCliente = (Application.Current.Properties["Correo"] as string);

                using (var contexto = new DataContext())
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
            } 
            else
            {
                GetCliente(); //Obtenemos de la Api
            }
        }
        async void GetCliente()//Obtenemos datos del cliente de la Api
        {
            this.IsEnabled = false;
            try
            {
                var idCliente = Application.Current.Properties["Id"] as string;
                string resultado = string.Empty;
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                HttpResponseMessage response = await httpClient.GetAsync("http://efrain1234-001-site1.ftempurl.com/api/Cliente/" + idCliente);
                var result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)//Si existe algun error en la peticion, cargamos del local
                {
                    await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ha ocurrido un error enviando la solicitud",
                    "Aceptar");

                    this.IsEnabled = false;

                    //Cargamos desde la DB local
                    LoadClientFromLocal();

                    return;
                }
                //IsSuccessStatusCode
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

                    this.IsEnabled = true;

                    return;
                }
                this.Nombre = json[0].Nombre.ToString();
                this.Edad = Convert.ToInt32(json[0].Edad);
                this.Mail = json[0].Mail.ToString();
                this.Telefono = json[0].Telefono.ToString();
                this.Saldo = json[0].Saldo;
                this.Usuario = json[0].Usuario.ToString();

                this.IsEnabled = true;
            }
            catch (System.Exception error)//Algun error cargamos los datos del local
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    error.Message,
                    "Aceptar");

                this.IsEnabled = false;

                LoadClientFromLocal();
            }

            this.IsEnabled = true;
        }
        async void LoadClientFromLocal()//Cargamos desde la DB local
        {
            this.IsEnabled = false;
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
            this.IsEnabled = true;
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
                var aCTUALIZAR = "ACTUALIZAR";
                var cliente = contexto.Consultar(correo);
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

                //Insertamos en tabla registro
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
            await Application.Current.MainPage.DisplayAlert(
                "Mensaje", 
                "Actualizado Localmente", 
                "Ok");

            this.IsEnabled = true;

            MessagingCenter.Send<EdiarClienteViewModel>(this, "EjecutaLista");
        }
        async void PutWithConn()//Put con conexión
        {
            this.IsEnabled = false;
            var correo = (Application.Current.Properties["Correo"] as string);
            var numero = (Application.Current.Properties["Numero"]);
            var aCTIVO = "ACTIVO";
            var aCTUALIZAR = "ACTUALIZAR";
            //Verificamos conexion
            var connection = await apiServices.CheckConnection();
            if (connection.IsSuccess)//Si hay conexion
            {
                using (var contexto = new DataContext()) 
                {
                    var cliente = contexto.Consultar(correo);
                    //Actualizamos en Cliente
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
                var id = Application.Current.Properties["Id"] as string;
                //Preparamos modelo
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
                var aCTUALIZAR = "ACTUALIZAR";
                var version = Application.Current.Properties["VersionNew"] as string;
                var dispositivo = Application.Current.Properties["device"] as string;
                var respuestaOcupado = "http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/-109";
                var noExisteId = "http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/-103";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsync("http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/", new StringContent(json, Encoding.UTF8, "application/json"));
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
                            "Actualizado en local, servidor ocupado "+header,
                            "Aceptar");

                        MessagingCenter.Send<EdiarClienteViewModel>(this, "EjecutaLista");

                        return;
                    }
                }

                if (header == noExisteId)//No existe Id del cliente
                {
                    await Application.Current.MainPage.DisplayAlert(
                            "Hola",
                            "No existe el cliente seleccionado" + header,
                            "Aceptar");

                    return;
                }

                await Application.Current.MainPage.DisplayAlert(
                        response.IsSuccessStatusCode.ToString(),
                        "Actualizado" + header,
                        "Aceptar");

                //Ejecuamos Metodo para refrescar listview de Lista principal
                MessagingCenter.Send<EdiarClienteViewModel>(this, "EjecutaLista");

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

