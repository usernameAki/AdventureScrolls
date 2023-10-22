using AdventureScrolls.Model;

namespace AdventureScrolls.Services
{
    public class ScrollCreatorService : IScrollCreatorService
    {
        public ScrollModel NewScroll { get; set; }
        public ScrollCreatorService()
        {
            NewScroll = new ScrollModel();

        }
    }
}
