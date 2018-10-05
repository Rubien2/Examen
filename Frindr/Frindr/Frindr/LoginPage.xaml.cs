using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        Connection conn = new Connection();
        RestfulClass restful = new RestfulClass();

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

                    string getUser = restful.GetData($"/records/user?filter=name,eq,{NameEntry.Text}&filter=pwd,eq,{PasswordEntry.Text}");

                    Records json = JsonConvert.DeserializeObject<Records>(getUser);

                    //check for hash later

                    if (NameEntry.Text == json.records[0].name && PasswordEntry.Text == json.records[0].pwd)
                    {
                        string cmdStr = $"SELECT * FROM client WHERE id = 1";

                        SqliteCommand cmd = new SqliteCommand(cmdStr, con);
                        cmd.ExecuteNonQuery();

                        using (SqliteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                try
                                {
                                    string cmdStr1 = $"UPDATE client SET name = '{json.records[0].name}', pw = '{json.records[0].pwd}', email = '{json.records[0].email}', auto = {Convert.ToInt32(RememberSwitch.IsToggled)} WHERE id = 1";
                                    SqliteCommand cmd1 = new SqliteCommand(cmdStr1, con);
                                    cmd1.ExecuteNonQuery();

                                    Navigation.PushModalAsync(new Profile());
                                }
                                catch (SqliteException ea)
                                {
                                    DisplayAlert("", ea.ToString(), "ok");
                                }
                            }
                            rdr.Close();
                        }
                        con.Close();
                    }
                }
            }

            catch (Exception ea)
            {
                DisplayAlert("Error",ea.ToString(),"OK");
            }            
        }

        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new RegisterPage());
        }
    }
}