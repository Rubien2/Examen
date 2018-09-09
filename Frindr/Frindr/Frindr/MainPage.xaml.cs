using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using System.IO;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            DatabaseConnection();
        }
        
        public void DatabaseConnection()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FrindrDB.db3");
            if (!File.Exists(dbPath))
            {
                var db = new SQLiteConnection(dbPath);
            }
        }

    }
}
