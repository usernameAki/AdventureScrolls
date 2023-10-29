﻿using AdventureScrolls.Model;
using System.Collections.ObjectModel;

namespace AdventureScrolls.Services
{
    public interface IScribeService
    {
        ObservableCollection<ScrollModel> ScrollLibrary { get; }
        ObservableCollection<ScrollModel> GetScrolls();
        void RemoveScroll(object scrollToDelete);
        void StoreNewScroll(ScrollModel scrollToStore);
    }
}