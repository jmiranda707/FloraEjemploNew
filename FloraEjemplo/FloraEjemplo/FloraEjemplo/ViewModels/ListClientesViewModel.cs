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
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class ListClientesViewModel : INotifyPropertyChanged
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
        DataContext dataContext;
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
            dataContext = new DataContext();
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
        public ICommand RegistrosCommand
        {
            get
            {
                return new RelayCommand(Registros);
            }
        }
        #endregion

        #region Methods
        async void Registros()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ConsultaTablaRegistro());
        }
        public async void LoadData()
        {
            var connection = await apiServices.CheckConnection();
            if (!connection.IsSuccess)
            {
                LoadClientFronLocal(); //From Local

            }
            else
            {
                LoadClientFronApi(); //From Api
            }
        }

        public async void LoadClientFronLocal()
        {
            this.SourceClientes = "Base de datos local";

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<Cliente2> modelo = new List<Cliente2>(contexto.Consultar());
                Clientes = modelo;
            }

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
                    var json2 = JsonConvert.DeserializeObject<List<ClienteRegistro>>(resulta);
                    var nombre = json[0].Nombre.ToString();
                    var edad = json[0].Edad.ToString();
                    var estado = json[0].Estado.ToString();
                    var telefono = json[0].Telefono.ToString();
                    this.Clientes = new List<Cliente2>(json);
                    this.SourceClientes = "API";

                    //Si la respuesta es correcta 
                    var listaClientesRegistro = new List<ClienteRegistro>(json2);
                    var listaClientes = this.Clientes;
                    //almacenando en DB Borra y despues guarda
                    dataContext.DeleteAll();
                    dataContext.DeleteAllClienteRegistro();
                    SaveCliente(listaClientes);
                    SaveClienteRegistro(listaClientesRegistro);
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
        void SaveClienteRegistro(List<ClienteRegistro> listaClientesRegistro)
        {
            using (var da = new DataContext())
            {
                foreach (var record in listaClientesRegistro)
                {
                    InsertOrUpdateSaveClienteRegistro(record);
                }
            }
        }
        void InsertOrUpdateSaveClienteRegistro(ClienteRegistro record)
        {
            using (var da = new DataContext())
            {
                da.InsertarClienteRegistro(record);
            }
        }
        void SaveCliente(List<Cliente2> listaClientes)
        {
            using (var da = new DataContext())
            {
                foreach (var record in listaClientes)
                {
                    InsertOrUpdateSaveCliente(record);
                }
            }
        }
        void InsertOrUpdateSaveCliente(Cliente2 record)
        {
            using (var da = new DataContext())
            {
                da.Insertar(record);
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
