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
            cnn.CreateTable<ClienteModel>();
            cnn.CreateTable<ClienteTrackingModel>();
        }

        #region ClienteModel
        public void Insertar(ClienteModel modelo)
        {
            cnn.Insert(modelo);
            Consultar();
        }
        public void Actualizar(ClienteModel modelo)
        {
            cnn.Update(modelo);
        }
        public void Eliminar(ClienteModel modelo)
        {
            cnn.Delete(modelo);
            Consultar();
        }
        public ClienteModel Consultar(string correo) //consultas segun el id
        {
            var corr = correo;

            return cnn.Table<ClienteModel>().FirstOrDefault(p => p.Mail == correo);
        }
        public List<ClienteModel> Consultar()
        {
            return cnn.Table<ClienteModel>().ToList();
        }
        public void DeleteAll()
        {
            cnn.DeleteAll<ClienteModel>();
        }
        #endregion

        #region ClienteTrackingModel
        public void InsertarClienteRegistro(ClienteTrackingModel modelo)
        {
            cnn.Insert(modelo);
            ConsultarClienteRegistro();
        }
        public void ActualizarClienteRegistro(ClienteTrackingModel modelo)
        {
            cnn.Update(modelo);
        }
        public void DeleteAllClienteRegistro()
        {
            cnn.DeleteAll<ClienteTrackingModel>();
        }
        public List<ClienteTrackingModel> ConsultarClienteRegistro()
        {
            return cnn.Table<ClienteTrackingModel>().ToList();
        }
        public List<ClienteTrackingModel> ConsultarCambios()
        {
            return cnn.Table<ClienteTrackingModel>().ToList().FindAll(p => p.Proceso != 0);
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
