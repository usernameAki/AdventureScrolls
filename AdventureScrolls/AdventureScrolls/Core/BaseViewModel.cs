using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureScrolls.Core
{
    public abstract class BaseViewModel : ObservableObjects
    {
        protected INavigationService NavigationService { get; }
        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
    }
}
