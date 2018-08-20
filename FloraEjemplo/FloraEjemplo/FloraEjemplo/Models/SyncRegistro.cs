using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;

namespace FloraEjemplo.Models
{
    public class SyncRegistro
    {
        [JsonProperty("Dispositivo")]
        public string Dispositivo { get; set; }
        [JsonProperty("Version")]
        public string Version { get; set; }
        [PrimaryKey, AutoIncrement, JsonProperty("Numero")]
        public int Numero { get; set; }
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("Edad")]
        public int Edad { get; set; }
        [JsonProperty("Telefono")]
        public string Telefono { get; set; }
        [JsonProperty("Mail")]
        public string Mail { get; set; }
        [JsonProperty("Saldo")]
        public double Saldo { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime FechaCreacion { get; set; }
        [JsonProperty("FechaCreacionUtc")]
        public string FechaCreacionUtc { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime FechaModificacion { get; set; }
        [JsonProperty("FechaModificacionUtc")]
        public string FechaModificacionUtc { get; set; }
        [JsonProperty("Proceso")]
        public int Proceso { get; set; }
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }
        [JsonProperty("Estado")]
        public string Estado { get; set; }
        [JsonProperty("Transaccion")]
        public string Transaccion { get; set; }
    }
}
