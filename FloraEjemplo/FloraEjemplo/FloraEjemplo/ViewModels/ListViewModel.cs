using FloraEjemplo.Data;
using FloraEjemplo.Models;
using FloraEjemplo.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace FloraEjemplo.ViewModels
{
    public class ListViewModel : Cliente
    {
        private ApiServices apiServices;
        private ObservableCollection<Cliente> _listadoPersonas;

        public ObservableCollection<Cliente> ListadoPersonas
        {
            get
            {
                if (_listadoPersonas == null)
                {
                    LlenarPersonas();
                }
                return _listadoPersonas;

            }
            set
            {
                _listadoPersonas = value;

            }
        }

        public async void LlenarPersonas()
        {
            var result = await apiServices.CheckConnection();
            if (!result.IsSuccess)//si no hay conexion
            {
                #region obtener datos de Local si no hay internet
                using (var contexto = new DataContext())
                {
                    ObservableCollection<Cliente> modelo = new ObservableCollection<Cliente>(contexto.Consultara());
                    ListadoPersonas = modelo;
                }
                #endregion
            }
            else //aqui si estoy conectado, pido los datos al api
            {
               await Application.Current.MainPage.DisplayAlert("mensaje", "aca va un post", "ok");
            }


        }
        public ListViewModel()
        {
            instance = this;
            apiServices = new ApiServices();
        }
        #region Singleton 
        private static ListViewModel instance;
        public static ListViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ListViewModel();
            }
            return instance;
        }
        #endregion

    }
}
