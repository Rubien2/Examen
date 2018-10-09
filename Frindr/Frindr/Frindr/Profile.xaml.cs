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

        public Profile()
        {
            InitializeComponent();
            lblProfile.Text = "Welcome " + LoginPage.Username;
            lblEmail.Text = LoginPage.Email;
        }

        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    string cmdStr = $"DELETE FROM client WHERE name = '{LoginPage.Username}' AND email = '{LoginPage.Email}'";
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