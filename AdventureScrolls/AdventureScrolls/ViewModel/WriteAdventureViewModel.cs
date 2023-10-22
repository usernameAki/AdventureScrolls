using AdventureScrolls.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using AdventureScrolls.View;
using AdventureScrolls.Model;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using AdventureScrolls.Services;

namespace AdventureScrolls.ViewModel
{
    public class WriteAdventureViewModel : BaseViewModel
    {
        public ScrollModel Scroll { get; set; }

        private string _moodImage;
        public string MoodImage
        {
            get => _moodImage;
            set
            {
                _moodImage = value;
                OnPropertyChanged();
            }
        }
        public Command MoodButtonClicked { get; }
        public Command StoreScroll {  get; }

        private readonly IScrollCreatorService _scrollCreatorService;
        private readonly IScribeService _scribeService;
        public WriteAdventureViewModel()
        {
            //Pass new scroll through DI
            _scrollCreatorService = DependencyService.Get<IScrollCreatorService>();
            _scribeService = DependencyService.Get<IScribeService>();

            Scroll = _scrollCreatorService.NewScroll;



            //Commands
            MoodButtonClicked = new Command(o => PopupNavigation.Instance.PushAsync(new MoodPopUpView()));
            StoreScroll = new Command(o => 
            { 
                _scribeService.StoreNewScroll(Scroll);
            }); 
            
        }

    }
}
