using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SPInfo.Models;
using SPInfo.Services;
using SPInfo.UserControls;
using Xamarin.Forms;
using Timer = System.Timers.Timer;

namespace SPInfo.Views
{
    public partial class TrackListPage : DialogContentPage
    {
        private IGlobalStates _globalStates;
        private ISettings _settings;

        private Timer showTimer;

        public TrackListPage()
        {
            InitializeComponent();
            _globalStates = DependencyService.Resolve<IGlobalStates>();
            _settings = DependencyService.Resolve<ISettings>();

            showTimer = new Timer(_settings.TrackListShowTime * 1000);
            showTimer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            PageService.GoBack();
        }

        protected override void OnAppearing()
        {
            _globalStates.IsTrackListPageShowing = true;
            showTimer.Start();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            showTimer.Stop();
            base.OnDisappearing();
            _globalStates.IsTrackListPageShowing = false;
        }
    }
}
