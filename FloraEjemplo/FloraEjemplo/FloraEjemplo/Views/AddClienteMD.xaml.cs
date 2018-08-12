
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddClienteMD : MasterDetailPage
    {
		public AddClienteMD ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            this.Master = new MenuView();
            Detail = new NavigationPage(new AddCliente());
            try
            {
                //para que permita la navegacion
                App.MasterD = this;

                if (!IsPresented)
                {
                    MessagingCenter.Subscribe<AddCliente>(this, "preset", (sender) =>
                    {
                        IsPresented = true;
                    });
                }
                else
                {
                    MessagingCenter.Subscribe<AddCliente>(this, "preset", (sender) =>
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