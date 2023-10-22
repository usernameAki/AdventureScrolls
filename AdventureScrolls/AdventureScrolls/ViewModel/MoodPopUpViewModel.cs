using AdventureScrolls.Core;
using AdventureScrolls.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace AdventureScrolls.ViewModel
{
    public class MoodPopUpViewModel : BaseViewModel
    {
        public Command ChangeMood { get; }
        private readonly IScrollCreatorService _scrollCreatorService;
        public MoodPopUpViewModel()
        {
            _scrollCreatorService = DependencyService.Get<IScrollCreatorService>();

            ChangeMood = new Command(parameter =>
            {
                _scrollCreatorService.NewScroll.Mood = parameter.ToString();
                PopupNavigation.Instance.PopAsync();
            });
        }
    }
}
