using AdventureScrolls.Core;
using AdventureScrolls.Model;
using AdventureScrolls.Services;
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
        public IScribeService _scribe { get; }
        public Command EditScroll { get; }
        public Command RemoveScroll { get; }
        public AdventureScrollsViewModel()
        {
            _scribe = DependencyService.Get<IScribeService>();
            EditScroll = new Command(o => Console.WriteLine(nameof(o)));
            RemoveScroll = new Command(o => _scribe.RemoveScroll(o));
        }
    }
}
