using AdventureScrolls.Model;
using System.Collections.ObjectModel;

namespace AdventureScrolls.Services
{
    public interface IScribeService
    {
        ObservableCollection<ScrollModel> GetScrolls();
        void StoreNewScroll(ScrollModel scrollToStore);
    }
}