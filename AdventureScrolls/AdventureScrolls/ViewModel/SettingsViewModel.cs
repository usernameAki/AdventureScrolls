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
        private IScribeService _scribeService { get;}
        private IGoogleUserAuthenticationService _googleUserAuthenticationService { get;}
        private IGoogleDriveDataService _googleDriveDataService { get;}
        public Command ConnectToGD { get; }
        public Command SaveFilesOnGoogle { get; }
        public Command DownloadFilesOnGoogle { get; }
        public SettingsViewModel(INavigationService navigationService) : base(navigationService)
        {
            _scribeService = DependencyService.Get<IScribeService>();
            _googleUserAuthenticationService = DependencyService.Get<IGoogleUserAuthenticationService>();
            _googleDriveDataService = DependencyService.Get<IGoogleDriveDataService>();

            ConnectToGD = new Command(async o =>
            {
                await _googleUserAuthenticationService.LoginToGoogleDrive();
            });

            SaveFilesOnGoogle = new Command(async o =>
            {
                await _googleDriveDataService.UploadScrollLibrary();
            });

            DownloadFilesOnGoogle = new Command(async o =>
            {
                await _googleDriveDataService.DownloadScrollLibrary();
                _scribeService.GetScrolls();
            });
        }
    }
}
