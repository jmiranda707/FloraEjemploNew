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
        //Put sin conexion
        async void PutWithoutConn()
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
                    //ClientId = Convert.ToInt32(clientId),
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    Proceso = 0,
                    Usuario = Usuario,
                    FechaCreacion = cliente.FechaCreacion,
                    FechaCreacionUtc = cliente.FechaCreacionUtc,
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow,
                    Id = cliente.Id,
                    Estado = aCTIVO,
                    Transaccion = aCTUALIZAR
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
                    FechaCreacionUtc = cliente.FechaCreacionUtc,
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow,
                    Id = cliente.Id,
                    Estado = aCTIVO,
                    Transaccion = aCTUALIZAR
                };
                contexto.InsertarClienteRegistro(modeloClienteRegistro);
            }
            await Application.Current.MainPage.DisplayAlert("Mensaje", "Actualizado Localmente", "Ok");
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
                    //ClientId = Convert.ToInt32(clientId),
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    Proceso = 0,
                    Usuario = Usuario,
                    FechaCreacion = cliente.FechaCreacion,
                    FechaCreacionUtc = cliente.FechaCreacionUtc,
                    FechaModificacion = DateTime.Now,
                    FechaModificacionUtc = DateTime.UtcNow,
                    Id = cliente.Id,
                    Estado = aCTIVO,
                    Transaccion = aCTUALIZAR
                };
                contexto.Actualizar(modelo);
                ////Actualizamos en tabla registro

                //ClienteTrackingModel modeloClienteRegistro = new ClienteTrackingModel
                //{
                //    Numero = Convert.ToInt32(numero),
                //    Nombre = Nombre,
                //    Edad = Edad,
                //    Telefono = Telefono,
                //    Mail = Mail,
                //    Saldo = Saldo,
                //    Proceso = 0,
                //    Usuario = Usuario,
                //    FechaCreacion = cliente.FechaCreacion,
                //    FechaCreacionUtc = cliente.FechaCreacionUtc,
                //    FechaModificacion = cliente.FechaModificacion,
                //    FechaModificacionUtc = cliente.FechaModificacionUtc,
                //    Id = cliente.Id,
                //    Estado = aCTIVO,
                //    Transaccion = aCTUALIZAR
                //};
                //contexto.InsertarClienteRegistro(modeloClienteRegistro);
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
                    FechaModificacionUtc = DateTime.UtcNow,
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
            string urlValidacion = string.Empty;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PutAsync("http://efrain1234-001-site1.ftempurl.com/api/ActualizarCliente/", new StringContent(json, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert(
                    response.IsSuccessStatusCode.ToString(),
                    response.RequestMessage.ToString(),
                    "Aceptar");

                return;
            }

            await Application.Current.MainPage.DisplayAlert(
                    response.IsSuccessStatusCode.ToString(),
                    "Actualizado",
                    "Aceptar");

            await Application.Current.MainPage.Navigation.PopAsync();
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
