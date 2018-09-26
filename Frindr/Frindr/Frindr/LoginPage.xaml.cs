using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        Connection conn = new Connection();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnConfirm_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    string cmdStr = $"SELECT * FROM client WHERE name = '{NameEntry.Text}' AND pw = '{PasswordEntry.Text}' AND id = 1";

                    SqliteCommand cmd = new SqliteCommand(cmdStr, con);
                    cmd.ExecuteNonQuery();

                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string readUser = rdr.GetString(1);
                            string readPass = rdr.GetString(2);
                            /*
                            try
                            {
                                string cmdStr2 = $"DELETE FROM client WHERE name = '{NameEntry.Text}' AND pw = '{PasswordEntry.Text}' AND id = 1";
                                SqliteCommand cmd1 = new SqliteCommand(cmdStr2, con);
                                SqliteDataReader rdr2 = cmd1.ExecuteReader();

                                while (rdr2.Read())
                                {
                                    //check if record exists online and then insert into local
                                    rdr2.Close();
                                }
                            }

                            catch (SqliteException ea)
                            {
                                if (NameEntry.Text == readUser && PasswordEntry.Text == readPass)
                                {
                                    Profile profile = new Profile();
                                    profile.Username = readUser;
                                    Navigation.PushModalAsync(profile);
                                }
                                else
                                {
                                    DisplayAlert("Login fout", "Gebruiker en/of wachtwoord is fout", "Ok");
                                }
                            }*/
                            if (NameEntry.Text == readUser && PasswordEntry.Text == readPass)
                            {
                                Profile profile = new Profile();
                                profile.Username = NameEntry.Text;
                                Navigation.PushModalAsync(profile);
                            }
                            else
                            {
                                DisplayAlert("Login fout", "Gebruiker en/of wachtwoord is fout", "Ok");
                            }
                        }
                        rdr.Close();
                    }
                    con.Close();
                }
                if (RememberSwitch.IsToggled)
                {
                    
                }
            }

            catch (Exception ea)
            {
                DisplayAlert("An error occurred", "Something went wrong with your login attempt. Try again or come back later", "Ok");
                Console.WriteLine(ea.ToString());
            }
        }
    }
}