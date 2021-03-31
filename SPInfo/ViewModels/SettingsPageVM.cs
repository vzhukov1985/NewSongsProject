using System;
using SPInfo.Models;
using SPInfo.Services;
using Xamarin.Forms;

namespace SPInfo.ViewModels
{
    public class SettingsPageVM : BaseVM
    {
        public string IP
        {
            get { return _settings.IP; }
            set
            {
                _settings.IP = value;

                if (initialIP == null)
                    initialIP = value;

                IsIPChanged = value != initialIP;

                OnPropertyChanged("IP");
            }
        }

        public bool ShowFullTrackName
        {
            get { return _settings.ShowFullTrackName; }
            set
            {
                _settings.ShowFullTrackName = value;
                OnPropertyChanged("ShowFullTrackName");
            }
        }

        public bool ShowKey
        {
            get { return _settings.ShowTrackKey; }
            set
            {
                _settings.ShowTrackKey = value;
                OnPropertyChanged("ShowKey");
            }
        }

        public bool ReceiveTrackList
        {
            get { return _settings.ReceiveTrackList; }
            set
            {
                _settings.ReceiveTrackList = value;
                OnPropertyChanged("ReceiveTrackList");
            }
        }

        public int TrackListShowTime
        {
            get => _settings.TrackListShowTime;
            set
            {
                _settings.TrackListShowTime = value;
                OnPropertyChanged("TrackListShowTime");
            }
        }

        public bool IsIPChanged { get; set; }

        private ISettings _settings;
        private string initialIP = null;
         

        public Command GoBack { get; set; }

        public SettingsPageVM()
        {
            IsIPChanged = false;
            _settings = DependencyService.Resolve<ISettings>();

            GoBack = new Command(_ => PageService.GoBack());
        }
    }
}
