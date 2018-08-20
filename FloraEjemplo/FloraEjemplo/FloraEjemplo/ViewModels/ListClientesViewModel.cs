using FloraEjemplo.Data;
using FloraEjemplo.Interfaces;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using FloraEjemplo.Views;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        private List<ClienteModel> _clientes;
        DataContext dataContext;
        string _conn;
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
        public List<ClienteModel> Clientes
        {
            get { return _clientes; }
            set
            {
                _clientes = value;
                OnPropertyChanged("Clientes");
            }
        }
        public string Conn
        {
            get { return _conn; }
            set
            {
                _conn = value;
                OnPropertyChanged("Conn");
            }
        }
        #endregion

        #region Constructors
        public ListClientesViewModel()
        {
            apiServices = new ApiServices();
            dataContext = new DataContext();
            //PrimeraSincronizacion();
            LoadData();
            CheckWifiContinuosly();
            CheckWifiOnStart();
            MessagingCenter.Subscribe<EdiarClienteViewModel>(this, "EjecutaLista", (sender) =>
            {
                LoadData();
            });
            MessagingCenter.Subscribe<AddClienteViewModel>(this, "EjecutaLista", (sender) =>
            {
                LoadData();
            });
            MessagingCenter.Subscribe<ListaClientes>(this, "EjecutaLista", (sender) =>
            {
                LoadData();
            });
            MessagingCenter.Subscribe<App>(this, "EjecutaLista", (sender) =>
            {
                LoadData();
            });
            Device.StartTimer(TimeSpan.FromSeconds(363), () =>
            {
                Task.Run(() =>
                {
                    LoadData();
                });
                return true;
            });

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
        public ICommand DeviceIdentifierCommand
        {
            get
            {
                return new RelayCommand(DeviceIdentifier);
            }
        }
        #endregion

        #region Methods
        public void CheckWifiContinuosly()
        {
            Conn = CrossConnectivity.Current.IsConnected ? "online.png" : "offline";
        }
        public void CheckWifiOnStart()
        {
            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            {
                Conn = args.IsConnected ? "online.png" : "offline";
                LoadData();
            };
        }
        async void Registros()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ConsultaTablaRegistroMD());
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
                //PrimeraSincronizacion(); //From Api
            }
        }
        public async void PrimeraSincronizacion()
        {
            IDevice device = DependencyService.Get<IDevice>();
            string deviceIdentifier = device.GetIdentifier();
            var Tu_NombreUsuario = "Eleazar Saavedra";
            var Tu_Identificador = deviceIdentifier;
            var cambiosPendientes = await apiServices.CheckChanges();
            if (cambiosPendientes.IsSuccess)
            {
                try
                {
                    string resultado = string.Empty;
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    //http://efrain1234-001-site1.ftempurl.com/api/SyncSeleccion?Usuario=Tu-Usuario&Dispositivo=Tu_Identificador
                    //http://efrain1234-001-site1.ftempurl.com/api/SyncSeleccion?Usuario=Tu-Usuario&Dispositivo=Tu_Identificador
                    HttpResponseMessage response = await httpClient.GetAsync(
                        "http://efrain1234-001-site1.ftempurl.com/api/SyncSeleccion?Usuario=" + Tu_NombreUsuario + "&Dispositivo=" + Tu_Identificador);
                    var result = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {

                        resultado = response.Content.ReadAsStringAsync().Result;
                        resultado = resultado.Replace("\\", "");
                        resultado = resultado.Replace("/", "");
                        resultado = resultado.Replace("\"[", "[");
                        resultado = resultado.Replace("]\"", "]");
                        var resulta = resultado;
                        //var get = await apiServices.LoadClientFronApi();

                        //if (get.IsSuccess)
                        //{
                        var json = JsonConvert.DeserializeObject<List<ClienteModel>>(resulta);
                        var json2 = JsonConvert.DeserializeObject<List<ClienteTrackingModel>>(resulta);
                        this.Clientes = new List<ClienteModel>(json);
                        this.SourceClientes = "API";

                        //Si la respuesta es correcta
                        var listaClientesRegistro = new List<ClienteTrackingModel>(json2);
                        var listaClientes = this.Clientes;
                        //almacenando en DB Borra y despues guarda
                        dataContext.DeleteAll();
                        dataContext.DeleteAllClienteRegistro();
                        SaveCliente(listaClientes);
                        //SaveClienteRegistro(listaClientesRegistro);
                        //}
                        //else
                        //{

                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        response.RequestMessage.ToString(),
                        "Aceptar");
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
        }
        public void LoadClientFronLocal()
        {
            this.SourceClientes = "Base de datos local";

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<ClienteModel> modelo = new List<ClienteModel>(contexto.Consultar());
                Clientes = modelo;
            }
        }
        public async void LoadClientFronApi()
        {
            var cambiosPendientes = await apiServices.CheckChanges();
            if (cambiosPendientes.IsSuccess)
            {
                try
                {
                    var get = await apiServices.LoadClientFronApi();
                    var resulta = get.Result.ToString();
                    if (get.IsSuccess)
                    {
                        var json = JsonConvert.DeserializeObject<List<ClienteModel>>(resulta);
                        var json2 = JsonConvert.DeserializeObject<List<ClienteTrackingModel>>(resulta);
                        if (json != null)
                        {
                            this.Clientes = new List<ClienteModel>(json);
                            this.SourceClientes = "API";
                            //Si la respuesta es correcta
                            var listaClientesRegistro = new List<ClienteTrackingModel>(json2);
                            var listaClientes = this.Clientes;
                            //almacenando en DB Borra y despues guarda
                            dataContext.DeleteAll();
                            dataContext.DeleteAllClienteRegistro();
                            SaveCliente(listaClientes);
                            //SaveClienteRegistro(listaClientesRegistro);
                        }
                        else
                        {
                            this.SourceClientes = "No hay usuarios registrados";
                            dataContext.DeleteAll();
                            dataContext.DeleteAllClienteRegistro();
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        get.Message.ToString(),
                        "Aceptar");
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
            else
            {
                LoadClientFronLocal();
            }
        }
        void SaveClienteRegistro(List<ClienteTrackingModel> listaClientesRegistro)
        {
            using (var da = new DataContext())
            {
                foreach (var record in listaClientesRegistro)
                {
                    InsertOrUpdateSaveClienteRegistro(record);
                }
            }
        }
        void InsertOrUpdateSaveClienteRegistro(ClienteTrackingModel record)
        {
            using (var da = new DataContext())
            {
                da.InsertarClienteRegistro(record);
            }
        }
        void SaveCliente(List<ClienteModel> listaClientes)
        {
            using (var da = new DataContext())
            {
                foreach (var record in listaClientes)
                {
                    InsertOrUpdateSaveCliente(record);
                }
            }
        }
        void InsertOrUpdateSaveCliente(ClienteModel record)
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
        private static void DeviceIdentifier()
        {

            IDevice device = DependencyService.Get<IDevice>();
            string deviceIdentifier = device.GetIdentifier();

            Application.Current.MainPage.DisplayAlert("Indetificador de Dispositivo", deviceIdentifier, "Ok");
        }
        #endregion
    }
}

//if (resultado == string.empty)
//{
//no hay datos}
//if(resultado == "ExisteSync")
//{
//Otro usuario se sincroniza}
//els
//{
//resultado = desserializa la clase cliente
//}
