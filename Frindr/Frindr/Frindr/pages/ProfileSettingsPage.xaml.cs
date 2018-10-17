using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Frindr.pages;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfileSettingsPage : ContentPage
	{
        public ProfileSettingsPage ()
		{
			InitializeComponent ();

            NameEntry.Text = GlobalVariables.loginUser.name;
            EmailEntry.Text = GlobalVariables.loginUser.email;
            LocationEntry.Text = GlobalVariables.loginUser.location;
            BirthdayPicker.Date = Convert.ToDateTime(GlobalVariables.loginUser.birthday);
            PrivacyFindSwitch.IsToggled = Convert.ToBoolean(GlobalVariables.loginUser.userVisible);
            PrivacyLocationSwitch.IsToggled = Convert.ToBoolean(GlobalVariables.loginUser.locationVisible);
		}

        private async void SelectHobbyButton_Clicked(object sender, EventArgs e)
        {
            this.IsEnabled = false;

            await Navigation.PushModalAsync(new SelectHobbyPage());

            this.IsEnabled = true;
        }
    }
}