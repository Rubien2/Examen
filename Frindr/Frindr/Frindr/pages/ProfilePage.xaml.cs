using System;
using System.Collections.Generic;
using System.IO;
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
            lblProfileName.Text = "Naam: " + GlobalVariables.loginUser.name;
            lblEmail.Text = "Email: " + GlobalVariables.loginUser.email;
            lblLocation.Text = "Locatie: " + GlobalVariables.loginUser.location;
            lblDescription.Text = GlobalVariables.loginUser.description;

            try
            {

            RestfulClass restfulClass = new RestfulClass();

            var dataBuffer = restfulClass.GetImage(GlobalVariables.loginUser.imagePath);
            GlobalVariables.currentUserImage = dataBuffer;

            ProfileImage.Source = GlobalVariables.currentUserImage;
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }



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