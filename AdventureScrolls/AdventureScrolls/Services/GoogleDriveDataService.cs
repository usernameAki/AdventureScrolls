using AdventureScrolls.Model;
using Google.Apis.Drive.v3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AdventureScrolls.Services
{
    public class GoogleDriveDataService : IGoogleDriveDataService
    {
        private IScribeService _scribeService;
        private IGoogleUserAuthenticationService _userAuthenticationService;
        public GoogleDriveDataService()
        {
            _scribeService = DependencyService.Get<IScribeService>();
            _userAuthenticationService = DependencyService.Get<IGoogleUserAuthenticationService>();
        }

        /// <summary>
        /// Uploads ScrollLibrary on Google Drive.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UploadScrollLibrary()
        {
            if(!await _userAuthenticationService.CheckTokenValidity()) return false;
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = "ScrollLibrary.json",
                Parents = new List<string> { "appDataFolder" }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(_scribeService.filePath,
                                    FileMode.Open))
            {
                request = _userAuthenticationService.driveService.Files.Create(
                    fileMetadata, stream, "application/json");
                request.Fields = "id";
                try
                {
                    await request.UploadAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UploadScrollLibrary. Uploading failed. " +
                    $"Exception: {ex.Message}");
                }
            }
            return false;
        }
        /// <summary>
        /// Downloads ScrollLibrary.json from google drive,
        /// and saves it on device.
        /// </summary>
        /// <returns>Result of retriving data.</returns>
        public async Task<bool> DownloadScrollLibrary()
        {
            if (!await _userAuthenticationService.CheckTokenValidity()) return false;
            var file = await RetrieveAppDataFileByName("ScrollLibrary.json");
            if (file != null)
            {
                try
                {
                    //Downloading file.
                    var request = _userAuthenticationService.driveService.Files.Get(file.Id);
                    var stream = new MemoryStream();
                    await request.DownloadAsync(stream);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        var downloadedScrollLibrary = serializer.Deserialize<ObservableCollection<ScrollModel>>(jsonReader);

                        //Saves data on device.
                        _scribeService.StoreScrolls(downloadedScrollLibrary);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"RetrieveAppDataFileByName. " +
                        $"Failed to donload and deserialize file. Exception: {ex.Message}");
                }
            }

            //If file is null or downloading/deserializing file failed, method will return false.
            Console.WriteLine("DownloadScrollLibrary. Downloading ScrollLibrary failed.");
            return false;
        }

        /// <summary>
        /// Searches Google Drive AppData folder for a file with passed name.
        /// </summary>
        /// <returns>AppData file ID.</returns>
        private async Task<Google.Apis.Drive.v3.Data.File> RetrieveAppDataFileByName(string fileName)
        {
            var request = _userAuthenticationService.driveService.Files.List();
            request.Spaces = "appDataFolder";
            request.Fields = "files(id, name)";
            request.Q = $"name = '{fileName}'";
            try
            {
                var response = await request.ExecuteAsync();
                return response.Files.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RetrieveAppDataFileByName. Failed to retrive item named:{fileName}. Exception: {ex.Message}");
            }
            return null;
        }
    }
}
