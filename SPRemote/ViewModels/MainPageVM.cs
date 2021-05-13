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
                Device.BeginInvokeOnMainThread(() => MovePlaylistItemUpCmd.ChangeCanExecute());
                Device.BeginInvokeOnMainThread(() => MovePlaylistItemDownCmd.ChangeCanExecute());
                Device.BeginInvokeOnMainThread(() => RemovePlaylistItemCmd.ChangeCanExecute());
                Device.BeginInvokeOnMainThread(() => ClearPlaylistCmd.ChangeCanExecute());
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

        private bool _awaitSendingPlaylist;
        public bool AwaitSendingPlaylist
        {
            get { return _awaitSendingPlaylist; }
            set
            {
                _awaitSendingPlaylist = value;
                OnPropertyChanged("AwaitSendingPlaylist");
                Device.BeginInvokeOnMainThread(() => CancelSendingPlaylistOnConnectCmd.ChangeCanExecute());
            }
        }

        private TrackListItem _selectedPlaylistItem;
        public TrackListItem SelectedPlaylistItem
        {
            get { return _selectedPlaylistItem; }
            set
            {
                _selectedPlaylistItem = value;
                OnPropertyChanged("SelectedPlaylistItem");
                if (value != null)
                {
                    ProcessSelectTrackListItem(value);
                }
                if (!selectPlaylistItemReceived)
                {
                    socket.SendMessage("PlaylistItemSelected", Playlist.IndexOf(value).ToString());
                }
                Device.BeginInvokeOnMainThread(() => MovePlaylistItemUpCmd.ChangeCanExecute());
                Device.BeginInvokeOnMainThread(() => MovePlaylistItemDownCmd.ChangeCanExecute());
                Device.BeginInvokeOnMainThread(() => RemovePlaylistItemCmd.ChangeCanExecute());
            }
        }





        private ISettings _settings;
        private SPClientSocket socket;
        private Timer tmrConnectionChecker;
        private string upDirPath;
        private bool selectPlaylistItemReceived = false;

        private List<TrackListItem> allTracksList;

        public Command UpDirCmd { get; set; }
        public Command ProcessSelectTrackListItemCmd { get; set; }
        public Command OpenNextTrackCmd { get; set; }
        public Command PlayStopCmd { get; set; }
        public Command AddTrackToPlaylistCmd { get; set; }
        public Command CancelSendingPlaylistOnConnectCmd { get; set; }
        public Command MovePlaylistItemUpCmd { get; set; }
        public Command MovePlaylistItemDownCmd { get; set; }
        public Command RemovePlaylistItemCmd { get; set; }
        public Command ClearPlaylistCmd { get; set; }

        public delegate void FocusPlaylist();

        public FocusPlaylist OnPlaylistFocusNeeded { get; set; }

        public MainPageVM()
        {
            _settings = DependencyService.Resolve<ISettings>();

            DeviceDisplay.KeepScreenOn = true;

            SearchText = "";

            TrackList = new List<TrackListItem>();
            Playlist = new List<TrackListItem>();

            ProcessSelectTrackListItemCmd = new Command<TrackListItem>(t => ProcessSelectTrackListItem(t));
            UpDirCmd = new Command(_ => socket.SendMessage("RequestDirContents", upDirPath));
            OpenNextTrackCmd = new Command(_ => socket.SendMessage("OpenNextTrack"), _ => CanOpenNextTrack);
            PlayStopCmd = new Command(_ => socket.SendMessage("PlayStop"), _ => PlayState != PlayState.None);
            AddTrackToPlaylistCmd = new Command<TrackListItem>(t => AddTrackToPlaylistAsync(t));
            CancelSendingPlaylistOnConnectCmd = new Command(_ => AwaitSendingPlaylist = false, _ => AwaitSendingPlaylist == true);
            MovePlaylistItemUpCmd = new Command(_ => { MovePlaylistItemUp(); SendPlaylist(); }, _ => SelectedPlaylistItem != null && Playlist.IndexOf(SelectedPlaylistItem) > 0);
            MovePlaylistItemDownCmd = new Command(_ => { MovePlaylistItemDown(); SendPlaylist(); }, _ => SelectedPlaylistItem != null && Playlist.IndexOf(SelectedPlaylistItem) < Playlist.Count - 1);
            RemovePlaylistItemCmd = new Command(_ => { Playlist.Remove(SelectedPlaylistItem); Playlist = Playlist.ToList(); SelectedPlaylistItem = null; SendPlaylist(); }, _ => SelectedPlaylistItem != null);
            ClearPlaylistCmd = new Command(_ => { Playlist = new List<TrackListItem>(); SendPlaylist(); }, _ => Playlist != null && Playlist.Count > 0);


            CreateConnection();
        }

        private void MovePlaylistItemUp()
        {
            var item = SelectedPlaylistItem;
            var id = Playlist.IndexOf(SelectedPlaylistItem);
            Playlist.Remove(item);
            Playlist.Insert(id - 1, item);
            Playlist = Playlist.ToList();
            SelectedPlaylistItem = null;
            SelectedPlaylistItem = Playlist[id-1];
        }

        private void MovePlaylistItemDown()
        {
            var item = SelectedPlaylistItem;
            var id = Playlist.IndexOf(SelectedPlaylistItem);
            Playlist.Remove(item);
            Playlist.Insert(id + 1, item);
            Playlist = Playlist.ToList();
            SelectedPlaylistItem = null;
            SelectedPlaylistItem = Playlist[id+1];
        }


        private async void AddTrackToPlaylistAsync(TrackListItem track)
        {
            await Task.Run(() =>
            {
            if (Playlist.FirstOrDefault(t => t.FullPath == track.FullPath) == null)
                {
                    Playlist.Add(track);
                    Playlist = Playlist.ToList();
                    SendPlaylist();
                    SelectedPlaylistItem = track;
                }
            });
        }

        private void SendPlaylist()
        {
            if (IsConnected)
            {
                socket.SendMessage("Playlist", JsonConvert.SerializeObject(Playlist));
            }
            else
            {
                AwaitSendingPlaylist = true;
            }
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
            socket.SendMessage("ClientConnected", AwaitSendingPlaylist ? "1" : "0");
            if (AwaitSendingPlaylist)
            {
                socket.SendMessage("Playlist", JsonConvert.SerializeObject(Playlist));
                AwaitSendingPlaylist = false;
            }
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
                case "PlaylistItemSelected":
                    SelectPlaylistItem(int.Parse(data));
                    break;
                default:
                    break;
            }
        }

        private void SelectPlaylistItem(int index)
        {
            if (index != -1 && Playlist.Count > index)
            {
                selectPlaylistItemReceived = true;
                SelectedPlaylistItem = Playlist[index];
                selectPlaylistItemReceived = false;
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
