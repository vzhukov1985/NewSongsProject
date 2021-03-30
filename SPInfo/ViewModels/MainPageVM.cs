using System;
using System.Timers;
using Common.Services;
using Common.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xamarin.Forms;
using SPInfo.Services;
using SPInfo.Models;
using System.Net;
using Xamarin.Essentials;

namespace SPInfo.ViewModels
{
    public class MainPageVM : BaseVM
    {
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        private string _currentTrack;
        public string CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _currentTrack = value;
                }
                else
                {
                    _currentTrack = "Нет";
                }

                OnPropertyChanged("CurrentTrack");
            }
        }

        private string _nextTrack;
        public string NextTrack
        {
            get { return _nextTrack; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _nextTrack = value;
                }
                else
                {
                    _nextTrack = "Нет";
                }
                OnPropertyChanged("NextTrack");
            }
        }

        private TimeSpan _setTime;
        public TimeSpan SetTime
        {
            get { return _setTime; }
            set
            {
                _setTime = value;
                OnPropertyChanged("SetTime");
            }
        }





        private SPClientSocket socket;
        private Timer tmrConnectionChecker;
        private ISettings settings;
        private TrackListItem currentTrackInfo;
        private TrackListItem nextTrackInfo;

        public Command ShowSettingsCmd { get; set; }


        public MainPageVM()
        {
            settings = DependencyService.Resolve<ISettings>();
            DeviceDisplay.KeepScreenOn = true;

            ShowSettingsCmd = new Command(_ => ShowSettings());

            ConnectAsync();
        }

        private async void ShowSettings()
        {
            if (await PageService.GoToSettingsPage())
            {
                ConnectAsync();
            }

            CurrentTrack = RecreateTrackName(currentTrackInfo);
            NextTrack = RecreateTrackName(nextTrackInfo);
        }

        private async void ConnectAsync()
        {
            IPAddress ipCheck;
            if (!IPAddress.TryParse(settings.IP, out ipCheck))
                return;

            if (socket != null && socket.Connected)
                await socket.Disconnect();

            socket = new SPClientSocket(settings.IP, 55555);
            socket.Connect();
            socket.OnMessageReceived += OnMessageReceived;
            socket.OnConnected += OnConnected;

            tmrConnectionChecker = new Timer(500);
            tmrConnectionChecker.Elapsed += OnConnectionCheckerElapsed;
            tmrConnectionChecker.Start();
        }



        private void OnConnectionCheckerElapsed(object sender, ElapsedEventArgs e)
        {
            if (socket != null && socket.Connected)
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        }

        private async void OnConnected()
        {
            await Task.Run(() =>
            {
                while (!socket.Connected)
                {
                }
                socket.SendMessage("ClientConnected");
            });

        }

        private void OnMessageReceived(string header, string data)
        {
            switch (header)
            {
                case "CurrentTrack":
                    currentTrackInfo = JsonConvert.DeserializeObject<TrackListItem>(data);
                    CurrentTrack = RecreateTrackName(currentTrackInfo);
                    break;
                case "NextTrack":
                    nextTrackInfo = JsonConvert.DeserializeObject<TrackListItem>(data);
                    NextTrack = RecreateTrackName(nextTrackInfo);
                    break;
                case "SetTime":
                    SetTime = TimeSpan.Parse(data);
                    break;
                default:
                    break;
            }
        }

        private string RecreateTrackName(TrackListItem track)
        {
            if (track == null)
                return "Нет";

            string result;
            if (settings.ShowFullTrackName && !string.IsNullOrEmpty(track.FullName))
            {
                result = track.FullName;
            }
            else
            {
                result = track.Caption;
            }

            if (settings.ShowTrackKey && !string.IsNullOrEmpty(track.Key))
            {
                result += $" ({track.Key})";
            }

            return result;
        }
    }
}
