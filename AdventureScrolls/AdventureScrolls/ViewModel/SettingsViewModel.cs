using AdventureScrolls.Core;
using AdventureScrolls.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AdventureScrolls.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        private string _userName;
        public string userName 
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
        //Services
        private IScribeService _scribeService { get;}
        private IGoogleUserAuthenticationService _googleUserAuthenticationService { get;}
        private IGoogleDriveDataService _googleDriveDataService { get;}

        //Commands
        public Command LoginGoogleDrive { get; } //Login into google.
        public Command UploadScrollLibrary { get; } //Makes backup on google drive.
        public Command DownloadScrollLibrary { get; } //Downloads backup from google drive.

        public SettingsViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Services
            _scribeService = DependencyService.Get<IScribeService>();
            _googleUserAuthenticationService = DependencyService.Get<IGoogleUserAuthenticationService>();
            _googleDriveDataService = DependencyService.Get<IGoogleDriveDataService>();

            Task.Run(async () =>
            {
                if (await _googleUserAuthenticationService.LoginAgain())
                {
                    userName = "Logged as: " + _googleUserAuthenticationService.userName;
                }
                else userName = "Please login.";
            });

            //Commands
            LoginGoogleDrive = new Command(async o =>
            {
                if(await _googleUserAuthenticationService.LoginToGoogleDrive())
                {
                    userName = "Logged as: " + _googleUserAuthenticationService.userName;
                    await Application.Current.MainPage.DisplayAlert("Login succeed!",
                        userName, "OK");
                }
                else
                {
                    userName = "Please login.";
                    await Application.Current.MainPage.DisplayAlert("Login failed!", "", "OK");
                }

            });
            UploadScrollLibrary = new Command(async o =>
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
            DownloadScrollLibrary = new Command(async o =>
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
