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
using System.Collections.Generic;

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

        private TrackListItem _currentTrackInfo;
        public TrackListItem CurrentTrackInfo
        {
            get { return _currentTrackInfo; }
            set
            {
                _currentTrackInfo = value;
                OnPropertyChanged("CurrentTrackInfo");
            }
        }

        private TrackListItem _nextTrackInfo;
        public TrackListItem NextTrackInfo
        {
            get { return _nextTrackInfo; }
            set
            {
                _nextTrackInfo = value;
                OnPropertyChanged("NextTrackInfo");
            }
        }






        private SPClientSocket socket;
        private Timer tmrConnectionChecker;

        private ISettings _settings;
        private IGlobalStates _globalStates;


        public Command ShowSettingsCmd { get; set; }


        public MainPageVM()
        {
            _settings = DependencyService.Resolve<ISettings>();
            _globalStates = DependencyService.Resolve<IGlobalStates>();
            CurrentTrack = "Нет подключения";
            NextTrack = "Нет подключения";

            DeviceDisplay.KeepScreenOn = true;

            ShowSettingsCmd = new Command(_ => ShowSettings());

            CreateConnection();
        }

        private async void ShowSettings()
        {
            if (await PageService.GoToSettingsPage())
            {
                socket.Disconnect();
                CreateConnection();
            }

            CurrentTrack = RecreateTrackName(CurrentTrackInfo);
            NextTrack = RecreateTrackName(NextTrackInfo);
        }

        private void CreateConnection()
        {
            IPAddress ipCheck;
            if (!IPAddress.TryParse(_settings.IP, out ipCheck))
                return;

            socket = new SPClientSocket(_settings.IP, 55555);
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

        private void OnConnected()
        {
            socket.SendMessage("ClientConnected");
        }

        private async void OnMessageReceived(string header, string data)
        {
            switch (header)
            {
                case "CurrentTrack":
                    CurrentTrackInfo = JsonConvert.DeserializeObject<TrackListItem>(data);
                    if (CurrentTrackInfo.Caption != "Нет")
                        CurrentTrack = RecreateTrackName(CurrentTrackInfo);
                    break;
                case "NextTrack":
                    NextTrackInfo = JsonConvert.DeserializeObject<TrackListItem>(data);
                    if (NextTrackInfo.Caption != "Нет" && !NextTrackInfo.IsDirectory)
                        NextTrack = RecreateTrackName(NextTrackInfo);
                    break;
                case "SetTime":
                    SetTime = TimeSpan.Parse(data);
                    break;
                case "TrackList":
                    if (_settings.ReceiveTrackList)
                    {
                        if (_globalStates.IsTrackListPageShowing)
                            PageService.GoBack();
                        _globalStates.IsTrackListPageShowing = true;

                        var selectedTrack = await PageService.ShowTrackListPage(JsonConvert.DeserializeObject<List<TrackListItem>>(data));
                        if (selectedTrack != null)
                            socket.SendMessage("SelectTrack", JsonConvert.SerializeObject(selectedTrack));
                    }
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
            if (_settings.ShowFullTrackName && !string.IsNullOrEmpty(track.FullName))
            {
                result = track.FullName;
            }
            else
            {
                result = track.Caption;
            }

            if (_settings.ShowTrackKey && !string.IsNullOrEmpty(track.Key))
            {
                result += $" ({track.Key})";
            }

            return result;
        }
    }
}
