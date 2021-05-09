using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using Common.Models;
using SPRemote.Models;
using Xamarin.Forms;
using Common.Services;
using Newtonsoft.Json;
using Common.DTOs;

namespace SPRemote.ViewModels
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

        public string IP
        {
            get { return _settings.IP; }
            set
            {
                _settings.IP = value;
                socket.Disconnect();
                CreateConnection();
                OnPropertyChanged("IP");
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                ProcessSearch();
                OnPropertyChanged("SearchText");
            }
        }

        private List<TrackListItem> _trackList;
        public List<TrackListItem> TrackList
        {
            get { return _trackList; }
            set
            {
                _trackList = value;
                OnPropertyChanged("TrackList");
            }
        }

        private bool _canUpDir;
        public bool CanUpDir
        {
            get { return _canUpDir; }
            set
            {
                _canUpDir = value;
                OnPropertyChanged("CanUpDir");
            }
        }



        private TrackListItem _currentTrack;
        public TrackListItem CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                _currentTrack = value;
                if (value == null || value.Caption == "Нет")
                {
                    PlayState = PlayState.None;
                }
                else
                {
                    if (PlayState == PlayState.None)
                        socket.SendMessage("RequestPlayStatus");
                }
                OnPropertyChanged("CurrentTrack");
            }
        }

        private TrackListItem _nextTrack;
        public TrackListItem NextTrack
        {
            get { return _nextTrack; }
            set
            {
                _nextTrack = value;
                if (value == null || value.IsDirectory)
                {
                    value.Caption = "Нет";
                    CanOpenNextTrack = false;
                }
                else
                {
                    CanOpenNextTrack = true;
                }

                OnPropertyChanged("NextTrack");
            }
        }

        private List<TrackListItem> _playlist;
        public List<TrackListItem> Playlist
        {
            get { return _playlist; }
            set
            {
                _playlist = value;
                OnPropertyChanged("Playlist");
            }
        }

        private bool _canOpenNextTrack;
        public bool CanOpenNextTrack
        {
            get { return _canOpenNextTrack; }
            set
            {
                _canOpenNextTrack = value;
                OnPropertyChanged("CanOpenNextTrack");
            }
        }

        private PlayState _playState;
        public PlayState PlayState
        {
            get { return _playState; }
            set
            {
                _playState = value;
                if (CurrentTrack == null || CurrentTrack.Caption == "Нет")
                {
                    _playState = PlayState.None;
                }
                OnPropertyChanged("PlayState");
            }
        }




        private ISettings _settings;
        private SPClientSocket socket;
        private Timer tmrConnectionChecker;
        private string upDirPath;

        private List<TrackListItem> allTracksList;

        public Command UpDirCmd { get; set; }
        public Command ProcessSelectTrackListItemCmd { get; set; }
        public Command OpenNextTrackCmd { get; set; }
        public Command PlayStopCmd { get; set; }

        public MainPageVM()
        {
            _settings = DependencyService.Resolve<ISettings>();

            DeviceDisplay.KeepScreenOn = true;

            SearchText = "";

            ProcessSelectTrackListItemCmd = new Command<TrackListItem>(t => ProcessSelectTrackListItem(t));
            UpDirCmd = new Command(_ => socket.SendMessage("RequestDirContents", upDirPath));
            OpenNextTrackCmd = new Command(_ => socket.SendMessage("OpenNextTrack"), _ => CanOpenNextTrack);
            PlayStopCmd = new Command(_ => socket.SendMessage("PlayStop"), _ => PlayState != PlayState.None);

            CreateConnection();
        }

        private void CreateConnection()
        {
            IPAddress ipCheck;
            if (!IPAddress.TryParse(_settings.IP, out ipCheck))
                return;

            socket = new SPClientSocket(_settings.IP, 55556);
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


        private void ProcessSelectTrackListItem(TrackListItem trackListItem)
        {
            if (trackListItem.IsDirectory)
            {
                socket.SendMessage("RequestDirContents", trackListItem.FullPath);
            }
            else
            {
                socket.SendMessage("SelectTrack", JsonConvert.SerializeObject(trackListItem));
            }

            SearchText = "";
            ProcessSearch();
        }

        private void OnConnected()
        {
            socket.SendMessage("ClientConnected");
        }

        private async void OnMessageReceived(string header, string data)
        {
            switch (header)
            {
                case "DirContents":
                    DirContentsReceived(data);
                    break;
                case "CurrentTrack":
                    await Task.Run(() => CurrentTrack = JsonConvert.DeserializeObject<TrackListItem>(data));
                    break;
                case "NextTrack":
                    await Task.Run(() => NextTrack = JsonConvert.DeserializeObject<TrackListItem>(data));
                    break;
                case "Playlist":
                    await Task.Run(() => Playlist = JsonConvert.DeserializeObject<List<TrackListItem>>(data));
                    break;
                case "PlayStatus":
                    await Task.Run(() => PlayState = data == "1" ? PlayState.Playing : PlayState.Stopped);
                    break;
                default:
                    break;
            }
        }

        private async void DirContentsReceived(string data)
        {
            await Task.Run(() =>
            {
                var dirContents = JsonConvert.DeserializeObject<DirContentsDto>(data);
                allTracksList = dirContents.TrackListItems;
                CanUpDir = !dirContents.IsTopDir;
                upDirPath = dirContents.UpDirPath;

                SearchText = "";
                ProcessSearch();
            });
        }

        private void ProcessSearch()
        {
            if (allTracksList != null)
            {
                var filteredTrackList = allTracksList.Where(t => t.Caption.IndexOf(SearchText != null ? SearchText : "", StringComparison.OrdinalIgnoreCase) >= 0 || t.IsDirectory == true).ToList();
                Device.BeginInvokeOnMainThread(() => TrackList = filteredTrackList);
            }
        }
    }
}
