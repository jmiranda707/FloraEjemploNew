using SQLite.Net.Attributes;
using System;
using System.ComponentModel;

namespace FloraEjemplo.Models
{
    public class AgendaModel : INotifyPropertyChanged
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

        #region Metodos
        /* protected virtual void OnPropertyChanged(string propiedad)
         {
             if (PropertyChanged != null)
             {
                 PropertyChanged(this, new PropertyChangedEventArgs(propiedad));
             }
         }*/
        #endregion

        #region Atributos
        //private bool _isRefreshing;
        private int _idPersona;
        private string _nombre;
        private string _apellido;
        private string _correo;
        private string _pais;
        private string _ciudad;
        private DateTime _fecha;
        private long _telefono;
        #endregion

        #region Propiedades
        [PrimaryKey, AutoIncrement]
        public int IdPersona
        {
            get { return _idPersona; }
            set
            {
                _idPersona = value;
                OnPropertyChanged("IdPersona");
            }
        }
        public string Nombre
        {
            get { return _nombre; }
            set
            {
                _nombre = value;
                OnPropertyChanged("Nombre");
            }
        }
        public string Apellido
        {
            get { return _apellido; }
            set
            {
                _apellido = value;
                OnPropertyChanged("Apellido");
            }
        }
        public string Correo
        {
            get { return _correo; }
            set
            {
                _correo = value;
                OnPropertyChanged("Correo");
            }
        }
        public string Pais
        {
            get { return _pais; }
            set
            {
                _pais = value;
                OnPropertyChanged("Pais");
            }
        }
        public string Ciudad
        {
            get { return _ciudad; }
            set
            {
                _ciudad = value;
                OnPropertyChanged("Ciudad");
            }
        }
        public DateTime Fecha
        {
            get { return _fecha; }
            set
            {
                _fecha = value;
                OnPropertyChanged("Fecha");
            }
        }
        public long Telefono
        {
            get { return _telefono; }
            set
            {
                _telefono = value;
                OnPropertyChanged("Telefono");
            }
        }
        /*public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRefreshing)));
                    //OnPropertyChanged("_nombre");
                }
            }
        }*/
        #endregion
    }
}
