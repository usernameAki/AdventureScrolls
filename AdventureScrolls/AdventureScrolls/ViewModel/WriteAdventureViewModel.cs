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
            _editingMode = false;
            _scrollCreatorService = DependencyService.Get<IScrollCreatorService>();
            _scribeService = DependencyService.Get<IScribeService>();
            Scroll = _scrollCreatorService.NewScroll;


            //Commands
            //
            //Calls popup to change mood.
            MoodButtonClicked = new Command(o => 
            {
                var parameter = new NavigationParameters();
                parameter.Add("Scroll", Scroll);
                navigationService.NavigateAsync("MoodPopUpView", parameter);
            });

            //Stores new scroll
            //If editingMode is false, then this command will add and save new ScrollModel in our diary.
            //Otherwise, command will override existing entry and save file. After saving editingMode will be switched off.
            StoreScroll = new Command(o => 
            {
                if (string.IsNullOrEmpty(Scroll.Title)) //Checks if title is empty.
                {
                    Application.Current.MainPage.DisplayAlert("Attention", "Storing unnamed Scrolls is forbidden!", "Yes Your Highness");
                }else if(string.IsNullOrEmpty(Scroll.ScrollContent)) // Checks if content is empty.
                {
                    Application.Current.MainPage.DisplayAlert("Attention", "Storing Scrolls without content is forbidden!", "Yes Your Highness");
                }
                else // If entry have title and content, then it will be saved.
                {
                    if (!_editingMode)
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
                }
                
            });

        }

        public void OnNavigatedFrom(INavigationParameters parameters) //INavigationAware interface. It has to be here...
        {
        }

        /// <summary>
        /// When navigated to this VM with parameter of type ScrollModel, then we gonna turn on editingMode.
        /// EditingMode allow us to operate on passed ScrollModel in order to change existing entry in diary.
        /// </summary>
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters.GetValue<ScrollModel>("Scroll") != null)
            {
                _editingMode = true;
                Scroll = parameters.GetValue<ScrollModel>("Scroll");
            }
        }
    }
}
