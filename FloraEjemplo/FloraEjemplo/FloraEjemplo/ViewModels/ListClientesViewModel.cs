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
        bool _isVisible;
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
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        #endregion

        #region Constructors
        public ListClientesViewModel()
        {
            apiServices = new ApiServices();
            dataContext = new DataContext();
            this.IsVisible = true;
            if (Application.Current.Properties.ContainsKey("FirstUse"))
            {
                //Do things if it's NOT the first run of the app...
                LoadData();
                CheckWifiContinuosly();
                //CheckWifiOnStart();
            }
            else
            {
                Application.Current.Properties["FirstUse"] = false;
                //Do things if it IS the first run of the app...
                PrimeraSincronizacion();
            }

           
            Device.StartTimer(TimeSpan.FromSeconds(60), () =>
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
        public void hola()
        {
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
        }
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
        public async void LoadData()
        {

            var connection = await apiServices.CheckConnection();
            if (!connection.IsSuccess)
            {
                LoadClientFronLocal(); //From Local
            }
            else
            {
                //Dependiendo a la respuesta se presentaran los siguientes casos
                var cambiosPendientes = await apiServices.CheckChanges();
                if (cambiosPendientes.Codigo == 201)//Si los cambios pendientes se realizaron
                {
                    GetSincronizacion();
                }
                else if (cambiosPendientes.Codigo == 200)//Si no hay cambios pendientes por realizar
                {
                    LoadClientFronApi();
                }
                else if (cambiosPendientes.Codigo == 109)//Si el servidor esta ocupado con una sincronizacion en procesp
                {
                    CambiosPendientesSincronizar();
                }
                else //En caso de fallas las anteriores se carga desde local
                {
                    LoadClientFronLocal();
                }
            }
        }
        public async void PrimeraSincronizacion()//Primera sincronizacion
        {
            this.IsVisible = true;
            IDevice device = DependencyService.Get<IDevice>();
            string deviceIdentifier = device.GetIdentifier();
            var Tu_NombreUsuario = "José Hernández";
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
                    //+ "&Version="+version
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
                        var json = JsonConvert.DeserializeObject<List<ClienteModel>>(resulta);
                        var json2 = JsonConvert.DeserializeObject<List<ClienteTrackingModel>>(resulta);
                        var version2 = json[0].Version.ToString();
                        Application.Current.Properties["Version"] = version2;
                        await Application.Current.SavePropertiesAsync();
                        this.Clientes = new List<ClienteModel>(json);
                        this.SourceClientes = "API";
                        this.IsVisible = true;
                        Application.Current.Properties["device"] = Tu_Identificador;
                        Application.Current.Properties["Usuario"] = Tu_NombreUsuario;
                        await Application.Current.SavePropertiesAsync();

                        //Si la respuesta es correcta
                        //var listaClientesRegistro = new List<ClienteTrackingModel>(json2);
                        ////var listaClientes = this.Clientes;
                        ////almacenando en DB Borra y despues guarda
                        //dataContext.DeleteAll();
                        //dataContext.DeleteAllClienteRegistro();
                        //SaveCliente(listaClientes);
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
        public void LoadClientFronLocal()//Carga los clientes desde la DB local
        {
            this.IsVisible = true;
            this.SourceClientes = "Base de datos local";

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<ClienteModel> modelo = new List<ClienteModel>(contexto.Consultar());
                Clientes = modelo;
            }
        }
        public async void GetSincronizacion()//Codigo 201 sincronizacion realizada
        {
            try
            {
                this.IsVisible = true;
                var get = await apiServices.Sincronizacion();
                var resulta = get.Result.ToString();
                if (get.IsSuccess)
                {
                    var json = JsonConvert.DeserializeObject<List<ClienteModel>>(resulta);
                    var json2 = JsonConvert.DeserializeObject<List<ClienteTrackingModel>>(resulta);
                    if (json != null)
                    {
                        this.IsVisible = true;
                        this.Clientes = new List<ClienteModel>(json);
                        this.SourceClientes = "API";
                        //Si la respuesta es correcta
                        var listaClientesRegistro = new List<ClienteTrackingModel>(json2);
                        var listaClientes = this.Clientes;
                        //almacenando en DB Borra y despues guarda
                        dataContext.DeleteAll();
                        dataContext.DeleteAllClienteRegistro();
                        SaveCliente(listaClientes);
                    }
                    else
                    {
                        this.IsVisible = false;
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
        public async void LoadClientFronApi()//Carga usuarios desde la Api
        {
            try
            {
                this.IsVisible = true;
                var get = await apiServices.LoadClientFronApi();
                var resulta = get.Result.ToString();
                if (get.IsSuccess)
                {
                    var json = JsonConvert.DeserializeObject<List<ClienteModel>>(resulta);
                    var json2 = JsonConvert.DeserializeObject<List<ClienteTrackingModel>>(resulta);
                    if (json != null)
                    {
                        this.IsVisible = true;
                        this.Clientes = new List<ClienteModel>(json);
                        this.SourceClientes = "API";
                        //Si la respuesta es correcta
                        var listaClientesRegistro = new List<ClienteTrackingModel>(json2);
                        var listaClientes = this.Clientes;
                        //almacenando en DB Borra y despues guarda
                        dataContext.DeleteAll();
                        dataContext.DeleteAllClienteRegistro();
                        SaveCliente(listaClientes);
                    }
                    else
                    {
                        this.IsVisible = false;
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
        public async void CambiosPendientesSincronizar()//Codigo 109 Sincronizacion en proceso
        {
            try
            {
                var get = await apiServices.LoadClientFronApi();
                var resulta = get.Result.ToString();
                if (get.IsSuccess)
                {
                    var json = JsonConvert.DeserializeObject<List<ClienteModel>>(resulta);
                    if (json != null)
                    {
                        this.IsVisible = true;
                        this.Clientes = new List<ClienteModel>(json);
                        this.SourceClientes = "API";
                        //Si la respuesta es correcta
                        var listaClientes = this.Clientes;
                        //almacenando en DB Borra y despues guarda
                        dataContext.DeleteAll();
                        SaveCliente(listaClientes);
                        //Como los cambios pendientes aun no se realizan
                        //no se borrará la tabla de registro
                    }
                    else
                    {
                        this.IsVisible = false;
                        this.SourceClientes = "No hay usuarios registrados";
                        dataContext.DeleteAll();
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
        async void Registros()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ConsultaTablaRegistroMD());
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
