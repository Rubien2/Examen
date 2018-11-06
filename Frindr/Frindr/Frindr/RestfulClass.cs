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
            }
            catch (Exception e)
            {
                //debug
                Console.WriteLine("Exception Caught: " + e.ToString());

                return;
            }
        }


        public string GetImage(string ftpImageName)
        {
            try
            {
                string ftpImageFilePath = "ftp://frindradmin%2540frindr.nl@ftp.strato.com/images/" + ftpImageName;

                // Create a new WebClient instance.
                WebClient myWebClient = new WebClient();
                myWebClient.Credentials = new System.Net.NetworkCredential(ftpUser, ftpPassword);

                Console.WriteLine("Downloading " + ftpImageFilePath);
                byte[] myDataBuffer = myWebClient.DownloadData(ftpImageFilePath);

                string download = Encoding.ASCII.GetString(myDataBuffer);
                Console.WriteLine(download);

                return download;


            }
            catch (Exception e)
            {
                Console.WriteLine("Get Image exception:" + e.ToString());

                return e.ToString();
            }
        }


        public static bool SendFilesByFTP(string password, string userName, string originFile, string destinationFile)
        {
            try
            {

                //
                Uri severUri = new Uri(destinationFile);

                //
                if (severUri.Scheme != Uri.UriSchemeFtp)
                    return false;

                //
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(severUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.UsePassive = false;//true;
                request.KeepAlive = false;
                request.Timeout = System.Threading.Timeout.Infinite;
                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                request.Credentials = new NetworkCredential(userName, password);

                StreamReader sourceStream = new StreamReader(originFile);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                requestStream.Dispose();
                
                //
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //
                response.Close();
                response.Dispose();

                //
                return true;
            }
            catch (Exception e)
            {
                //
                Console.WriteLine("SendFilesByFTP", e);

                //
                return (false);
            }
        }

    }
}
