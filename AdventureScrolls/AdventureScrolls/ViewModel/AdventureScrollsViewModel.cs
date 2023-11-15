using AdventureScrolls.Core;
using AdventureScrolls.Model;
using AdventureScrolls.Services;
using AdventureScrolls.View;
using Prism.Navigation;
using Prism.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AdventureScrolls.ViewModel
{
    public class AdventureScrollsViewModel : BaseViewModel
    {
        public IScribeService Scribe { get; }
        public Command EditScroll { get; }
        public Command RemoveScroll { get; }
        public AdventureScrollsViewModel(INavigationService navigationService) :base(navigationService) 
        {
            Scribe = DependencyService.Get<IScribeService>();

            //Navigates to WriteAdventureView in editingMode.
            EditScroll = new Command(o => 
            {
                var parameter = new NavigationParameters();
                parameter.Add("Scroll", o);
                NavigationService.NavigateAsync("WriteAdventureView", parameter);
            });

            //As it says. Removes entry from diary.
            RemoveScroll = new Command(async o =>
            {
                ScrollModel temp = new ScrollModel(o as ScrollModel);
                //First asks user if he intentionally wants to discard entry.
                bool answer = await Application.Current.MainPage.DisplayAlert("Discard this scroll?", temp.Title, "Yes", "No");
                if (answer) Scribe.RemoveScroll(o);
            });
        }
    }
}
