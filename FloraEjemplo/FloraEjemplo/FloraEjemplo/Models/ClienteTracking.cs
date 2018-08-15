
using SQLite.Net.Attributes;

namespace FloraEjemplo.Models
{
    public class ClienteTracking
    {
        [PrimaryKey, AutoIncrement]
        public int Numero { get; set; }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public long Saldo { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaCreacionUtc { get; set; }
        public string FechaModificacion { get; set; }
        public string FechaModificacionUtc { get; set; }
        public int Proceso { get; set; }
        public string Usuario { get; set; }
        public string Estado { get; set; }
        //public object Transaccion { get; set; }
    }
}
