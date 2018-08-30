using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using FloraEjemplo.Views;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class ClienteConflictoViewModel : INotifyPropertyChanged
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
        private List<ClientsConflicts> _clientes;
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
        public List<ClientsConflicts> Clientes
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
        public ClienteConflictoViewModel()
        {
            apiServices = new ApiServices();
            dataContext = new DataContext();
            LoadClientFronLocal();
            MessagingCenter.Subscribe<ClienteConflicto>(this, "ListaConflicto", (sender) =>
            {
                LoadClientFronLocal();
            });
        }
        #endregion

        #region Commands
        public ICommand PinchaCommand
        {
            get
            {
                return new RelayCommand(LoadClientFronLocal);
            }
        }
        public ICommand BackToolCommand
        {
            get
            {
                return new RelayCommand(BackTool);
            }
        }
        public ICommand BorrarTablaCommand
        {
            get
            {
                return new RelayCommand(BorrarTabla);
            }
        }
        #endregion

        #region Methods
        public void LoadClientFronLocal()
        {
            this.SourceClientes = "Base de datos local";

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<ClientsConflicts> modelo = new List<ClientsConflicts>(contexto.ConsultarClienteConflicto());
                Clientes = modelo;
            }
        }
        async void BackTool()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private void BorrarTabla()
        {
            dataContext.DeleteAllClienteConflicto();

            LoadClientFronLocal();
        }
        #endregion

    }
}
