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
        public string Username { get; set; }
        MainPage mp = new MainPage();
        Connection conn = new Connection();

        public Profile()
        {
            InitializeComponent();
            lblProfile.Text = "Welcome " + mp.Username;
        }

        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            using (SqliteConnection con = conn.SQLConnection)
            {
                string cmdStr = $"DELETE FROM client WHERE name = {mp.Username}, pw = {/*insert password. email and id*/""}";
                SqliteCommand cmd = new SqliteCommand(cmdStr, con);
            }
        }
    }
}