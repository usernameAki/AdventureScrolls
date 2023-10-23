using AdventureScrolls.Core;
using Xamarin.Forms;
using AdventureScrolls.View;
using AdventureScrolls.Model;
using Rg.Plugins.Popup.Services;
using AdventureScrolls.Services;

namespace AdventureScrolls.ViewModel
{
    public class WriteAdventureViewModel : BaseViewModel
    {
        public ScrollModel Scroll { get; set; }
        public Command MoodButtonClicked { get; }
        public Command StoreScroll {  get; }
        public Command ChangeMood {  get; }
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
                Scroll.CleanData();
            });



            ChangeMood = new Command(parameter =>
            {
                _scrollCreatorService.NewScroll.Mood = parameter.ToString();
                PopupNavigation.Instance.PopAsync();
            });

        }

    }
}
