using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FirstRegisterPage : ContentPage
	{
        Connection conn = new Connection();
        Hash hash = new Hash();
        string hashedString;
		public FirstRegisterPage ()
		{
			InitializeComponent ();
		}

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            if (CheckEmail(EmailEntry.Text) && CheckPassword(PasswordEntry.Text))
            {
                try
                {
                    using (SqliteConnection con = conn.SQLConnection)
                    {
                        con.Open();
                        SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(256), email VARCHAR(255), auto TINYINT(1))", con);
                        cmd.ExecuteNonQuery();

                        SqliteCommand cmd1 = new SqliteCommand("SELECT * FROM client WHERE id = 1", con);
                        cmd1.ExecuteNonQuery();

                        using (SqliteDataReader rdr = cmd1.ExecuteReader())
                        {
                            if (!rdr.HasRows)
                            {
                                SqliteCommand cmd2 = new SqliteCommand($"INSERT INTO client (pw, email) VALUES ('{hashedString}', '{EmailEntry.Text}')", con);
                                cmd2.ExecuteNonQuery();
                            }

                            while (rdr.Read())
                            {
                                SqliteCommand cmd3 = new SqliteCommand($"UPDATE client SET pw = '{hashedString}', email = '{EmailEntry.Text}' WHERE id = 1", con);
                                cmd3.ExecuteNonQuery();
                            }
                            rdr.Close();
                        }
                        con.Close();
                    }
                }
                catch (SqliteException)
                {
                    DisplayAlert("An error occurred", "Git gud", "Ok");
                }

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