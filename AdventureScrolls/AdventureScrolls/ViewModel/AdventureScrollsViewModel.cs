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
        private ObservableCollection<ScrollModel> _scrolls {  get; set; }
        public ObservableCollection<ScrollModel> Scrolls 
        {
            get => _scrolls; 
            set
            {
                _scrolls = value;
                OnPropertyChanged();
            }
        }
        IScribeService Scribe { get; set; }

        public AdventureScrollsViewModel()
        {
            Scribe = DependencyService.Get<IScribeService>();
            Scrolls = Scribe.GetScrolls();
            
        }
    }
}
