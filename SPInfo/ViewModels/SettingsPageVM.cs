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
            get { return settings.IP; }
            set
            {
                settings.IP = value;
                OnPropertyChanged("IP");
            }
        }

        public bool ShowFullTrackName
        {
            get { return settings.ShowFullTrackName; }
            set
            {
                settings.ShowFullTrackName = value;
                OnPropertyChanged("ShowFullTrackName");
            }
        }

        public bool ShowKey
        {
            get { return settings.ShowTrackKey; }
            set
            {
                settings.ShowTrackKey = value;
                OnPropertyChanged("ShowKey");
            }
        }

        private ISettings settings;

        public Command GoBack { get; set; }

        public SettingsPageVM()
        {
            settings = DependencyService.Resolve<ISettings>();

            GoBack = new Command(_ => PageService.GoBack());
        }
    }
}
