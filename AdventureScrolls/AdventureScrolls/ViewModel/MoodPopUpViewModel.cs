using AdventureScrolls.Core;
using AdventureScrolls.Model;
using Prism.Navigation;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AdventureScrolls.ViewModel
{
    public class MoodPopUpViewModel : BaseViewModel, INavigationAware
    {
        public ScrollModel Scroll {  get; set; }
        public Command ChangeMood { get; }
        public MoodPopUpViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Changes Mood in pased ScrollModel, and closes itself.
            ChangeMood = new Command(parameter =>
            {
                Scroll.Mood = parameter.ToString();
                PopupNavigation.Instance.PopAsync();
            });
        }
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
            //After navigating to this popup WE NEED to pass ScrollModel as parameter, in order to make changes.
        {
            Scroll = parameters.GetValue<ScrollModel>("Scroll");
        }
    }
}
