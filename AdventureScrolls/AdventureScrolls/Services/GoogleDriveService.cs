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
using System.Diagnostics.SymbolStore;


// I still need to clean and organize code here... （￣。。￣）
//scalić funkcje GetAccessTokenAsync oraz RefreshAccessToken


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

        //We call this method first, to check if there is any access token already stored on device.
        //If no, then standard procedure of authorization process will be launched.
        //If authorization process will succeed, then access token and refresh token will be stored on device Secure Storage.
        
        /// <summary>
        /// Login to google drive.
        /// </summary>
        /// <returns></returns>
        public async Task LoginToGoogleDrive()
        {
            if (await LoginAgain())
            {
                Console.WriteLine("LoginToGoogleDrive. LoginAgain Succeed.");
            }
            else if (await AuthenticateUser())
            {
                Console.WriteLine("LoginToGoogleDrive. LoginAgain Failed.");
                Console.WriteLine("LoginToGoogleDrive. AuthenticateUser Succeed.");
            } else
            {
                Console.WriteLine("LoginToGoogleDrive. Megumin broke (T_T)");
            }
        }
        /// <summary>
        /// This method gets access Token from KeyStore,
        /// and tries to login us again in google drive.
        /// </summary>
        /// <returns>Retunr true if login succeed. False if failed.</returns>
        private async Task<bool> LoginAgain()
        {
            string token;
            string refreshToken;
            //Try to retrive token from KeyStore.
            try
            {
                token = await SecureStorage.GetAsync("oauth_token");
                refreshToken = await SecureStorage.GetAsync("oauth_refresh_token");
            }
            catch (Exception ex)
            { 
                Console.WriteLine("LoginAgain. Retriving token from KeyStore failed. Exception:" + ex.Message); 
                return false; //if fails to retrive key, return false.
            }
            //After retriving token, we are going to try token validity.
            InitializeDriveService(token);
            if (await CheckTokenValidity())
            {
                return true; //If actual token still works.
            }
            //If token is not valid, we are trying refresh token.
            TokenModel newToken = await GetAccessTokenAsync(null, refreshToken);
            if (newToken != null)
            {
                await StoreTokenInKeyStore(newToken);
                InitializeDriveService(newToken.AccessToken);
                return true;
            } else return false;
        }

        /// <summary>
        /// Authorizes connection between app and users google drive using OAuth2.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AuthenticateUser()
        {
            //Authentication URL
            Uri authUrl = new Uri(
                $"https://accounts.google.com/o/oauth2/v2/auth"
                + "?client_id=" + GetClientID()
                + "&response_type=code"
                + "&scope=https://www.googleapis.com/auth/drive.appdata"
                + "&redirect_uri=com.companyname.adventurescrolls:/redirect"
                + "&prompt=consent");

            //Callback to app
            Uri callbackUrl = new Uri("com.companyname.adventurescrolls://");

            WebAuthenticatorResult authResult = null;
            try //login to google
            {
                //access google login page, and get authentication results
                authResult = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthenticateUser. Authorization failed. Exception: " + ex.Message);
                return false;
            }
            //If authorization succeed, app will ask for access token.
            if (authResult == null)
            {
                Console.WriteLine("Authentication failed!");
                return false;
            }
            Console.WriteLine("AuthenticateUser. Authentication succeed.");
            string authorizationCode = authResult.Properties["code"];
            try
            {
                TokenModel accessToken = await GetAccessTokenAsync(authorizationCode, null);
                //Access Token and refresh Token will be stored in devices Secure Storage.
                await StoreTokenInKeyStore(accessToken);
                InitializeDriveService(accessToken.AccessToken);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthenticateUser. Failed to retrive new token. Exception: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Stores Access Token and refresh token inside KeyStore.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task StoreTokenInKeyStore(TokenModel token)
        {
            try
            {
                await SecureStorage.SetAsync("oauth_token", token.AccessToken);
                await SecureStorage.SetAsync("oauth_refresh_token", token.RefreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Requests for a new access token to google drive.
        /// Function checks is refreshToken is avaible, and will prioritize refreshing.
        /// Otherwise it will use code.
        /// One of the passed values can be null.
        /// </summary>
        /// <returns>Returns Access Token</returns>
        public async Task<TokenModel> GetAccessTokenAsync(string code, string refreshToken)
        {
            string requestUrl = $"https://oauth2.googleapis.com/token";
            if (refreshToken != null)
            {
                requestUrl +=
                "&client_id=" + GetClientID()
                + "&refresh_token=" + refreshToken
                + "&grant_type=refresh_token";
            }
            else if (code != null)
            {
                requestUrl +=
                "?code=" + code
                + "&client_id=" + GetClientID()
                + "&redirect_uri=com.companyname.adventurescrolls:/redirect"
                + "&grant_type=authorization_code";
            }
            else return null;

            //Sends request to receive token
            var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.PostAsync(requestUrl, null);
                var json = await response.Content.ReadAsStringAsync();
                var accessToken = JsonConvert.DeserializeObject<TokenModel>(json);
                return accessToken;
            } catch (Exception ex)
            {
                Console.WriteLine("GetAccessTokenAsync Failed. Exception: " + ex.Message);
                return null;
            }
        }


        public async Task<TokenModel> RefreshAccessToken(string refreshToken) //DRY principle is sreaming here ┗|｀O′|┛
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
            try
            {
                var response = await request.ExecuteAsync();
                return response.Files.FirstOrDefault();
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message);   
            }
            return null;
        }
        private async Task<bool> CheckTokenValidity()
        {
            try
            {
                var request = _service.About.Get();
                request.Fields = "user";
                var about = await request.ExecuteAsync();
                Console.WriteLine("Loged in as: " + about.User.DisplayName);
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine("CheckTokenValidity. Invalid token. Exception: " + ex.Message);
                return false;
            }
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
