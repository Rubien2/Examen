using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Frindr.pages;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordResetPage : ContentPage
    {
        private static Random random = new Random();
        RestfulClass rest = new RestfulClass();
        Connection conn = new Connection();
        Hash hash = new Hash();

        public PasswordResetPage()
        {
            InitializeComponent();
        }

        public static string RandomString()
        {
            const string generatePassword = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(generatePassword, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool CheckEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                DisplayAlert("", "Email adres is niet geldig", "ok");
                return false;
            }
        }

        private void ButtonReset_Clicked(object sender, EventArgs e)
        {
            bool mailCheck = CheckEmail(txtPasswordReset.Text);
            if (conn.IsOnline())
            {
                if (mailCheck)
                {
                    try
                    {
                        string toBeHashedPWD = RandomString();

                        MailMessage mail = new MailMessage("info@frindr.nl", txtPasswordReset.Text, "Uw Frindr wachtwoord is gereset", $"Uw wachtwoord is veranderd naar {toBeHashedPWD}. U kunt uw wachtwoord de volgende keer weer veranderen in uw profiel instellingen");
                        SmtpClient smtpClient = new SmtpClient("smtp.strato.com", 587);
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new System.Net.NetworkCredential("info@frindr.nl", "FrindrWachtwoord");
                        smtpClient.Send(mail);

                        string getLostUser = rest.GetData($"/records/user?filter=email,eq,{txtPasswordReset.Text}");
                        UserRecords user = JsonConvert.DeserializeObject<UserRecords>(getLostUser);


                        string hashedPWD = hash.HashString(toBeHashedPWD);

                        user.records[0].pwd = hashedPWD;
                        string json = JsonConvert.SerializeObject(user.records[0]);


                        rest.SetData($"/records/user/{user.records[0].id}", json);

                        Navigation.PushModalAsync(new LoginPage());
                    }
                    catch (SmtpException ea)
                    {
                        DisplayAlert(ea.ToString(), ea.ToString(), "ok");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        DisplayAlert("","Email bestaat niet, kies een geldig email adres","ok");
                    }

                    
                }
            }
            else
            {
                DisplayAlert("","Geen verbinding met internet, check de verbinding en probeer het opnieuw","ok");
            }
            
        }
    }
}