using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;

namespace Frindr.pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();
            string cmdStr = "DROP TABLE IF EXISTS client";
            Connection conn = new Connection();

            using (SqliteConnection con = conn.SQLConnection)
            {
                con.Open();
                SqliteCommand cmd = new SqliteCommand(cmdStr, con);
                cmd.ExecuteNonQuery();
                con.Close();
                Navigation.PushModalAsync(new MainPage());
            }
        }
    }
}