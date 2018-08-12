
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditarClienteMD : MasterDetailPage
    {
		public EditarClienteMD ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            this.Master = new MenuView();
            Detail = new NavigationPage(new EditarCliente());
            try
            {
                //para que permita la navegacion
                App.MasterD = this;

                if (!IsPresented)
                {
                    MessagingCenter.Subscribe<EditarCliente>(this, "preset", (sender) =>
                    {
                        IsPresented = true;
                    });
                }
                else
                {
                    MessagingCenter.Subscribe<EditarCliente>(this, "preset", (sender) =>
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