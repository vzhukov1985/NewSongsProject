﻿using System;
using Xamarin.Essentials;

namespace SPInfo.Models
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
        public bool ShowFullTrackName
        {
            get { return Preferences.Get("ShowFullTrackName", "False") == "True" ? true : false; }
            set
            {
                Preferences.Set("ShowFullTrackName", value ? "True" : "False");
            }
        }

        public bool ShowTrackKey
        {
            get { return Preferences.Get("ShowTrackKey", "True") == "True" ? true : false; }
            set
            {
                Preferences.Set("ShowTrackKey", value ? "True" : "False");
            }
        }

        public Settings()
        {
        }
    }
}
