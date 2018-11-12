using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Frindr.pages;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            Connection conn = new Connection();
            RestfulClass restful = new RestfulClass();
            Hash hash = new Hash();

            if (conn.IsOnline())
            {
                string hashedString = "";
                bool emptyPWD = true;

                if (PasswordEntry.Text != null)
                {
                    hashedString = hash.HashString(PasswordEntry.Text);
                    emptyPWD = false;
                }
                else
                {
                    DisplayAlert("", "Vul uw wachtwoord in", "ok");
                    emptyPWD = true;
                }
                
                try
                {
                    if (!emptyPWD)
                    {
                        using (SqliteConnection con = conn.SQLConnection)
                        {
                            con.Open();

                            SqliteCommand cmd5 = new SqliteCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(256), email VARCHAR(255), auto TINYINT(1))", con);
                            cmd5.ExecuteNonQuery();

                            string getUser = restful.GetData($"/records/user?filter=name,eq,{NameEntry.Text}&filter=pwd,eq,{hashedString}");

                            UserRecords json = JsonConvert.DeserializeObject<UserRecords>(getUser);
                            if (json.records.Count == 0)
                            {
                                DisplayAlert("", "Gebruikersnaam en/of wachtwoord is verkeerd, vul dit a.u.b. correct in", "ok");
                            }
                            if (NameEntry.Text == json.records[0].name && hashedString == json.records[0].pwd)
                            {
                                string cmdStr = $"SELECT * FROM client WHERE id = 1";

                                SqliteCommand cmd = new SqliteCommand(cmdStr, con);
                                cmd.ExecuteNonQuery();

                                using (SqliteDataReader rdr = cmd.ExecuteReader())
                                {
                                    if (!rdr.HasRows)
                                    {
                                        try
                                        {
                                            string cmdStr2 = $"INSERT INTO client (name, pw, email, auto) VALUES ('{json.records[0].name}','{json.records[0].pwd}','{json.records[0].email}', {Convert.ToInt32(RememberSwitch.IsToggled)})";
                                            SqliteCommand cmd2 = new SqliteCommand(cmdStr2, con);
                                            cmd2.ExecuteNonQuery();

                                            GlobalVariables.loginUser.id = json.records[0].id;
                                            GlobalVariables.loginUser.name = json.records[0].name;
                                            GlobalVariables.loginUser.pwd = json.records[0].pwd;
                                            GlobalVariables.loginUser.email = json.records[0].email;
                                            GlobalVariables.loginUser.description = json.records[0].description;
                                            GlobalVariables.loginUser.birthday = json.records[0].birthday;
                                            GlobalVariables.loginUser.imagePath = json.records[0].imagePath;
                                            GlobalVariables.loginUser.location = json.records[0].location;
                                            GlobalVariables.loginUser.locationVisible = json.records[0].locationVisible;
                                            GlobalVariables.loginUser.userVisible = json.records[0].userVisible;

                                            MenuPage profile = new MenuPage();
                                            Navigation.PushModalAsync(profile);
                                        }
                                        catch (SqliteException)
                                        {
                                            DisplayAlert("Login error", "Couldn't log you in. Please try again or come back later", "ok");
                                        }
                                    }

                                    while (rdr.Read())
                                    {
                                        try
                                        {
                                            string cmdStr1 = $"UPDATE client SET name = '{json.records[0].name}', pw = '{json.records[0].pwd}', email = '{json.records[0].email}', auto = {Convert.ToInt32(RememberSwitch.IsToggled)} WHERE id = 1";
                                            SqliteCommand cmd1 = new SqliteCommand(cmdStr1, con);
                                            cmd1.ExecuteNonQuery();

                                            GlobalVariables.loginUser.id = json.records[0].id;
                                            GlobalVariables.loginUser.name = json.records[0].name;
                                            GlobalVariables.loginUser.pwd = json.records[0].pwd;
                                            GlobalVariables.loginUser.email = json.records[0].email;
                                            GlobalVariables.loginUser.description = json.records[0].description;
                                            GlobalVariables.loginUser.birthday = json.records[0].birthday;
                                            GlobalVariables.loginUser.imagePath = json.records[0].imagePath;
                                            GlobalVariables.loginUser.location = json.records[0].location;
                                            GlobalVariables.loginUser.locationVisible = json.records[0].locationVisible;
                                            GlobalVariables.loginUser.userVisible = json.records[0].userVisible;

                                            MenuPage menuPage = new MenuPage();
                                            Navigation.PushModalAsync(menuPage);
                                        }
                                        catch (SqliteException)
                                        {
                                            
                                        }
                                    }
                                    rdr.Close();
                                }
                                con.Close();
                            }
                        }
                    }
                    
                }

                catch (Exception)
                {
                    
                }
            }
            else
            {
                DisplayAlert("Check internet verbinding", "Frindr kon geen internetverbinding krijgen", "ok");
            }
        }

        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FirstRegisterPage());
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void PWDResetButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PasswordResetPage());
        }
    }
}