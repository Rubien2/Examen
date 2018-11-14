using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;
using Frindr.pages;
using Newtonsoft.Json;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirstRegisterPage : ContentPage
    {
        Connection conn = new Connection();
        RestfulClass rest = new RestfulClass();
        Hash hash = new Hash();
        string hashedString;

        public FirstRegisterPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            if (conn.IsOnline())
            {
                string checkEmail = rest.GetData($"/records/user?filter=email,eq,{EmailEntry.Text}");
                UserRecords userRecords = JsonConvert.DeserializeObject<UserRecords>(checkEmail);

                EmailEntry.Text.Replace(" ", string.Empty);

                if (ValidateEmail(EmailEntry.Text) && CheckPassword(PasswordEntry.Text) && PasswordEntry.Text == ConfirmPasswordEntry.Text && userRecords.records.Count == 0)
                {
                    GlobalVariables.loginUser.email = EmailEntry.Text;
                    GlobalVariables.loginUser.pwd = hashedString;
                    GlobalVariables.loginUser.description = "";

                    PersonalRegisterPage personalRegisterPage = new PersonalRegisterPage();
                    Navigation.PushModalAsync(personalRegisterPage);
                }
                else if (!ValidateEmail(EmailEntry.Text))
                {
                    DisplayAlert("", "Email adres is niet geldig", "ok");
                }
                else if (userRecords.records.Count > 0)
                {
                    DisplayAlert("","Email is al in gebruik","ok");
                }
            }
            else
            {
                DisplayAlert("Check internet verbinding", "Frindr kon niet met het internet verbinden", "ok");
            }
        }

        private void GoToLoginButton_Clicked(object sender, EventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            Navigation.PushModalAsync(loginPage);
        }

        public bool ValidateEmail(string email)
        {
            Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (string.IsNullOrWhiteSpace(email)){
                return false;
            }

            return EmailRegex.IsMatch(email);
        }

        private bool CheckPassword(string password)
        {
            if (password != null && password.Length >= 8)
            {
                hashedString = hash.HashString(password);

                return true;
            }
            else
            {
                DisplayAlert("", "Wachtwoord moet uit minimaal 8 karakters bestaan", "ok");
                return false;
            }
        }
    }
}