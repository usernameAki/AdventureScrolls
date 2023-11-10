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
            EditScroll = new Command(o => 
            {
                var parameter = new NavigationParameters();
                parameter.Add("Scroll", o);
                NavigationService.NavigateAsync("WriteAdventureView", parameter);
            });
            RemoveScroll = new Command(o => Scribe.RemoveScroll(o));
        }
    }
}
