using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentPage
    {
        public string Username { get; set; }
        MainPage mp = new MainPage();
        public Profile()
        {
            InitializeComponent();
            lblProfile.Text = "Welcome " + mp.Username;
        }
    }
}