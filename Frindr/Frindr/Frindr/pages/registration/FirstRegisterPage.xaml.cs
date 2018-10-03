using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FirstRegisterPage : ContentPage
	{
		public FirstRegisterPage ()
		{
			InitializeComponent ();
		}

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            if (CheckEmail(EmailEntry.Text) && CheckPassword(PasswordEntry.Text))
            {
                PersonalRegisterPage personalRegisterPage = new PersonalRegisterPage();
                Navigation.PushModalAsync(personalRegisterPage);
            }
        }

        private void GoToLoginButton_Clicked(object sender, EventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            Navigation.PushModalAsync(loginPage);
        }

        private bool CheckEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                DisplayAlert("","Email adres is niet geldig","ok");
                return false;
            }
        }

        private bool CheckPassword(string password)
        {
            if (password != null && password.Length >= 8)
            {
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