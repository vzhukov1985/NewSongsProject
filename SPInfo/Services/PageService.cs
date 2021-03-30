using System;
using System.Threading;
using System.Threading.Tasks;
using SPInfo.ViewModels;
using SPInfo.Views;
using Xamarin.Forms;

namespace SPInfo.Services
{
    public static class PageService
    {
        public static async Task<bool> GoToSettingsPage()
        {
            var waiter = new EventWaitHandle(false, EventResetMode.AutoReset);

            var vm = new SettingsPageVM();
            var page = new SettingsPage() { BindingContext = vm };
            page.OnPageDisappear += () => waiter.Set();

            await Application.Current.MainPage.Navigation.PushAsync(page);

            await Task.Run(() => waiter.WaitOne());

            return vm.IsIPChanged;
        }

        public static void GoBack()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
