using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Frindr.pages;
using Plugin.Messaging;
using System.Net.Mail;
using Microsoft.Data.Sqlite;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HobbyRegisterPage : ContentPage
	{

        private ObservableCollection<GlobalVariables.Category> grouped { get; set; }

        private ObservableCollection<GlobalVariables.Hobbies> hobbiesCollection = GlobalVariables.hobbiesCollection;
        private ObservableCollection<GlobalVariables.Hobbies> selectedHobbies = GlobalVariables.selectedHobbies;

        Connection conn = new Connection();

        public HobbyRegisterPage ()
		{
			InitializeComponent ();

            FillListView();
		}

        private void CreateAccountButton_Clicked(object sender, EventArgs e)    
        {
            try
            {
                using (SqliteConnection con = conn.SQLConnection)
                {
                    con.Open();
                    SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS client (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), pw VARCHAR(256), email VARCHAR(255), auto TINYINT(1))", con);
                    cmd.ExecuteNonQuery();

                    SqliteCommand cmd1 = new SqliteCommand("SELECT * FROM client WHERE id = 1", con);
                    cmd1.ExecuteNonQuery();

                    using (SqliteDataReader rdr = cmd1.ExecuteReader())
                    {
                        if (!rdr.HasRows)
                        {
                            SqliteCommand cmd2 = new SqliteCommand($"INSERT INTO client (name, pw, email, auto) VALUES ('{GlobalVariables.loginUser.name}', '{GlobalVariables.loginUser.pwd}', '{GlobalVariables.loginUser.email}', 1)", con);
                            cmd2.ExecuteNonQuery();
                        }

                        while (rdr.Read())
                        {
                            SqliteCommand cmd3 = new SqliteCommand($"UPDATE client SET name = '{GlobalVariables.loginUser.name}', pw = '{GlobalVariables.loginUser.pwd}', email = '{GlobalVariables.loginUser.email}', auto = 1", con);
                            cmd3.ExecuteNonQuery();
                        }
                        rdr.Close();
                    }
                    con.Close();
                }

            }
            catch (SqliteException)
            {
            }

            var selected = hobbiesCollection
             .Where(p => p.selected)
             .ToList();
            GlobalVariables.selectedHobbies = new ObservableCollection<GlobalVariables.Hobbies>(selected);

            RestfulClass rest = new RestfulClass();
            string json = JsonConvert.SerializeObject(GlobalVariables.loginUser);
            rest.CreateData("/records/user/", json);

            Thread.Sleep(500);
            
            //get id from user that's registering
            string json2 = rest.GetData($"/records/user?filter=name,eq,{GlobalVariables.loginUser.name}&filter=email,eq,{GlobalVariables.loginUser.email}");
            UserRecords deJson = JsonConvert.DeserializeObject<UserRecords>(json2);

            foreach (var hobby in selected)
            {
                GlobalVariables.hobbyUser.userId = deJson.records[0].id ?? default(int);
                GlobalVariables.loginUser.id = deJson.records[0].id;
                GlobalVariables.hobbyUser.hobbyId = hobby.id;

                json = JsonConvert.SerializeObject(GlobalVariables.hobbyUser);
                rest.CreateData("/records/userHobby/", json);
            }
            SendEmail(GlobalVariables.loginUser.email);
            MenuPage menuPage = new MenuPage();
            Navigation.PushModalAsync(menuPage);
        }

        private void SendEmail(string receiverEmail)
        {
            try
            {
                MailMessage mail = new MailMessage("info@frindr.nl", receiverEmail, "Bedankt voor het registreren bij Frindr", $"Welkom bij Frindr {GlobalVariables.loginUser.name}, een plaats waar u mensen kan vinden met dezelfde hobby's als u");
                SmtpClient smtpClient = new SmtpClient("smtp.strato.com", 587);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("info@frindr.nl", "FrindrWachtwoord");
                smtpClient.Send(mail);
            }
            catch (SmtpException)
            {

            }
            
        }

        void FillListView()
        {
            var records = MainPage.Hobbies;

            if (records != null)
            {
                MyListView.ItemsSource = null;

                var root = JsonConvert.DeserializeObject<GlobalVariables.HobbyRecords>(records);
                //Convert list to observable collection. This is easier for the grouping in the listview
                hobbiesCollection = new ObservableCollection<GlobalVariables.Hobbies>(root.records);

                grouped = new ObservableCollection<GlobalVariables.Category>();

                //Define Hobby categories
                var VideoGamesGroup =       new GlobalVariables.Category() { id = 1, name = "Video games" };
                var SportGroup =            new GlobalVariables.Category() { id = 2, name = "Sporten" };
                var HobbyAndFreeTimeGroup = new GlobalVariables.Category() { id = 3, name = "Hobby en vrije tijd" };
                var DoItYourselfGroup =     new GlobalVariables.Category() { id = 4, name = "Doe-het-zelf" };
                var TechnologieGroup =      new GlobalVariables.Category() { id = 5, name = "Technologie" };
                var OtherGroup =            new GlobalVariables.Category() { id = 0, name = "Overig" };

                //add hobbies to groups
                foreach (var hobby in hobbiesCollection)
                {
                    if (selectedHobbies != null && selectedHobbies.Any(p => p.id == hobby.id))
                    {
                        hobby.selected = true;
                    }

                    int caseSwitch = hobby.hobbyCategoryId;
                    switch (caseSwitch)
                    {
                        case 1:
                            VideoGamesGroup.Add(hobby);
                            break;
                        case 2:
                            SportGroup.Add(hobby);
                            break;
                        case 3:
                            HobbyAndFreeTimeGroup.Add(hobby);
                            break;
                        case 4:
                            DoItYourselfGroup.Add(hobby);
                            break;
                        case 5:
                            TechnologieGroup.Add(hobby);
                            break;
                        default:
                            OtherGroup.Add(hobby);
                            break;
                    }

                }

                //set grouped as item source
                grouped.Add(VideoGamesGroup); grouped.Add(SportGroup); grouped.Add(HobbyAndFreeTimeGroup); grouped.Add(DoItYourselfGroup); grouped.Add(TechnologieGroup);
                MyListView.ItemsSource = grouped;
            }

        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {

            return base.OnBackButtonPressed();
        }

    }
}