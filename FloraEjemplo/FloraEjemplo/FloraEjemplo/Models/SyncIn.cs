
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace FloraEjemplo.Models
{
    public class SyncIn
    {
        [PrimaryKey, AutoIncrement, JsonIgnore]
        public int Posicion { get; set; }
        public string Id { get; set; }
        public bool Resultado { get; set; }
        public int Error { get; set; }
        public string Version { get; set; }
    }
}
