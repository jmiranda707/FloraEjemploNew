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
    public class DataContext : ClienteModel, IDisposable
    {
        private SQLiteConnection cnn;

        public DataContext()
        {
            var configuracion = DependencyService.Get<IConfiguracion>();
            cnn = new SQLiteConnection(configuracion.plataforma, Path.Combine(configuracion.directorio, "clienteorigin.db3"));
            cnn.CreateTable<ClienteModel>();
            cnn.CreateTable<ClienteTrackingModel>();
            cnn.CreateTable<SyncIn>();
            cnn.CreateTable<ClientsConflicts>();
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
        public void EliminarNoNulos(ClienteModel modelo)
        {
            cnn.Table<ClienteModel>().Delete(p => p.Id != "");
            Consultar();

        }
        public ClienteModel Consultar(string correo) //consultas segun el id
        {
            var corr = correo;

            return cnn.Table<ClienteModel>().FirstOrDefault(p => p.Mail == correo);
        }
        public ClienteModel ConsultarUsuario(string usuario) //consultas segun el id
        {
            var user = usuario;

            return cnn.Table<ClienteModel>().FirstOrDefault(p => p.Usuario == user);
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
        public void EliminarClienteTracking(ClienteTrackingModel modelo)
        {
            cnn.Delete(modelo);
            Consultar();
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
            //cnn.Table<Cliente>().Where(p => p.Mail == X);
            return cnn.Table<ClienteTrackingModel>().ToList().FindAll(p => p.Proceso != 0);
        }
        public ClienteTrackingModel ConsultarUsuarioTracking(string id) //consultas segun el id
        {
            var Id = id;

            return cnn.Table<ClienteTrackingModel>().FirstOrDefault(p => p.Id == Id);
        }
        public ClienteTrackingModel ConsultarCorreoTracking(string correo) //consultas segun el correo
        {
            var mail = correo;

            return cnn.Table<ClienteTrackingModel>().FirstOrDefault(p => p.Mail == mail);
        }
        #endregion

        #region Cliente Conflicto
        public void InsertarClienteConflicto(ClientsConflicts modelo)
        {
            cnn.Insert(modelo);
            ConsultarClienteConflicto();
        }
        public void ActualizarClienteConflicto(ClientsConflicts modelo)
        {
            cnn.Update(modelo);
        }
        public void EliminarClienteConflicto(ClientsConflicts modelo)
        {
            cnn.Delete(modelo);
            Consultar();
        }
        public void DeleteAllClienteConflicto()
        {
            cnn.DeleteAll<ClientsConflicts>();
        }
        public List<ClientsConflicts> ConsultarClienteConflicto()
        {
            return cnn.Table<ClientsConflicts>().ToList();
        }
        public ClientsConflicts ConsultarCorreoClienteConflicto(string correo) //consultas segun el correo
        {
            var mail = correo;

            return cnn.Table<ClientsConflicts>().FirstOrDefault(p => p.Mail == mail);
        }
        #endregion

        #region SyncIn
        public void InsertarSyncIn(SyncIn modelo)
        {
            cnn.Insert(modelo);
            ConsultarSyncIn();
        }
        public void Actualizar(SyncIn modelo)
        {
            cnn.Update(modelo);
        }
        public void Eliminar(SyncIn modelo)
        {
            cnn.Delete(modelo);
            ConsultarSyncIn();
        }
        public SyncIn ConsultarSyncInFirst() 
        {
            return cnn.Table<SyncIn>().FirstOrDefault();
        }
        public List<SyncIn> ConsultarSyncIn()
        {
            return cnn.Table<SyncIn>().ToList();
        }
        public void DeleteAllSyncIn()
        {
            cnn.DeleteAll<SyncIn>();
        }
        #endregion

        public void Dispose()
        {
            cnn.Dispose();
        }


    }
}
