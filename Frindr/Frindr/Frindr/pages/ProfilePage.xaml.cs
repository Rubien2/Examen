using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr.pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{

        public ProfilePage ()
		{
            InitializeComponent();
            lblProfileName.Text = GlobalVariables.loginUser.name;
            lblEmail.Text = GlobalVariables.loginUser.email;
            lblLocation.Text = GlobalVariables.loginUser.location;
            lblDescription.Text = GlobalVariables.loginUser.description;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void btnSettings_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ProfileSettingsPage());
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
            
            //return base.OnBackButtonPressed();
        }
    }
}