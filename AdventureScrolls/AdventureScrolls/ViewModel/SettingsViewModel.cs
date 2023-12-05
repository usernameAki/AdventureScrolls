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
        private GoogleDriveService _googleDriveService { get;}
        private IScribeService _scribeService { get;}
        public Command ConnectToGD { get; }
        public Command SaveFilesOnGoogle { get; }
        public Command DownloadFilesOnGoogle { get; }
        public SettingsViewModel(INavigationService navigationService) : base(navigationService)
        {
            _googleDriveService = new GoogleDriveService();
            _scribeService = DependencyService.Get<IScribeService>();

            ConnectToGD = new Command(async o =>
            {
                await _googleDriveService.LoginToGoogleDrive();
            });

            SaveFilesOnGoogle = new Command(async o =>
            {
                await _googleDriveService.UploadScrollLibrary();
            });

            DownloadFilesOnGoogle = new Command(async o =>
            {
                var file = await _googleDriveService.RetrieveAppDataFileByName();
                await _googleDriveService.DownloadScrollLibrary(file.Id);
                _scribeService.GetScrolls();
            });
        }
    }
}
