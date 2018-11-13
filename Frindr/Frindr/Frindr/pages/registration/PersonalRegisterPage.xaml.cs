using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using Frindr.pages;
using Xamarin.Forms.Maps;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalRegisterPage : ContentPage
    {
        RestfulClass rest = new RestfulClass();
        Connection conn = new Connection();

        public PersonalRegisterPage()
        {
            InitializeComponent();
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (conn.IsOnline())
            {

                if (CheckName(NameEntry.Text) && await CheckLocationAsync(LocationEntry.Text) && CheckAge())
                {
                    using (SqliteConnection con = conn.SQLConnection)
                    {
                        con.Open();
                        SqliteCommand cmd = new SqliteCommand($"UPDATE client SET name = '{NameEntry.Text}'", con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    GlobalVariables.loginUser.name = NameEntry.Text;
                    GlobalVariables.loginUser.location = LocationEntry.Text;
                    GlobalVariables.loginUser.birthday = BirthdayPicker.Date.ToString("yyyyMMdd");
                    GlobalVariables.loginUser.imagePath = "iets";

                    HobbyRegisterPage hobbyRegisterPage = new HobbyRegisterPage();
                    await Navigation.PushModalAsync(hobbyRegisterPage);
                }
            }
            else
            {
                await DisplayAlert("Check internet connection", "Frindr could not connect to the internet, please check your internet connection and try again", "Continue");
            }
        }

        private async Task<bool> CheckLocationAsync(string location)
        {
            string regex = @"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$";

            if (location == null)
            {
                await DisplayAlert("", "Voer een Postcode in", "ok");
                return false;
            }

            bool isLocationValid = Regex.IsMatch(location, regex);

            

            if (isLocationValid)
            {

                Geocoder geocoder = new Geocoder();
                var currentUserPosition = await geocoder.GetPositionsForAddressAsync(location);

                if(currentUserPosition == null)
                {
                    await DisplayAlert("", "Postcode is ongeldig", "ok");
                    return false;
                }

                return true;
            }
            else
            {
                await DisplayAlert("", "Postcode is ongeldig", "ok");
                return false;
            }
        }

        private bool CheckAge()
        {
            var birthday = int.Parse(BirthdayPicker.Date.ToString("yyyyMMdd"));
            var today = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
            var age = (today - birthday) / 10000;

            if (age >= 18) return true;

            else
            {
                DisplayAlert("", "Je moet minimaal 18 jaar zijn", "ok");
                return false;
            }
        }

        private bool CheckName(string name)
        {
            if (name != null)
            {
                return true;
            }
            else
            {
                DisplayAlert("", "Voer een naam in", "ok");
                return false;
            }
        }
    }
}