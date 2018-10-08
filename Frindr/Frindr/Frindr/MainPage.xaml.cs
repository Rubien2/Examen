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
                            string cmdStr2 = "SELECT * FROM client WHERE id = 1";
                            SqliteCommand cmd2 = new SqliteCommand(cmdStr2, con);
                            cmd2.ExecuteNonQuery();

                            using (SqliteDataReader rdr2 = cmd2.ExecuteReader())
                            {
                                while (rdr2.Read())
                                {
                                    byte check = 1;
                                    //tinyint keeps returning 0 even tho it's 1
                                    if (rdr2.GetByte(4) == check)
                                    {
                                        Navigation.PushModalAsync(new Profile());
                                    }
                                    else
                                    {
                                        Navigation.PushModalAsync(new LoginPage());
                                    }
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
