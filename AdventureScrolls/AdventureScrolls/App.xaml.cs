﻿using AdventureScrolls.Model;
using AdventureScrolls.Services;
using AdventureScrolls.View;
using AdventureScrolls.ViewModel;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Plugin.Popups;
using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace AdventureScrolls
{
    public partial class App
    {
        public App() : this(null) { }
        public App(IPlatformInitializer initializer) : base(initializer) { }
        protected override async void OnInitialized()
        {
            InitializeComponent();
            //This line prevent Editor to fill whole screen
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().
                UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            //DI
            DependencyService.Register<IScribeService, ScribeService>();
            DependencyService.Register<IGoogleUserAuthenticationService, GoogleUserAuthenticationService>();
            DependencyService.Register<IGoogleDriveDataService, GoogleDriveDataService>();

            //Navigation
            await NavigationService.NavigateAsync("MainView");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainView>();
            containerRegistry.RegisterForNavigation<WriteAdventureView>();
            containerRegistry.RegisterForNavigation<SettingsView>();
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();
            containerRegistry.RegisterDialog<MoodPopUpView, MoodPopUpViewModel>();
        }
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => 
            {
                var viewName = viewType.FullName.Replace(".View.", ".ViewModel.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}Model, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }

        protected override void OnResume()
        {
            this.PopupPluginOnResume();
        }

        protected override void OnSleep()
        {
            this.PopupPluginOnSleep();
        }
    }
    
}
