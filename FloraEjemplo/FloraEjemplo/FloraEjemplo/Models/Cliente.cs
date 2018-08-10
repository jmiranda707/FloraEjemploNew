using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FloraEjemplo.Models
{
    public class Cliente : INotifyPropertyChanged
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
        private int _numero;
        private string _id;
        private string _nombre;
        private int _edad;
        private string _telefono;
        private string _mail;
        private double _saldo;
        private DateTime _fechaCreacion;
        private DateTime _fechaCreacionUtc;
        private DateTime _fechaModificacionlocal;
        private DateTime _fechaModificacionUtclocal;
        private DateTime _fechaModificacion;
        private DateTime _fechaModificacionUtc;
        private int _proceso;
        private string _usuario;
        private string _estado;
        private string _estadolocal;
        private int _idlocal;
        private bool _sincronizado;
        #endregion

        #region Properties
        [PrimaryKey, AutoIncrement]
        public int IdLocal //idlocal de la app
        {
            get { return _idlocal; }
            set
            {
                _idlocal = value;
                OnPropertyChanged("IdLocal");
            }
        }
        public int Numero
        {
            get { return _numero; }
            set
            {
                _numero = value;
                OnPropertyChanged("Numero");
            }
        }
        public string Id //id del servidor
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
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
        public DateTime FechaCreacion
        {
            get { return _fechaCreacion; }
            set
            {
                _fechaCreacion = value;
                OnPropertyChanged("FechaCreacion");
            }
        }
        public DateTime FechaCreacionUtc
        {
            get { return _fechaCreacionUtc; }
            set
            {
                _fechaCreacionUtc = value;
                OnPropertyChanged("FechaCreacionUtc");
            }
        }
        public DateTime FechaModificacion
        {
            get { return _fechaModificacion; }
            set
            {
                _fechaModificacion = value;
                OnPropertyChanged("FechaModificacion");
            }
        }
        public DateTime FechaModificacionUtc
        {
            get { return _fechaModificacionUtc; }
            set
            {
                _fechaModificacionUtc = value;
                OnPropertyChanged("FechaModificacionUtc");
            }
        }
        public int Proceso
        {
            get { return _proceso; }
            set
            {
                _proceso = value;
                OnPropertyChanged("Proceso");
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
        public bool Sincronizado
        {
            get { return _sincronizado; }
            set
            {
                _sincronizado = value;
                OnPropertyChanged("Sincronizado");
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
        public string EstadoLocal
        {
            get { return _estadolocal; }
            set
            {
                _estadolocal = value;
                OnPropertyChanged("EstadoLocal");
            }
        }
        public DateTime FechaModificacionLocal
        {
            get { return _fechaModificacionlocal; }
            set
            {
                _fechaModificacionlocal = value;
                OnPropertyChanged("FechaModificacionLocal");
            }
        }
        public DateTime FechaModificacionUtcLocal
        {
            get { return _fechaModificacionUtclocal; }
            set
            {
                _fechaModificacionUtclocal = value;
                OnPropertyChanged("FechaModificacionUtcLocal");
            }
        }
        #endregion
    }

    public class ClientePadre
    {
        public List<Cliente> data { set; get; }
    }
}