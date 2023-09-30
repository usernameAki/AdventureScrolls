﻿using AdventureScrolls.Core;
using AdventureScrolls.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AdventureScrolls.ViewModel
{
    public class AdventureScrollsViewModel : BaseViewModel
    {
        public ObservableCollection<ScrollModel> Scrolls { get; set; }

        public AdventureScrollsViewModel()
        {
            Scrolls = new ObservableCollection<ScrollModel>
            {
                new ScrollModel("Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit...\"", "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum."),
                new ScrollModel("tytuł 2", "mój wpis 2"),
                new ScrollModel("tytuł 3", "mój wpis 3"),
                new ScrollModel("tytuł 4", "mój wpis 4"),
                new ScrollModel("tytuł 5", "mój wpis 5"),
                new ScrollModel("tytuł 6", "mój wpis 6"),
                new ScrollModel("tytuł 7", "mój wpis 7"),
                new ScrollModel("tytuł 8", "mój wpis 8"),
                new ScrollModel("tytuł 9", "mój wpis 9"),
                new ScrollModel("tytuł 10", "mój wpis 10"),
                new ScrollModel("tytuł 11", "mój wpis 11"),
                new ScrollModel("tytuł 12", "mój wpis 12"),
                new ScrollModel("tytuł 13", "mój wpis 13"),
                new ScrollModel("tytuł 14", "mój wpis 14"),
                new ScrollModel("tytuł 15", "mój wpis 15")
            };
        }
    }
}
