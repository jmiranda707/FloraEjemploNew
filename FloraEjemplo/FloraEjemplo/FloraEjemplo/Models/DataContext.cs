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
            cnn = new SQLiteConnection(configuracion.plataforma, Path.Combine(configuracion.directorio, "clienteorigin.db3"));
            cnn.CreateTable<Cliente2>();
            cnn.CreateTable<Cliente>();
            cnn.CreateTable<ClienteRegistro>();

        }

        #region Cliente2
        public void Insertar(Cliente2 modelo)
        {
            cnn.Insert(modelo);
            Consultar();
        }
        public void Actualizar(Cliente2 modelo)
        {
            cnn.Update(modelo);
        }
        public void Eliminar(Cliente2 modelo)
        {
            cnn.Delete(modelo);
            Consultar();
        }
        public Cliente2 Consultar(string correo) //consultas segun el id
        {
            return cnn.Table<Cliente2>().FirstOrDefault(p => p.Mail == correo);
        }
        public List<Cliente2> Consultar()
        {
            return cnn.Table<Cliente2>().ToList();
        }
        public void DeleteAll()
        {
            cnn.DeleteAll<Cliente2>();
        }
        #endregion

        #region ClienteRegistro
        public void InsertarClienteRegistro(ClienteRegistro modelo)
        {
            cnn.Insert(modelo);
            Consultar();
        }
        public void ActualizarClienteRegistro(ClienteRegistro modelo)
        {
            cnn.Update(modelo);
        }
        public void DeleteAllClienteRegistro()
        {
            cnn.DeleteAll<ClienteRegistro>();
        }
        public List<ClienteRegistro> ConsultarClienteRegistro()
        {
            return cnn.Table<ClienteRegistro>().ToList();
        }
        #endregion

        public void Dispose()
        {
            cnn.Dispose();
        }

        /// <summary>
        /// /mi tabla anteriorrr
        /// </summary>
        /// <param name="modelo"></param>
        public void Insertara(Cliente modelo)
        {
            cnn.Insert(modelo);
        }

        public void Actualizara(Cliente modelo)
        {
            cnn.Update(modelo);
        }

        public void Eliminara(Cliente modelo)
        {
            cnn.Delete(modelo);
        }

        public Cliente Consultara(int id) //consultas segun el id
        {
            return cnn.Table<Cliente>().FirstOrDefault(p => p.IdLocal == id);
        }

        public List<Cliente> Consultara()
        {
            return cnn.Table<Cliente>().ToList();
        }

    }
}
