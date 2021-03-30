using System;
using Xamarin.Forms;

namespace SPInfo.UserControls
{
    public class DialogContentPage:ContentPage
    {
        public delegate void PageDisappearHandler();

        public event PageDisappearHandler OnPageDisappear;

        public DialogContentPage()
        {

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            OnPageDisappear?.Invoke();
        }
    }
}
