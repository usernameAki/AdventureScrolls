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
		public MoodPopUpView ()
		{
			InitializeComponent ();
		}
	}
}