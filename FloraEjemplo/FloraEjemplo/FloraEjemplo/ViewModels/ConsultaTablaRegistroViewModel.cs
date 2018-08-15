using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class ConsultaTablaRegistroViewModel : INotifyPropertyChanged
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
        private List<ClienteTrackingModel> _clientes;
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
        public List<ClienteTrackingModel> Clientes
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
        public ConsultaTablaRegistroViewModel()
        {
            apiServices = new ApiServices();
            dataContext = new DataContext();
            LoadClientFronLocal();
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
        #endregion

        #region Methods
        public void LoadClientFronLocal()
        {
            this.SourceClientes = "Base de datos local";

            using (var contexto = new DataContext()) //para obtener todos mis Clientes desde Local
            {
                List<ClienteTrackingModel> modelo = new List<ClienteTrackingModel>(contexto.ConsultarClienteRegistro());
                Clientes = modelo;
            }
        }
        
        async void BackTool()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        
        #endregion
    }
}
