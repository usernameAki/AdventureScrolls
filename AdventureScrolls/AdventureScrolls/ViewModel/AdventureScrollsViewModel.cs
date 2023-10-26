using AdventureScrolls.Core;
using AdventureScrolls.Model;
using AdventureScrolls.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace AdventureScrolls.ViewModel
{
    public class AdventureScrollsViewModel : BaseViewModel
    {
        public ObservableCollection<ScrollModel> ScrollLibrary { get; }
        public IScribeService _scribe { get; }
        public AdventureScrollsViewModel()
        {
            _scribe = DependencyService.Get<IScribeService>();
            ScrollLibrary = _scribe.ScrollLibrary;

        }
    }
}
