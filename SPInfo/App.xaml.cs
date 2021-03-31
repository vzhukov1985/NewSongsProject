using System;
using SPInfo.Models;
using SPInfo.ViewModels;
using SPInfo.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SPInfo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DependencyService.RegisterSingleton<ISettings>(new Settings());
            DependencyService.RegisterSingleton<IGlobalStates>(new GlobalStates());

            MainPage = new NavigationPage(new MainPage());
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
