using SQLite.Net.Interop;

namespace FloraEjemplo.Interfaces
{
    public interface IConfiguracion
    {
        string directorio { get; }
        ISQLitePlatform plataforma { get; }
    }
}
