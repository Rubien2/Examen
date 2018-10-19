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

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfileSettingsPage : ContentPage
	{
        RestfulClass rest = new RestfulClass();
        Connection conn = new Connection();

        public ProfileSettingsPage ()
		{
			InitializeComponent ();

            NameEntry.Text = GlobalVariables.loginUser.name;
            EmailEntry.Text = GlobalVariables.loginUser.email;
            LocationEntry.Text = GlobalVariables.loginUser.location;
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
            this.IsEnabled = false;

            await Navigation.PushModalAsync(new SelectHobbyPage());

            this.IsEnabled = true;
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

                    string nameCheck = GlobalVariables.loginUser.name;
                    string pwCheck = GlobalVariables.loginUser.pwd;

                    GlobalVariables.loginUser.name = NameEntry.Text;
                    GlobalVariables.loginUser.email = EmailEntry.Text;
                    GlobalVariables.loginUser.birthday = BirthdayPicker.Date.ToString("yyyyMMdd");
                    GlobalVariables.loginUser.location = LocationEntry.Text;
                    GlobalVariables.loginUser.userVisible = Convert.ToInt32(PrivacyFindSwitch.IsToggled);
                    GlobalVariables.loginUser.locationVisible = Convert.ToInt32(PrivacyLocationSwitch.IsToggled);

                    string json = JsonConvert.SerializeObject(GlobalVariables.loginUser);

                    string getData = rest.GetData($"/records/user?filter=name,eq,{nameCheck}&filter=pwd,eq,{pwCheck}");
                    UserRecords user = JsonConvert.DeserializeObject<UserRecords>(getData);

                    rest.SetData($"/records/user?filter=name,eq,{user.records[0].id}", json);
                }
                catch (SqliteException ea)
                {
                    DisplayAlert("",ea.ToString(),"ok");
                }
            }
        }
    }
}