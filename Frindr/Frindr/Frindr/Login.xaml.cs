using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

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
                
                SQLiteConnection connect = conn.SQLConnection;

                
                    //I HATE THIS WEIRD QUERY
                    var db = connect.CreateCommand("SELECT * FROM client WHERE name = '" + txtUsername.Text + "' AND pw = '" + txtPassword.Text + "'");
                    int result = db.ExecuteNonQuery();

                    if(result == 1)
                    {
                        DisplayAlert("WE IN","IM A GENIUS","ok");
                    }
                    //con.Close();
                
                
            }
            catch (Exception ea)
            {
                DisplayAlert("An error occurred", "Something went wrong with your login attempt. Try again or come back later","Ok");
                Console.WriteLine(ea.ToString());
            }
        }
    }
}