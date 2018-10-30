using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frindr.pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Plugin.Messaging;
using Plugin;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OtherProfilePage : ContentPage
	{
        GlobalVariables.User SelectedUser = FriendFinderPage.SelectedUser;

        public OtherProfilePage ()
		{

			InitializeComponent();
            
            SetLabels();

            SetSelectedHobbies();

        }

        private void SetLabels()
        {
            lblEmail.Text       = SelectedUser.email;
            lblLocation.Text    = SelectedUser.location;
            lblProfileName.Text = SelectedUser.name;
            lblDescription.Text = SelectedUser.description;


            //check if location is visible
            if(SelectedUser.locationVisible == 1)
            {
                lblLocation.IsVisible = false;
            }
            else
            {
                lblLocation.IsVisible = true;
                lblLocation.Text = SelectedUser.location;
            }
        }

        private List<int> GetSelectedUserHobbies()
        {

            GlobalVariables.UserHobbyRecords userHobby = JsonConvert.DeserializeObject<GlobalVariables.UserHobbyRecords>(MainPage.UserHobby);

            List<int> hobbyIdList = new List<int>();

            foreach(GlobalVariables.UserHobby i in userHobby.records)
            {
                if(i.userId == SelectedUser.id)
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

            if(userHobbies != null)
            {
                userHobbies = userHobbies.Remove(userHobbies.Length - 2);
            }
            else
            {
                userHobbies = "Deze gebruiker heeft nog geen hobby's geselecteerd.";
            }
            
            lblHobbies.Text = userHobbies;
        }

        private void btnSendMessage_Clicked(object sender, EventArgs e)
        {
            SendEmail(SelectedUser.email);
        }


        private void SendEmail(string receiverEmail)
        {

            var emailMessenger = CrossMessaging.Current.EmailMessenger;
            if (emailMessenger.CanSendEmail)
            {

                // Alternatively use EmailBuilder fluent interface to construct more complex e-mail with multiple recipients, bcc, attachments etc.
                var email = new EmailMessageBuilder()
                  .To(receiverEmail)
                  .Subject("Bericht van Frindr")
                  .Body("Hallo " + SelectedUser.name)
                  .Build();

                emailMessenger.SendEmail(email);
            }

        }
    }
}