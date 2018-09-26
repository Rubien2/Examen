using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        Connection conn = new Connection();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(255), email VARCHAR(255))", con);
                    cmd.ExecuteNonQuery();

                    try
                    {
                        SqliteCommand cmd1 = new SqliteCommand($"UPDATE client SET name = {NameEntry.Text}, pw = {PasswordEntry.Text}, email = {EmailEntry.Text} WHERE id = 1", con);
                        cmd1.ExecuteNonQuery();
                        SqliteDataReader rdr = cmd1.ExecuteReader();

                        while (rdr.Read())
                        {
                            Profile profile = new Profile();
                            profile.Username = rdr.GetString(1);
                            Navigation.PushModalAsync(profile);
                            rdr.Close();
                        }
                    }

                    catch (Exception ea)
                    {
                        SqliteCommand cmd2 = new SqliteCommand($"INSERT INTO client (name, pw, email) VALUES ('{NameEntry.Text}', '{PasswordEntry.Text}', '{EmailEntry.Text}')", con);
                        cmd2.ExecuteNonQuery();
                    }
                    
                    con.Close();
                }
            }

            catch (Exception ea)
            {
                DisplayAlert("An error occurred", "0", "Ok");
            }
        }
    }
}