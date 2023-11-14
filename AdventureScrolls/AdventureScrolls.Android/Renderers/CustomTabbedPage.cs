using AdventureScrolls.Droid.Renderers;
using AdventureScrolls.Droid;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using TabbedPage = Xamarin.Forms.TabbedPage;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPage))]
namespace AdventureScrolls.Droid.Renderers
{
    public class CustomTabbedPage : TabbedPageRenderer
    {
        public CustomTabbedPage(Context context) : base(context)
        {
            
        }
        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            var childViews = GetAllChildViews(ViewGroup);

            var scale = Resources.DisplayMetrics.Density;
            var paddingDp = 5; //indicates icon padding.
            var dpAsPixels = (int)(paddingDp * scale + 0.5f);

            foreach (var childView in childViews)
            {
                if (childView is BottomNavigationItemView tab)
                {
                    tab.SetPadding(tab.PaddingLeft, dpAsPixels, tab.PaddingRight, tab.PaddingBottom);
                    tab.SetIconSize(100); //indicates icon size.
                }
                else if (childView is TextView textView)
                {
                    textView.SetTextColor(Android.Graphics.Color.Transparent);
                }
            }
        }

        List<Android.Views.View> GetAllChildViews(Android.Views.View view)
        {
            if (!(view is ViewGroup group))
            {
                return new List<Android.Views.View> { view };
            }

            var result = new List<Android.Views.View>();

            for (int i = 0; i < group.ChildCount; i++)
            {
                var child = group.GetChildAt(i);

                var childList = new List<Android.Views.View> { child };
                childList.AddRange(GetAllChildViews(child));

                result.AddRange(childList);
            }

            return result.Distinct().ToList();
        }
    }
}