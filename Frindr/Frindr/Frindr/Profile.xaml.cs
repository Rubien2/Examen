using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentPage
    {
        Connection conn = new Connection();
        Hash hash = new Hash();

        public static bool LoggedIn { get; set; }
        string username;
        string email;

        public Profile()
        {
            InitializeComponent();
            CheckLogin();
            lblProfile.Text = "Welcome " + username;
            lblEmail.Text = email;
        }

        private void CheckLogin()
        {
            if (LoggedIn)
            {
                //all variables are from login
                username = LoginPage.Username;
                email = LoginPage.Email;
            }
            else
            {
                //all variables are from register
                username = RegisterPage.Username;
                email = RegisterPage.Email;
            }
        }

        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();                    
                    string cmdStr = $"DELETE FROM client WHERE name = '{username}' AND email = '{email}'";
                    SqliteCommand cmd = new SqliteCommand(cmdStr, con);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    Navigation.PushModalAsync(new LoginPage());
                }
            }
            catch (SqliteException)
            {
                DisplayAlert("soup","sup","kek");
            }
        }
    }
}