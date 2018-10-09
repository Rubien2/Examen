using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        Connection conn = new Connection();
        RestfulClass restful = new RestfulClass();
        Hash hash = new Hash();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            string hashedString = hash.HashString(PasswordEntry.Text);

            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(256), email VARCHAR(255), auto TINYINT(1))", con);
                    cmd.ExecuteNonQuery();

                    SqliteCommand cmd1 = new SqliteCommand("SELECT * FROM client WHERE id = 1",con);
                    cmd1.ExecuteNonQuery();

                    using (SqliteDataReader rdr = cmd1.ExecuteReader())
                    {
                        if (!rdr.HasRows)
                        {
                            SqliteCommand cmd2 = new SqliteCommand($"INSERT INTO client (name, pw, email, auto) VALUES ('{NameEntry.Text}', '{hashedString}', '{EmailEntry.Text}', 1)", con);
                            cmd2.ExecuteNonQuery();
                        }

                        while (rdr.Read())
                        {
                            SqliteCommand cmd3 = new SqliteCommand($"UPDATE client SET name = '{NameEntry.Text}', pw = '{hashedString}', email = '{EmailEntry.Text}', auto = 1 WHERE id = 1", con);
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

            int find = Convert.ToInt32(PrivacyFindSwitch.IsToggled);
            int location = Convert.ToInt32(PrivacyLocationSwitch.IsToggled);

            //order for json: id, name, email, pwd, location, birthday, imagePath, userVisible, locationVisible
            JsonValues json = new JsonValues
            {
                id = null,
                name = NameEntry.Text,
                email = EmailEntry.Text,
                pwd = hashedString,
                location = LocationEntry.Text,
                birthday = $"{YearEntry.Text}-{MonthEntry.Text}-{DayEntry.Text}",
                imagePath = "Something",
                userVisible = find,
                locationVisible = location
            };

            string output = JsonConvert.SerializeObject(json);
            
            restful.CreateData($"/records/user", output);

            Navigation.PushModalAsync(new Profile());
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new LoginPage());
        }
    }
}