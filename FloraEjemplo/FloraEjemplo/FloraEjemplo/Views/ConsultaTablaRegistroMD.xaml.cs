﻿
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FloraEjemplo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConsultaTablaRegistroMD : MasterDetailPage
    {
		public ConsultaTablaRegistroMD ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            this.Master = new MenuView();
            Detail = new NavigationPage(new ConsultaTablaRegistro());
            try
            {
                //para que permita la navegacion
                App.MasterD = this;

                if (!IsPresented)
                {
                    MessagingCenter.Subscribe<ConsultaTablaRegistro>(this, "preset", (sender) =>
                    {
                        IsPresented = true;
                    });
                }
                else
                {
                    MessagingCenter.Subscribe<ConsultaTablaRegistro>(this, "preset", (sender) =>
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