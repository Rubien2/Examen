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
        Connection conn = new Connection();
        RestfulClass restful = new RestfulClass();
        Hash hash = new Hash();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            if (conn.IsOnline())
            {
                string hashedString = hash.HashString(PasswordEntry.Text);

                try
                {
                    using (SqliteConnection con = conn.SQLConnection)
                    {
                        con.Open();

                        SqliteCommand cmd5 = new SqliteCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(256), email VARCHAR(255), auto TINYINT(1))", con);
                        cmd5.ExecuteNonQuery();

                        string getUser = restful.GetData($"/records/user?filter=name,eq,{NameEntry.Text}&filter=pwd,eq,{hashedString}");

                        UserRecords json = JsonConvert.DeserializeObject<UserRecords>(getUser);

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

                                        GlobalVariables.loginUser.name = json.records[0].name;
                                        GlobalVariables.loginUser.pwd = json.records[0].pwd;
                                        GlobalVariables.loginUser.email = json.records[0].email;
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

                                        GlobalVariables.loginUser.name = json.records[0].name;
                                        GlobalVariables.loginUser.pwd = json.records[0].pwd;
                                        GlobalVariables.loginUser.email = json.records[0].email;
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
                                        DisplayAlert("Login error", "Couldn't log you in. Please try again or come back later", "ok");
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
                    DisplayAlert("Error", ea.ToString(), "OK");
                }
            }
            else
            {
                DisplayAlert("Check internet connection", "Frindr could not connect to the internet, please check your internet connection and try again", "Continue");
            }
        }

        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FirstRegisterPage());
        }
    }
}