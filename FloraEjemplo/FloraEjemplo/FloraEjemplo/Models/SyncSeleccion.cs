using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;

namespace FloraEjemplo.Models
{
    public class SyncSeleccion
    {
        [PrimaryKey, AutoIncrement]
        public int Numero { get; set; }
        public string Dispositivo { get; set; }
        public string Version { get; set; }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public double Saldo { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime FechaCreacion { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime FechaCreacionUtc { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime FechaModificacion { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime FechaModificacionUtc { get; set; }
        public int Proceso { get; set; }
        public string Usuario { get; set; }
        public string Estado { get; set; }
        public string Transaccion { get; set; }
    }
}
