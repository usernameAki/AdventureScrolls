using AdventureScrolls.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AdventureScrolls.ViewModel
{
    public class AdventureScrollsViewModel
    {
        public ObservableCollection<ScrollModel> Scrolls { get; set; }

        public AdventureScrollsViewModel()
        {
            Scrolls = new ObservableCollection<ScrollModel>
            {
                new ScrollModel("tytuł 1", "mój wpis 1"),
                new ScrollModel("tytuł 2", "mój wpis 2"),
                new ScrollModel("tytuł 3", "mój wpis 3")
            };
        }
    }
}
