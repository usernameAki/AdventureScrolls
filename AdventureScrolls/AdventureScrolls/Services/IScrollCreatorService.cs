using AdventureScrolls.Model;

namespace AdventureScrolls.Services
{
    public interface IScrollCreatorService
    {
        ScrollModel NewScroll { get; set; }
    }
}