﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frindr.pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

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


            //check if location is visible
            if(SelectedUser.locationVisible == 0)
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

    }
}