using Frindr.pages;
using Microsoft.Data.Sqlite;
using System;
using Xamarin.Forms;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        public bool Registered { get; set; }
        public static string Users { get; set; }
        public static string Hobbies { get; set; }
        public static string UserHobby { get; set; }
        Connection conn = new Connection();

        public MainPage()
        {
            InitializeComponent();

            Users = GlobalVariables.GetUsers();
            Hobbies = GlobalVariables.GetHobbies();
            UserHobby = GlobalVariables.GetUserHobbies();

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
                                    Navigation.PushModalAsync(new ProfilePage());
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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Navigation.PushModalAsync(new FirstRegisterPage());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            RegisterPage register = new RegisterPage();
            Navigation.PushModalAsync(register);
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            LoginPage login = new LoginPage();
            Navigation.PushModalAsync(login);
        }
    }
}
