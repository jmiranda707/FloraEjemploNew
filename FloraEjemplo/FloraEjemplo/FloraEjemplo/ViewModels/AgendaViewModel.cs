using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using FloraEjemplo.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class AgendaViewModel : Cliente
    {
        private ApiServices apiServices;
        public ICommand Nuevo { get; private set; }
        public ICommand Guardar { get; private set; }
        public ICommand Modificar { get; private set; }
        public ICommand Eliminar { get; private set; }
        public ICommand Volver { get; private set; }

        public  AgendaViewModel()
        {
            apiServices = new ApiServices();
            Nuevo = new Command(() => {
                Nombre = "";
                //Edad = int.Parse("Edad");
                Telefono = "";
                Mail = "";
               // Saldo = int.Parse("Saldo");
                Usuario = "";
            });
            #region Metodos

            #region Insertar/guardar
            Guardar = new Command(async() => {

                #region Validaciones
                if (string.IsNullOrEmpty(Nombre) || Edad == 0 || string.IsNullOrEmpty(Telefono) || string.IsNullOrEmpty(Mail)
               || string.IsNullOrEmpty(Usuario))
                {
                    Application.Current.MainPage.DisplayAlert("Error", "No deje campos vacios", "Entendido");
                    return;
                }
                else if (Nombre.Length < 3 || Nombre.Length > 15)
                {
                    Application.Current.MainPage.DisplayAlert("Error", "Campo nombre no debe tener menos de 3 caracteres o mas de 15 caracteres", "Ok");
                    return;
                }
                #endregion



                #region guardo primero en local
                Cliente modelo = new Cliente
                {
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    FechaCreacion = DateTime.Now,
                    FechaCreacionUtc = DateTime.UtcNow,
                    FechaModificacionLocal = DateTime.Now,
                    FechaModificacionUtcLocal = DateTime.UtcNow,
                    FechaModificacion = DateTime.Now,                //servidor
                    FechaModificacionUtc = DateTime.UtcNow,         //servidor
                    Proceso = 0,
                    Usuario = Usuario,
                    Estado = "Activo",
                    EstadoLocal = "Activo",
                    Id = "",
                    Numero = 0,
                    Sincronizado = false,
                };

                using (var contexto = new DataContext()) //aqui inserto en mi bdLocal
                {
                    contexto.Insertar(modelo);
                    ObservableCollection<Cliente> listado = new ObservableCollection<Cliente>(contexto.Consultar());
                    ListViewModel.GetInstance().ListadoPersonas = listado;
                }
                Application.Current.MainPage.DisplayAlert("Mensaje", "Datos Guardados en Local", "Entendido");
                #endregion

                #region chequeo conexion
                var result = await apiServices.CheckConnection(); //le falta el await para q funcione el issucces
                if (result.IsSuccess)//si hay conexion
                {
                    #region subir al API
                    Application.Current.MainPage.DisplayAlert("Mensaje", "Aqui se hace un Post", "ok");
                    #endregion
                }

                #endregion



            });

            #endregion

            #region Modificar/Actualizar
            Modificar = new Command(async() => {

                #region Actualizo primero Localmente
                Cliente modelo = new Cliente
                {
                    Numero = Numero,
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    FechaCreacion = FechaCreacion,
                    Proceso = Proceso,
                    Usuario = Usuario,
                    Estado = Estado,
                    EstadoLocal = EstadoLocal,
                    FechaCreacionUtc = FechaCreacionUtc,
                    FechaModificacion = FechaModificacion,
                    FechaModificacionLocal = DateTime.Now, //internamente son las unicas que cambia
                    FechaModificacionUtc = FechaModificacionUtc, //internamente son las unicas que cambia
                    FechaModificacionUtcLocal = DateTime.UtcNow,
                    Id = Id,
                    IdLocal = IdLocal,
                    Sincronizado = false, //internamente cambia si no estoy conectado a internet
                };
                using (var contexto = new DataContext())
                {
                    contexto.Actualizar(modelo);
                    ObservableCollection<Cliente> listado2 = new ObservableCollection<Cliente>(contexto.Consultar());
                    ListViewModel.GetInstance().ListadoPersonas = listado2;
                }
                #endregion

                #region chequeo conexion
                var result = await apiServices.CheckConnection(); //le falta el await para q funcione el issucces
                if (result.IsSuccess)//si hay conexion
                {
                    #region subir al API
                    Application.Current.MainPage.DisplayAlert("Mensaje", "Aqui se hace un Put", "ok");
                    #endregion
                }

                #endregion

            });

            #endregion

            #region Eliminar
            Eliminar = new Command(async() => {
                Cliente modelo = new Cliente
                {
                    Numero = Numero,
                    Nombre = Nombre,
                    Edad = Edad,
                    Telefono = Telefono,
                    Mail = Mail,
                    Saldo = Saldo,
                    FechaCreacion = FechaCreacion,
                    Proceso = Proceso,
                    Usuario = Usuario,
                    Estado = Estado,
                    EstadoLocal = "Eliminado",
                    FechaCreacionUtc = FechaCreacionUtc,
                    FechaModificacion = FechaCreacion,
                    FechaModificacionLocal = DateTime.Now, //internamente son las unicas que cambia
                    FechaModificacionUtc = FechaModificacionUtc, //internamente son las unicas que cambia
                    FechaModificacionUtcLocal = DateTime.UtcNow,
                    Id = Id,
                    IdLocal = IdLocal,
                    Sincronizado = false, //internamente cambia si no estoy conectado a internet
                };

                using (var contexto = new DataContext())
                {
                    contexto.Eliminar(modelo);
                    ObservableCollection<Cliente> listado3 = new ObservableCollection<Cliente>(contexto.Consultar());
                    ListViewModel.GetInstance().ListadoPersonas = listado3;
                    Application.Current.MainPage.Navigation.PushAsync(new Listado());
                }

                #region chequeo conexion
                var result = await apiServices.CheckConnection(); //le falta el await para q funcione el issucces
                if (result.IsSuccess)//si hay conexion
                {
                    #region subir al API
                    Application.Current.MainPage.DisplayAlert("Mensaje", "Aqui se hace un Delete", "ok");
                    #endregion
                }

                #endregion


            });

            #endregion

            Volver = new Command(async() =>
            {
               await Application.Current.MainPage.Navigation.PopModalAsync();
            });
            #endregion


        }

    }
}
