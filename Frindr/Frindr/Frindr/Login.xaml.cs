using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Microsoft.Data.Sqlite;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentPage
	{
        Connection conn = new Connection();

        public Login ()
		{
			InitializeComponent ();
		}

        private void btnConfirm_Clicked(object sender, EventArgs e)
        {
            try
            {
                //conn.SQLConnection
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    string cmdStr = $"SELECT * FROM client WHERE name = '{txtUsername.Text}' AND pw = '{txtPassword.Text}'";

                    SqliteCommand cmd = new SqliteCommand(cmdStr, con);
                    cmd.ExecuteNonQuery();
                    
                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string readUser = rdr.GetString(1);
                            string readPass = rdr.GetString(2);

                            if (txtUsername.Text == readUser && txtPassword.Text == readPass)
                            {
                                DisplayAlert("WE IN", "WOUTER IS A GENIUS", "ok");
                            }
                            else
                            {
                                DisplayAlert("Login fout", "Gebruiker en/of wachtwoord is fout", "Ok");
                            }
                        }
                        rdr.Close();
                    }
                    con.Close();
                }
                
            }
            catch (Exception ea)
            {
                DisplayAlert("An error occurred", "Something went wrong with your login attempt. Try again or come back later","Ok");
                Console.WriteLine(ea.ToString());
            }
        }
    }
}