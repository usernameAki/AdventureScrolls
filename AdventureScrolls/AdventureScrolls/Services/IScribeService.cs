using AdventureScrolls.Model;
using System.Collections.ObjectModel;

namespace AdventureScrolls.Services
{
    public interface IScribeService
    {
        string filePath { get; }
        ObservableCollection<ScrollModel> ScrollLibrary { get; }
        void GetScrolls();
        void StoreScrolls(ObservableCollection<ScrollModel> scrollLibrary);
        void RemoveScroll(object scrollToDelete);
        void StoreNewScroll(ScrollModel scrollToStore);
        void OverrideScroll();
    }
}