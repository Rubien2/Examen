using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Frindr
{

    //class for intervacing with a restfull API. Go to: https://github.com/mevdschee/php-crud-api for documentation.
    public class RestfulClass
    {

        //location of the api.php file
        readonly string apiLocation = "http://www.frindr.nl/api.php";

        //FTP credentials used to store and get images
        string ftpUser = "frindradmin@frindr.nl";
        string ftpPassword = "frindrwachtwoord";
        string ftpPath = "ftp://frindradmin%2540frindr.nl@ftp.strato.com/images/";

        HttpClient client;


        //these functions are used to modify a database using a Restful API. The functions automatically add the API location
        //so you only need to add the command. for example: /records/categories?filter=name,eq,Internet

        public string GetData(string url)   //gets row from database specified by URL.
        {
            string newUrl = apiLocation + url;

            return ModifyData(newUrl, "GET");
        }

        //--------****|IMPORTANT|*****------\\
        //All data except ID can't be NULL
        public void SetData(string url, string json)  //updates row specified by URL. Data specified by Json needs to be serialized
        {
            string newUrl = apiLocation + url;

            ModifyData(newUrl, "PUT", json);
        }

        //--------****|IMPORTANT|*****------\\
        //All data except ID can't be NULL
        public void CreateData(string url, string json)  //creates new row specified by URL. Data specified by Json needs to be serialized
        {
            string newUrl = apiLocation + url;

            ModifyData(newUrl, "POST", json);
        }

        public void DeleteData(string url)  //deletes row specified by URL
        {
            string newUrl = apiLocation + url;

            RemoveData(newUrl).Wait();
        }


        //function used to modify data specified by Url and Method.
        //don't use this function use the functions above.
        private string ModifyData(string url, string method)
        {
            var rxcui = "198440";
            var request = HttpWebRequest.Create(string.Format(url, rxcui));
            request.ContentType = "application/json";
            request.Method = method;

            return GetWebResponse(request);
        }

        private void ModifyData(string url, string method, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            if (method == "POST")
            {
                var response = client.PostAsync(url, content);
                if (response.IsCompleted)
                {
                    Console.WriteLine("||------||Post Async Completed||------||");
                }
            }
            else
            {
                var response = client.PutAsync(url, content);
                if (response.IsCompleted)
                {
                    Console.WriteLine("||------||Put Async Completed||------||");
                }

            }

        }

        private async Task RemoveData(string url)
        {

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            HttpResponseMessage response = null;

            response = await client.DeleteAsync(url).ConfigureAwait(false);

            if(response.IsSuccessStatusCode) {

                Console.Out.WriteLine(@"                Deletely succesfully");

            }
            else
            {
                Console.Out.WriteLine(@"                Deletely unsuccesful-laleluliloly");
            }


            return;

        }

        private string GetWebResponse(WebRequest request)
        {
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            Console.Out.WriteLine("Response contained empty body...");

                            return null;
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", content);

                            return content;
                        }
                    }
                }
            }

            catch (WebException)
            {
                return null;
            }

        }


        //file string must reference filepath
        public async void UploadImage(string file)
        {
            
            try
            {

                string fileName = Path.GetFileName(file);

                string json = JsonConvert.SerializeObject(pages.GlobalVariables.loginUser);
                SetData($"/records/user/{pages.GlobalVariables.loginUser.id}",json);

                string ftpImagePath = ftpPath + fileName;

                //FTP webrequest used to upload images to the FTP webserver of frindr
                bool sendToServer = SendFilesByFTP(ftpPassword, ftpUser, file, ftpPath);
                if (!sendToServer)
                {
                    Console.WriteLine("Image could not be uploaded to server");
                }
                else
                {
                    pages.GlobalVariables.loginUser.imagePath = Path.GetFileName(file);

                    string updatedUserJson = JsonConvert.SerializeObject(pages.GlobalVariables.loginUser);

                    SetData($"/records/user/{pages.GlobalVariables.loginUser.id}", updatedUserJson);
                }
            }
            catch (Exception e)
            {
                //debug
                Console.WriteLine("Exception Caught: " + e.ToString());

                return;
            }
        }


        public ImageSource GetImage(string ftpImageName)
        {

            if (ftpImageName == "iets") ftpImageName = "Default.png";
            Uri ftpImageFilePath = new Uri("ftp://frindradmin%2540frindr.nl@ftp.strato.com/images/" + ftpImageName);
             
            if (ftpImageFilePath.Scheme != Uri.UriSchemeFtp)
            {
                return null;
            }

            WebClient request = new WebClient();

            request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            try
            {
                byte[] byteArray = request.DownloadData(ftpImageFilePath.ToString());
                ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));

                return imageSource;
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }


        }


        public bool SendFilesByFTP(string password, string userName, string originFile, string destinationFile)
        {
            
            try
            {
                
                string filename = originFile;
                Uri ftpfullpath = new Uri(destinationFile + Path.GetFileName(originFile));

                FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                ftp.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftp.KeepAlive = false;
                ftp.UseBinary = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;

                byte[] myFile = File.ReadAllBytes(filename);
                ftp.ContentLength = myFile.Length;

                Stream ftpstream = ftp.GetRequestStream();
                ftpstream.Write(myFile, 0, myFile.Length);
                ftpstream.Close();
                ftpstream.Flush();

                return true;
            }
            catch (WebException e)
            {
                String status = ((FtpWebResponse)e.Response).StatusDescription;
                return false;
            }
            
        }

    }
}
