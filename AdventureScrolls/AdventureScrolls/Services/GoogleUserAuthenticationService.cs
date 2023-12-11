using AdventureScrolls.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AdventureScrolls.Services
{
    public class GoogleUserAuthenticationService : IGoogleUserAuthenticationService
    {
        public string userName { get; set; }
        private DriveService _driveService;
        public DriveService driveService
        {
            get => _driveService;
            set => _driveService = value;
        }

        /// <summary>
        /// Login to Google.
        /// </summary>
        public async Task<bool> LoginToGoogleDrive()
        {
            if (await LoginAgain())
            {
                Console.WriteLine("LoginToGoogleDrive. LoginAgain Succeed.");
                return true;
            }
            else if (await AuthenticateUser())
            {
                Console.WriteLine("LoginToGoogleDrive. LoginAgain Failed.");
                Console.WriteLine("LoginToGoogleDrive. AuthenticateUser Succeed.");
                return true;
            }
            else
            {
                Console.WriteLine("LoginToGoogleDrive. Megumin broke (T_T)");
                return false;
            }
        }

        /// <summary>
        /// Gets ClientID from embedded json file.
        /// </summary>
        /// <returns>Returns string with client ID</returns>
        private string GetClientID()
        {
            string clientID;
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(GoogleUserAuthenticationService)).Assembly;

            //Read client ID property from json file.
            Stream stream = assembly.GetManifestResourceStream("AdventureScrolls.Resources.GDToken.json");
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                var jObject = JObject.Parse(json);
                clientID = jObject["installed"]["client_id"].ToString();
            }
            return clientID;
        }
        /// <summary>
        /// Login to google drive.
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// This method gets access Token from KeyStore,
        /// and tries to login us again in google drive.
        /// </summary>
        /// <returns>Retunr true if login succeed. False if failed.</returns>
        public async Task<bool> LoginAgain()
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
            //After retriving token, we are going to check token validity.
            InitializeDriveService(token);
            if (await CheckTokenValidity())
            {
                return true; //If present token still works.
            }
            //If token is not valid, app will try to refresh token.
            TokenModel newToken = await GetAccessTokenAsync(null, refreshToken);
            if (newToken != null)
            {
                await StoreTokenInKeyStore(newToken);
                InitializeDriveService(newToken.AccessToken);
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Redirects user to google login window, and tries to authenticate user.
        /// </summary>
        /// <returns>Result of authentication.</returns>
        private async Task<bool> AuthenticateUser()
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

            WebAuthenticatorResult authResult;
            try //login to google
            {
                //redirects to google login page
                authResult = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthenticateUser. Authorization failed. Exception: {ex.Message}");
                return false;
            }
            if (authResult == null)
            {
                Console.WriteLine("Authentication failed!");
                return false;
            }
            //If authentication succeed, app will ask for access token.
            Console.WriteLine("AuthenticateUser. Authentication succeed.");
            string authorizationCode = authResult.Properties["code"];
            try
            {
                TokenModel accessToken = await GetAccessTokenAsync(authorizationCode, null);
                //Access Token and refresh Token will be stored on device Secure Storage.
                await StoreTokenInKeyStore(accessToken);
                InitializeDriveService(accessToken.AccessToken);
                var getUserName = await CheckTokenValidity();
                return true;
               // return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthenticateUser. Failed to retrive new token. Exception: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Initializes the Google Drive Service with given access token,
        /// allowing the application to perform authorized operations on the user's Google Drive.
        /// </summary>
        private void InitializeDriveService(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);
            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "adventurescrolls",
            });
        }
        /// <summary>
        /// Asks google for a new token.
        /// Method checks is refreshToken is avaible and will prioritize refreshing, 
        /// otherwise it will use passed code.
        /// One of the passed values can be null.
        /// </summary>
        /// <returns>Returns new Token</returns>
        private async Task<TokenModel> GetAccessTokenAsync(string code, string refreshToken)
        {
            string requestUrl = "https://oauth2.googleapis.com/token";
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

            //Send request to receive new token
            var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.PostAsync(requestUrl, null);
                var json = await response.Content.ReadAsStringAsync();
                var accessToken = JsonConvert.DeserializeObject<TokenModel>(json);
                return accessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAccessTokenAsync Failed. Exception: " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Checks if available token is still valid.
        /// </summary>
        /// <returns>Result of token validity.</returns>
        public async Task<bool> CheckTokenValidity()
        {
            //We send simple query to google to get username.
            //If token is not valid app will met Exception, and thus return false. 
            try
            {
                var request = driveService.About.Get();
                request.Fields = "user";
                var about = await request.ExecuteAsync();
                Console.WriteLine("Loged in as: " + about.User.DisplayName);
                userName = about.User.DisplayName;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckTokenValidity. Invalid token. Exception: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Stores Access Token and refresh token inside SecureStorage.
        /// </summary>
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
    }
}
