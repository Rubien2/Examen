using System;
using Xamarin.Forms;
using Microsoft.Data.Sqlite;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        Connection conn = new Connection();

        public MainPage()
        {
            InitializeComponent();

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
                        if (!rdr.HasRows)
                        {
                            Navigation.PushModalAsync(new RegisterPage());
                        }

                        while (rdr.Read())
                        {
                            string cmdStr2 = "SELECT * FROM client WHERE id = 1 AND auto = 1";
                            SqliteCommand cmd2 = new SqliteCommand(cmdStr2, con);
                            cmd2.ExecuteNonQuery();

                            using (SqliteDataReader rdr2 = cmd2.ExecuteReader())
                            {
                                if (!rdr2.HasRows)
                                {
                                    Navigation.PushModalAsync(new LoginPage());
                                }

                                while (rdr2.Read())
                                {
                                    Navigation.PushModalAsync(new Profile());
                                }

                                rdr2.Close();
                            }
                        }
                        rdr.Close();
                    }
                    con.Close();
                }
            }

            catch (ArgumentOutOfRangeException ea)
            {
                Console.WriteLine(ea.ToString());
            }
            catch (SqliteException ea)
            {
                Console.WriteLine(ea.ToString());
            }
        }
    }
}
