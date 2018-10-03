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
	public partial class ProfileSettingsPage : ContentPage
	{

        

        public ProfileSettingsPage ()
		{
			InitializeComponent ();
		}

        private async void SelectHobbyButton_Clicked(object sender, EventArgs e)
        {
            this.IsEnabled = false;
   
            //SelectHobbyButton.IsEnabled = false;
            await Navigation.PushModalAsync(new pages.SelectHobbyPage());

            this.IsEnabled = true;
        }



    }
}