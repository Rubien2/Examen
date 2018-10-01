using System;
using Xamarin.Forms;
using Microsoft.Data.Sqlite;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        Connection conn = new Connection();
        public string Username { get; set; }

        public MainPage()
        {
            InitializeComponent();

            //auto login check, also not safe at all
            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    string cmdStr = "SELECT name FROM sqlite_master WHERE type='table' AND name = 'client'";
                    SqliteCommand cmd = new SqliteCommand(cmdStr, con);
                    cmd.ExecuteNonQuery();

                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string cmdStr2 = "SELECT * FROM client WHERE id = 1";
                            SqliteCommand cmd2 = new SqliteCommand(cmdStr2, con);
                            cmd2.ExecuteNonQuery();

                            using (SqliteDataReader rdr2 = cmd2.ExecuteReader())
                            {
                                while (rdr2.Read())
                                {
                                    Username = rdr2.GetString(1);
                                    Navigation.PushModalAsync(new Profile());
                                    rdr2.Close();
                                }
                            }
                            rdr.Close();
                        }
                    }
                    con.Close();
                }
            }

            catch (Exception ea)
            {
                DisplayAlert("Error", ea.ToString(), "Ok");
            }

            Navigation.PushModalAsync(new RegisterPage());
        }
    }
}
