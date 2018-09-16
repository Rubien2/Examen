using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Register : ContentPage
	{
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "user.db3");
        Connection conn = new Connection();

        public Register ()
		{
			InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                SQLiteConnection db = conn.SQLConnection;
                var content = db.CreateCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(255))");
                content.ExecuteNonQuery();
                
                var cmd = db.CreateCommand("INSERT INTO client (name, pw) VALUES ('" + txtUsername.Text + "', '" + txtPassword.Text + "')");
                cmd.ExecuteNonQuery();
            }
            catch (Exception edde)
            {
                DisplayAlert("An error occurred", "An error occurred: " + edde.ToString(),"Ok");
            }
        }
    }
}