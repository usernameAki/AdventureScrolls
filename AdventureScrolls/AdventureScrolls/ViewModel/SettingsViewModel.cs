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
                if(await _googleUserAuthenticationService.LoginToGoogleDrive())
                {
                    await Application.Current.MainPage.DisplayAlert("Login succeed!",
                        $"Logged as: {_googleUserAuthenticationService.userName}","OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Login failed!", "", "OK");
                }

            });

            SaveFilesOnGoogle = new Command(async o =>
            {
                if (await _googleDriveDataService.UploadScrollLibrary())
                {
                    await Application.Current.MainPage.DisplayAlert("Upload succeed!", "", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Upload failed!", "", "OK");
                }
            });

            DownloadFilesOnGoogle = new Command(async o =>
            {
                if (await _googleDriveDataService.DownloadScrollLibrary())
                {
                    _scribeService.GetScrolls();
                    await Application.Current.MainPage.DisplayAlert("Download succeed!", "", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Download failed!", "", "OK");
                }
            });
        }
    }
}
