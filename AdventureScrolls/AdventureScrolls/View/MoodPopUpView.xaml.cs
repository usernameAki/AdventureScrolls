using AdventureScrolls.Model;
using AdventureScrolls.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdventureScrolls.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MoodPopUpView : Rg.Plugins.Popup.Pages.PopupPage
    {
        private readonly WriteAdventureViewModel writeAdventureVM;
        public MoodPopUpView (WriteAdventureViewModel wavm) // We have to pass ViewModel to update MoodImage.
		{
			InitializeComponent ();
            writeAdventureVM = wavm;
        }
        protected override void OnDisappearingAnimationEnd() 
        {
            // When we change MoodImage before popup closes itself,
            // view will be not updated(And i don't know why.. (。_。)
            // Maybe because View is not active?
            // Anyway we have to update view after popup closes itself.
            writeAdventureVM.UpdateMoodImage();
        }
    }
}