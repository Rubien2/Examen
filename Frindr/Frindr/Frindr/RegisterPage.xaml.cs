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
                    }

                    catch (SqliteException ea)
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

            JsonValues json = new JsonValues();
            json.ID = null;
            json.User = NameEntry.Text;
            json.Pwd = PasswordEntry.Text;
            json.Email = EmailEntry.Text;
            json.Location = LocationEntry.Text;
            json.Birthday = $"{YearEntry.Text}-{MonthEntry.Text}-{DayEntry.Text}";
            json.ImagePath = "Something";
            json.UserVisibility = PrivacyFindSwitch.IsToggled;
            json.LocationVisibility = PrivacyLocationSwitch.IsToggled;

            string output = JsonConvert.SerializeObject(json);
            
            restful.CreateData($"/records/user", output);

            Navigation.PushModalAsync(new Profile());
        }
    }
}