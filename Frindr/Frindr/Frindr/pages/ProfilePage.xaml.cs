using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            SetSelectedHobbies();
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
        private List<int> GetSelectedUserHobbies()
        {

            GlobalVariables.UserHobbyRecords userHobby = JsonConvert.DeserializeObject<GlobalVariables.UserHobbyRecords>(MainPage.UserHobby);

            List<int> hobbyIdList = new List<int>();

            foreach (GlobalVariables.UserHobby i in userHobby.records)
            {
                if (i.userId == GlobalVariables.loginUser.id)
                {

                    hobbyIdList.Add(i.hobbyId);

                }
            }

            return hobbyIdList;
        }

        private void SetSelectedHobbies()
        {
            RestfulClass restfulClass = new RestfulClass();

            List<int> selectedUserHobbies = GetSelectedUserHobbies();

            GlobalVariables.HobbyRecords hobbies = JsonConvert.DeserializeObject<GlobalVariables.HobbyRecords>(MainPage.Hobbies);

            string userHobbies = null;

            foreach (GlobalVariables.Hobbies i in hobbies.records)
            {
                if (selectedUserHobbies.Contains(i.id))
                {
                    userHobbies += i.hobby + ", ";
                }
            }

            if (userHobbies != null)
            {
                userHobbies = userHobbies.Remove(userHobbies.Length - 2);
            }
            else
            {
                userHobbies = "Deze gebruiker heeft nog geen hobby's geselecteerd.";
            }

            lblHobbies.Text = userHobbies;
        }
    }
}