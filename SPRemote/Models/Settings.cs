using System;
using Xamarin.Essentials;

namespace SPRemote.Models
{
    public class Settings: ISettings
    {
        public string IP
        {
            get { return Preferences.Get("IP", "192.168.1.1"); }
            set
            {
                Preferences.Set("IP", value);
            }
        }

        public Settings()
        {
        }
    }
}
