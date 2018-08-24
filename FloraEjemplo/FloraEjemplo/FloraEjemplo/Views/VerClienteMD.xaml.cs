
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerClienteMD : MasterDetailPage
    {
		public VerClienteMD ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            this.Master = new MenuView();
            Detail = new NavigationPage(new VerCliente());
            try
            {
                //para que permita la navegacion
                App.MasterD = this;

                if (!IsPresented)
                {
                    MessagingCenter.Subscribe<VerCliente>(this, "preset", (sender) =>
                    {
                        IsPresented = true;
                    });
                }
                else
                {
                    MessagingCenter.Subscribe<VerCliente>(this, "preset", (sender) =>
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