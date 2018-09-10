using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Register : ContentPage
	{
        string userRecords = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "user.txt");
		public Register ()
		{
			InitializeComponent();
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            using (StreamWriter streamWriter = new StreamWriter(File.Create(userRecords)))
            {
                streamWriter.WriteLine("Harry");
            }
        }
    }
}