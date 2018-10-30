using Frindr.pages;
using Microsoft.Data.Sqlite;
using System;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        Connection conn = new Connection();

        public static string Users { get; set; }
        public static string Hobbies { get; set; }
        public static string UserHobby { get; set; }

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
                            Navigation.PushModalAsync(new FirstRegisterPage());
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
                                    RestfulClass restful = new RestfulClass();

                                    string rest = restful.GetData($"/records/user?filter=pwd,eq,{rdr2.GetString(2)}&filter=email,eq,{rdr2.GetString(3)}");
                                    UserRecords json = JsonConvert.DeserializeObject<UserRecords>(rest);

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

                                    Navigation.PushModalAsync(new MenuPage());
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
