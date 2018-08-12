using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FloraEjemplo.Models
{
    public class Cliente2
    {
        public string Id
        {
            get;
            set;
        }
        public string Nombre
        {
            get;
            set;
        }
        public int Edad
        {
            get;
            set;
        }
        public string Telefono
        {
            get;
            set;
        }
        public string Mail
        {
            get;
            set;
        }
        public double Saldo
        {
            get;
            set;
        }
        public string FechaCreacion
        {
            get;
            set;
        }
        public string FechaCreacionUtc
        {
            get;
            set;
        }
        public string FechaModificacion
        {
            get;
            set;
        }
        public string FechaModificacionUtc
        {
            get;
            set;
        }
        public int Proceso
        {
            get;
            set;
        }
        public string Usuario
        {
            get;
            set;
        }
        public string Estado
        {
            get;
            set;
        }

        /// Uso Interno de BDLocal ///
        [PrimaryKey, AutoIncrement]
        public int IdLocal
        {
            get;
            set;
        }                        // idlocal de la app ///
        public int Numero
        {
            get;
            set;
        }                        //// esta es del api ////
        public bool Sincronizado
        {
            get;
            set;
        }
        public string EstadoLocal
        {
            get;
            set;
        }
        public string FechaCreacionLocal
        {
            get;
            set;
        }
        public string FechaCreacionUtcLocal
        {
            get;
            set;
        }
        public DateTime FechaModificacionLocal
        {
            get;
            set;
        }
        public DateTime FechaModificacionUtcLocal
        {
            get;
            set;
        }
    }
}
