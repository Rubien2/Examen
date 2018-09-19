using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Data.Sqlite;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Register : ContentPage
	{
        Connection conn = new Connection();

        public Register ()
		{
			InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(255))", con);
                    cmd.ExecuteNonQuery();

                    SqliteCommand cmd2 = new SqliteCommand($"INSERT INTO client (name, pw) VALUES ('{txtUsername.Text}', '{txtPassword.Text}')", con);
                    cmd2.ExecuteNonQuery();

                    con.Close();
                }
                
            }
            catch (Exception edde)
            {
                DisplayAlert("An error occurred", "An error occurred: " + edde.ToString(),"Ok");
            }
        }
    }
}