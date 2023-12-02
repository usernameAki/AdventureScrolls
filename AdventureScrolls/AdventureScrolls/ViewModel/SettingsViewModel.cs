using AdventureScrolls.Core;
using AdventureScrolls.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace AdventureScrolls.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        public Command ConnectToGD { get; }
        public Command SaveFilesOnGoogle { get; }
        public Command DownloadFilesOnGoogle { get; }
        public Command CheckFile { get; }
        public SettingsViewModel(INavigationService navigationService) : base(navigationService)
        {
            GoogleDriveService googleDriveService = new GoogleDriveService();

            ConnectToGD = new Command(async o =>
            {
                Console.WriteLine("ConnectToGD");
                await googleDriveService.Authenticateuser();
            });

            SaveFilesOnGoogle = new Command(async o =>
            {
                Console.WriteLine("SaveFilesOnGoogle");
                await googleDriveService.UploadScrollLibrary();
            });

            DownloadFilesOnGoogle = new Command(async o =>
            {
                Console.WriteLine("DownloadFilesOnGoogle");
                var file = await googleDriveService.RetrieveAppDataFileByName();
                await googleDriveService.DownloadScrollLibrary(file.Id);
            });
        }
    }
}
