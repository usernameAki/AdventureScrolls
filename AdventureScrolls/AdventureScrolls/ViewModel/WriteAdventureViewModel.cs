using AdventureScrolls.Core;
using Xamarin.Forms;
using AdventureScrolls.View;
using AdventureScrolls.Model;
using Rg.Plugins.Popup.Services;
using AdventureScrolls.Services;
using Prism.Navigation;
using System.ComponentModel;

namespace AdventureScrolls.ViewModel
{
    public class WriteAdventureViewModel : BaseViewModel, INavigationAware
    {
        private bool _editingMode;
        private ScrollModel _scroll;
        public ScrollModel Scroll 
        {
            get => _scroll; 
            set
            {
                _scroll = value;
                OnPropertyChanged();
            }
        }
        public Command MoodButtonClicked { get; }
        public Command StoreScroll {  get; }
        public Command ChangeMood {  get; }
        private readonly IScrollCreatorService _scrollCreatorService;
        private readonly IScribeService _scribeService;
        public WriteAdventureViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Pass new scroll through DI
            _editingMode = false;
            _scrollCreatorService = DependencyService.Get<IScrollCreatorService>();
            _scribeService = DependencyService.Get<IScribeService>();
            Scroll = _scrollCreatorService.NewScroll;



            //Commands

            //Calls popup to change mood
            MoodButtonClicked = new Command(o => 
            {
                var parameter = new NavigationParameters();
                parameter.Add("Scroll", Scroll);
                navigationService.NavigateAsync("MoodPopUpView", parameter);
            });

            //Stores new scroll
            StoreScroll = new Command(o => 
            { 
                if(!_editingMode)
                {
                    _scribeService.StoreNewScroll(Scroll);
                    Scroll.CleanData();
                }
                else
                {
                    _scribeService.OverrideScroll();
                    _editingMode = false;
                    Scroll = _scrollCreatorService.NewScroll;
                    navigationService.NavigateAsync("/MainView");
                }
            });


            //Changes mood and closes popup
            //ChangeMood = new Command(parameter =>
            //{
            //    Scroll.Mood = parameter.ToString();
            //    PopupNavigation.Instance.PopAsync();
            //});

        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters.GetValue<ScrollModel>("Scroll") != null)
            {
                _editingMode = true;
                Scroll = parameters.GetValue<ScrollModel>("Scroll");
            }
            //Scroll.ScrollContent = temp.ScrollContent;
            //Scroll.Title = temp.Title;
            //Scroll.EntryDate = temp.EntryDate;
            //Scroll.Mood = temp.Mood;
        }
    }
}
