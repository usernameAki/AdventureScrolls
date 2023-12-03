using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using AdventureScrolls.Model;
using System.Linq;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Reflection;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace AdventureScrolls.Services
{
    //This class maintain authorization process between app and user's google drive for backup purposes.
    //After authorization succeed, user can store diary data on google drive or download existing data.
    public class GoogleDriveService
    {
        private DriveService _service;
        private string _scrollLibraryFilePath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScrollLibrary.json");

        /// <summary>
        /// Gets ClientID from embedded json file.
        /// </summary>
        /// <returns>Returns string with client ID</returns>
        private string GetClientID()
        {
            string clientID = string.Empty;
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(GoogleDriveService)).Assembly;

            //Read client ID property from json file.
            Stream stream = assembly.GetManifestResourceStream("AdventureScrolls.Resources.GDToken.json");
            using ( var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                var jObject = JObject.Parse(json);
                clientID = jObject["installed"]["client_id"].ToString();
            }
            return clientID;
        }
        public async Task ConnectToGoogleDrive()
        {
            string[] oauthToken = new string[2];
            bool isTokenValid = false;
            oauthToken[0] = await SecureStorage.GetAsync("oauth_token");
            oauthToken[1] = await SecureStorage.GetAsync("oauth_refresh_token");
            if (oauthToken[0] != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    InitializeDriveService(oauthToken[i]);
                    try
                    {
                        await RetrieveAppDataFileByName();
                        isTokenValid = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            } else if(oauthToken[0] != null ||  !isTokenValid)
            {
                await AuthenticateUser();
            }
            
        }

        /// <summary>
        /// Authorizes connection between app and users google drive using OAuth2.
        /// </summary>
        /// <returns></returns>
        public async Task AuthenticateUser()
        {
            //Authentication URL
            var authUrl = new Uri(
                $"https://accounts.google.com/o/oauth2/v2/auth"
                + "?client_id=" + GetClientID()
                + "&response_type=code"
                + "&scope=https://www.googleapis.com/auth/drive.appdata"
                + "&redirect_uri=com.companyname.adventurescrolls:/redirect"
                + "&prompt=consent");

            //Callback to app
            var callbackUrl = new Uri("com.companyname.adventurescrolls://");

            WebAuthenticatorResult authResult = null;
            try //login to google
            {
                //access google login page, and get authentication results
                authResult = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //If authorization succeed, app will ask for access token.
            if (authResult != null)
            {
                Console.WriteLine("authentication successfully.");
                var authorizationCode = authResult.Properties["code"];
                var accessToken = await GetAccessTokenAsync(authorizationCode);

                //Access Token and refresh Token will be stored in devices Secure Storage.
                try
                {
                    await SecureStorage.SetAsync("oauth_token", accessToken.AccessToken);
                    await SecureStorage.SetAsync("oauth_refresh_token", accessToken.RefreshToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                InitializeDriveService(accessToken.AccessToken);
            }
            else { Console.WriteLine("Authentication failed!"); }
        }
        /// <summary>
        /// Exchanges authorization code for Access Token, after successfully authorization.
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Returns Access Token</returns>
        public async Task<TokenModel> GetAccessTokenAsync(string code)
        {
            var requestUrl =
                $"https://oauth2.googleapis.com/token"
                + "?code=" + code
                + "&client_id=" + GetClientID()
                + "&redirect_uri=com.companyname.adventurescrolls:/redirect"
                + "&grant_type=authorization_code";
            var httpClient = new HttpClient();


            //Sends request to receive token
            var response = await httpClient.PostAsync(requestUrl, null);

            var json = await response.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<TokenModel>(json);
            return accessToken;
        }
        public async Task<TokenModel> RefreshAccessToken(string refreshToken)
        {
            var requestUrl =
                "https://oauth2.googleapis.com/token"
                + "&client_id=" + GetClientID()
                + "&refresh_token=" + refreshToken
                + "&grant_type=refresh_token";
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(requestUrl, null);
            var json = await response.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<TokenModel>(json);
            return accessToken;
        }
        /// <summary>
        /// Initializes the Google Drive Service with the gives access token,
        /// allowing the application to perform authorized operations on the user's Google Drive.
        /// </summary>
        /// <param name="accessToken"></param>
        public void InitializeDriveService(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);
            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "adventurescrolls",
            });
        }
        /// <summary>
        /// Uploads ScrollLibrary to appData Folder in Google Drive.
        /// </summary>
        /// <returns></returns>
        public async Task UploadScrollLibrary()
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(_scrollLibraryFilePath),
                Parents = new List<string> { "appDataFolder" }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(_scrollLibraryFilePath,
                                    FileMode.Open))
            {
                request = _service.Files.Create(
                    fileMetadata, stream, "application/json");
                request.Fields = "id";
                await request.UploadAsync();
            }
        }

        /// <summary>
        /// Searches for ScrollLibrary file on Google Drive.
        /// </summary>
        /// <returns></returns>
        public async Task<Google.Apis.Drive.v3.Data.File> RetrieveAppDataFileByName()
        {
            var request = _service.Files.List();
            request.Spaces = "appDataFolder";
            request.Fields = "files(id, name)";
            request.Q = $"name = 'ScrollLibrary.json'";
            var response = await request.ExecuteAsync();
            return response.Files.FirstOrDefault(); 
        }

        /// <summary>
        /// Downloads and saves fetched ScrollLibrary file on device.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task DownloadScrollLibrary(string fileId)
        {
            //Downloading file.
            var request = _service.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0;

            //Reads and deserializes json file into Collection.
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                var downloadedScrollLibrary = serializer.Deserialize<ObservableCollection<ScrollModel>>(jsonReader);

                //Saves fetched data on device.
                File.WriteAllText(_scrollLibraryFilePath, JsonConvert.SerializeObject(downloadedScrollLibrary));
            }
        }
    }
}
