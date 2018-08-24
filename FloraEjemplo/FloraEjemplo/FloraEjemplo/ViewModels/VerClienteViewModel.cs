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
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class VerClienteViewModel : INotifyPropertyChanged
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
        string _id;
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
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
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
        public ICommand CloseToolCommand
        {
            get
            {
                return new RelayCommand(CloseTool);
            }
        }
        #endregion

        #region Constructor
        public VerClienteViewModel()
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
                this.Id = json[0].Id.ToString();
            }
            catch (System.Exception error)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    error.Message,
                    "Aceptar");
                GetClientFromLocal();
            }
        }

        private void GetClientFromLocal()
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
                this.Id = Lis.Id;
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
        async void SaveTool()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Hola",
                "Guardar",
                "Aceptar");
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
