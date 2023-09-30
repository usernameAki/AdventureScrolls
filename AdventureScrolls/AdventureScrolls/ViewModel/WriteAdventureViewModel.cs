using AdventureScrolls.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using AdventureScrolls.View;
using AdventureScrolls.Model;

namespace AdventureScrolls.ViewModel
{
    public class WriteAdventureViewModel : BaseViewModel
    {
        private  ScrollModel _newScroll;
        public ScrollModel NewScroll 
        {
            get => _newScroll;
            set
            {
                _newScroll = value;
                OnPropertyChanged();
            }
        }
        public Command MoodButtonClicked { get; }
        public Command ChangeMood { get; }
        public WriteAdventureViewModel()
        {
            NewScroll = new ScrollModel();
            MoodButtonClicked = new Command(o => Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new MoodPopUpView()));
           // ChangeMood = new Command(o =>);
        }
    }
}
