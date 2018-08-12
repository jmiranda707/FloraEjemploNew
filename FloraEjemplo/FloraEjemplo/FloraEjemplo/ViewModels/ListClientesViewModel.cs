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
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    class ListClientesViewModel : INotifyPropertyChanged
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
        string _sourceClientes;
        private ApiServices apiServices;
        private List<Cliente2> _clientes;
        #endregion

        #region Properties
        public string SourceClientes
        {
            get { return _sourceClientes; }
            set
            {
                _sourceClientes = value;
                OnPropertyChanged("SourceClientes");
            }
        }
        public List<Cliente2> Clientes
        {
            get { return _clientes; }
            set
            {
                _clientes = value;
                OnPropertyChanged("Clientes");
            }
        }
        #endregion

        #region Constructors
        public ListClientesViewModel()
        {
            apiServices = new ApiServices();
            LoadData();
        }
        #endregion

        #region Commands
        public ICommand PinchaCommand
        {
            get
            {
                return new RelayCommand(LoadData);
            }
        }
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
        public ICommand AddToolCommand
        {
            get
            {
                return new RelayCommand(AddTool);
            }
        }
        #endregion

        #region Methods
        public async void LoadData()
        {
            var connection = await apiServices.CheckConnection();
            if (!connection.IsSuccess)
            {
                LoadClientFronLocal();

            }
            else
            {
                LoadClientFronApi();
            }
        }

        public void LoadClientFronLocal()
        {
            this.SourceClientes = "Base de datos local";
        }

        public async void LoadClientFronApi()
        {
            try
            {
                string resultado = string.Empty;
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                HttpResponseMessage response = await httpClient.GetAsync("http://efrain1234-001-site1.ftempurl.com/api/Cliente");
                var result = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    resultado = response.Content.ReadAsStringAsync().Result;
                    resultado = resultado.Replace("\\", "");
                    resultado = resultado.Replace("/", "");
                    resultado = resultado.Replace("\"[", "[");
                    resultado = resultado.Replace("]\"", "]");
                    var resulta = resultado;
                    var json = JsonConvert.DeserializeObject<List<Cliente2>>(resulta);
                    var nombre = json[0].Nombre.ToString();
                    var edad = json[0].Edad.ToString();
                    var estado = json[0].Estado.ToString();
                    var telefono = json[0].Telefono.ToString();

                    this.Clientes = new List<Cliente2>(json);
                    this.SourceClientes = "API";
                }
                else
                {
                    resultado = response.IsSuccessStatusCode.ToString();
                }
            }
            catch (System.Exception error)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    error.Message,
                    "Aceptar");
            }
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
        async void AddTool()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AddClienteMD());
        }
        #endregion
    }
}
