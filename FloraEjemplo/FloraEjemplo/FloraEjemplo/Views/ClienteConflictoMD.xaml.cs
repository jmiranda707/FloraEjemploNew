
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ClienteConflictoMD : MasterDetailPage
    {
		public ClienteConflictoMD ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            this.Master = new MenuView();
            Detail = new NavigationPage(new ClienteConflicto());
            try
            {
                //para que permita la navegacion
                App.MasterD = this;

                if (!IsPresented)
                {
                    MessagingCenter.Subscribe<ClienteConflicto>(this, "preset", (sender) =>
                    {
                        IsPresented = true;
                    });
                }
                else
                {
                    MessagingCenter.Subscribe<ClienteConflicto>(this, "preset", (sender) =>
                    {
                        IsPresented = false;
                    });
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
	}
}