using System;
using SPInfo.Views;
using Xamarin.Forms;

namespace SPInfo.Services
{
    public static class PageService
    {
        public static void GoToSettingsPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
        }

        public static void GoBack()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
