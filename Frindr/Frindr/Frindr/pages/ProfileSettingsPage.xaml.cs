using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Frindr.pages;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.IO;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfileSettingsPage : ContentPage
	{
        RestfulClass rest = new RestfulClass();
        Connection conn = new Connection();

        MediaFile selectedImageFilePath;

        public ProfileSettingsPage ()
		{
			InitializeComponent ();

   
            ProfileImage.Source = GlobalVariables.currentUserImage;

            RestfulClass restfulClass = new RestfulClass();
            OverlayImage.Source = restfulClass.GetImage("AddOverlay.png");

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                OpenGallery();
            };

            OverlayImage.GestureRecognizers.Add(tapGestureRecognizer);

            NameEntry.Text = GlobalVariables.loginUser.name;
            EmailEntry.Text = GlobalVariables.loginUser.email;
            LocationEntry.Text = GlobalVariables.loginUser.location;
            DescriptionEditor.Text = GlobalVariables.loginUser.description;

            try
            {
                BirthdayPicker.Date = Convert.ToDateTime(GlobalVariables.loginUser.birthday);
            }
            catch (FormatException f)
            {
                //check later
                //BirthdayPicker.Date = GlobalVariables.loginUser.birthday;
            }
            
            PrivacyFindSwitch.IsToggled = Convert.ToBoolean(GlobalVariables.loginUser.userVisible);
            PrivacyLocationSwitch.IsToggled = Convert.ToBoolean(GlobalVariables.loginUser.locationVisible);
		}

        private async void SelectHobbyButton_Clicked(object sender, EventArgs e)
        {
            IsEnabled = false;

            await Navigation.PushModalAsync(new SelectHobbyPage());

            IsEnabled = true;
        }

        private void SaveSettingsButton_Clicked(object sender, EventArgs e)
        {
            if (conn.IsOnline())
            {
                try
                {
                    using (SqliteConnection con = conn.SQLConnection)
                    {
                        con.Open();
                        
                        SqliteCommand cmd = new SqliteCommand($"UPDATE client SET name = '{NameEntry.Text}', email = '{EmailEntry.Text}'",con);
                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                    GlobalVariables.loginUser.name = NameEntry.Text;
                    GlobalVariables.loginUser.email = EmailEntry.Text;
                    GlobalVariables.loginUser.birthday = BirthdayPicker.Date.ToString("yyyyMMdd");
                    GlobalVariables.loginUser.description = DescriptionEditor.Text;
                    //imagePath needs to be added
                    GlobalVariables.loginUser.location = LocationEntry.Text;
                    GlobalVariables.loginUser.userVisible = Convert.ToInt32(PrivacyFindSwitch.IsToggled);
                    GlobalVariables.loginUser.locationVisible = Convert.ToInt32(PrivacyLocationSwitch.IsToggled);

                    string json = JsonConvert.SerializeObject(GlobalVariables.loginUser);
                    
                    //SetData's URL can not have filters in it and needs a primary key only
                    rest.SetData($"/records/user/{GlobalVariables.loginUser.id}", json);

                    string newUserData = rest.GetData($"/records/user?filter=id,eq,{GlobalVariables.loginUser.id}");
                    UserRecords newUser = JsonConvert.DeserializeObject<UserRecords>(newUserData);
                    
                    GlobalVariables.loginUser.name = newUser.records[0].name;
                    GlobalVariables.loginUser.pwd = newUser.records[0].pwd;
                    GlobalVariables.loginUser.email = newUser.records[0].email;
                    GlobalVariables.loginUser.description = newUser.records[0].description;
                    GlobalVariables.loginUser.birthday = newUser.records[0].birthday;
                    GlobalVariables.loginUser.imagePath = newUser.records[0].imagePath;
                    GlobalVariables.loginUser.location = newUser.records[0].location;
                    GlobalVariables.loginUser.locationVisible = newUser.records[0].locationVisible;
                    GlobalVariables.loginUser.userVisible = newUser.records[0].userVisible;

                    Navigation.PushModalAsync(new MenuPage());
                }
                catch (SqliteException ea)
                {
                    DisplayAlert("",ea.ToString(),"ok");
                }
            }
        }



        private async void OpenGallery()
        {
            try
            {
            await CrossMedia.Current.Initialize();

            selectedImageFilePath = await CrossMedia.Current.PickPhotoAsync();

            ProfileImage.Source = selectedImageFilePath.Path;

            RestfulClass restfulClass = new RestfulClass();

            restfulClass.UploadImage(selectedImageFilePath.Path);
            }
            catch (MediaPermissionException e)
            {

            }
        }

    }
}