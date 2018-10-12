using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

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

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            if (conn.IsOnline())
            {
                if (CheckName(NameEntry.Text) && CheckLocation(LocationEntry.Text) && CheckAge())
                {
                    using (SqliteConnection con = conn.SQLConnection)
                    {
                        con.Open();
                        SqliteCommand cmd = new SqliteCommand($"UPDATE client SET name = '{NameEntry.Text}'", con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    pages.GlobalVariables.loginUser.name = NameEntry.Text;
                    pages.GlobalVariables.loginUser.location = LocationEntry.Text;
                    pages.GlobalVariables.loginUser.birthday = BirthdayPicker.Date.ToString("yyyyMMdd");
                    pages.GlobalVariables.loginUser.imagePath = "iets";

                    HobbyRegisterPage hobbyRegisterPage = new HobbyRegisterPage();
                    Navigation.PushModalAsync(hobbyRegisterPage);
                }
            }
            else
            {
                DisplayAlert("Check internet connection", "Frindr could not connect to the internet, please check your internet connection and try again", "Continue");
            }
        }

        private bool CheckLocation(string location)
        {
            string regex = @"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$";

            if (location == null)
            {
                DisplayAlert("", "Voer een Postcode in", "ok");
                return false;
            }

            bool isLocationValid = Regex.IsMatch(location, regex);

            if (isLocationValid)
            {
                return true;
            }
            else
            {
                DisplayAlert("", "Postcode is ongeldig", "ok");
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