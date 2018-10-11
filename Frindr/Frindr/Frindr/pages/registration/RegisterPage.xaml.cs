using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage ()
		{
			InitializeComponent ();
		}

        private void GoToLoginButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new LoginPage());
        }

        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new MenuPage());
        }

        private void SelectHobbyButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new pages.SelectHobbyPage());
        }
    }
}