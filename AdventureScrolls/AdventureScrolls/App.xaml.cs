using AdventureScrolls.Model;
using AdventureScrolls.Services;
using AdventureScrolls.View;
using AdventureScrolls.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdventureScrolls
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<IScribeService,ScribeService>();
            DependencyService.Register<IScrollCreatorService, ScrollCreatorService>();
            MainPage = new MainView();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
