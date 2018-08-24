
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerCliente : ContentPage
	{
        private double width = 0;
        private double height = 0;

        public VerCliente ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<VerCliente>(this, "preset");
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    if (width > height)
                    {
                        Hamb.IsVisible = true;
                    }
                    else
                    {
                        Hamb.IsVisible = true;
                    }
                }
                else
                {
                    if (width > height)
                    {
                        Hamb.IsVisible = false;
                    }
                    else
                    {
                        Hamb.IsVisible = true;
                    }
                }
            }
        }
    }
}