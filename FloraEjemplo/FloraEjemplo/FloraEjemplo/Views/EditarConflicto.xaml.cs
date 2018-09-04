
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditarConflicto : ContentPage
	{
		public EditarConflicto ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }
	}
}