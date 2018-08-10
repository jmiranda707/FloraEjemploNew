using FloraEjemplo.Interfaces;
using FloraEjemplo.Models;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace FloraEjemplo.Data
{
    public class DataContext : AgendaModel, IDisposable
    {

        private SQLiteConnection cnn;

        public DataContext()
        {
            var configuracion = DependencyService.Get<IConfiguracion>();
            cnn = new SQLiteConnection(configuracion.plataforma, Path.Combine(configuracion.directorio, "client.db3"));
            cnn.CreateTable<Cliente>();
        }


        public void Insertar(Cliente modelo)
        {
            cnn.Insert(modelo);
            Consultar();
        }

        public void Actualizar(Cliente modelo)
        {
            cnn.Update(modelo);
        }

        public void Eliminar(Cliente modelo)
        {
            cnn.Delete(modelo);
            Consultar();
        }

        public Cliente Consultar(int id)
        {
            return cnn.Table<Cliente>().FirstOrDefault(p => p.Numero == id);
        }

        public List<Cliente> Consultar()
        {
            return cnn.Table<Cliente>().ToList();
        }

        public void Dispose()
        {
            cnn.Dispose();
        }
    }
}
