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
	public partial class HobbyRegisterPage : ContentPage
	{
		public HobbyRegisterPage ()
		{
			InitializeComponent ();
		}

        private void CreateAccountButton_Clicked(object sender, EventArgs e)
        {
            MenuPage menuPage = new MenuPage();
            Navigation.PushModalAsync(menuPage);
        }
    }
}