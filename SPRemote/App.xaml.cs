using System;
using SPRemote.Models;
using SPRemote.ViewModels;
using SPRemote.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SPRemote
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.RegisterSingleton<ISettings>(new Settings());

            MainPage = new MainPage();
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
