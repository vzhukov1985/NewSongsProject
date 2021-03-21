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

namespace SPRemote.ViewModels
{
    public class MainPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private string _ip;
        public string IP
        {
            get { return _ip; }
            set
            {
                _ip = value;
                Preferences.Set("IP", value);
                if (socket != null) socket.IP = value;
                OnPropertyChanged("IP");
            }
        }

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

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                ProcessSearch();
                OnPropertyChanged("SearchString");
            }
        }

        private List<string> _shownTracks;
        public List<string> ShownTracks
        {
            get { return _shownTracks; }
            set
            {
                _shownTracks = value;
                OnPropertyChanged("ShownTracks");
            }
        }

        private string _openedTrack;
        public string OpenedTrack
        {
            get { return _openedTrack; }
            set
            {
                _openedTrack = value;
                OnPropertyChanged("OpenedTrack");
            }
        }

        private string _selectedTrack;
        public string SelectedTrack
        {
            get { return _selectedTrack; }
            set
            {
                _selectedTrack = value;
                if (!string.IsNullOrEmpty(value))
                    SelectTrackDir(value);
                OnPropertyChanged("SelectedTrack");
            }
        }





        private SPClientSocket socket;

        private Timer connectionChecker;
        private List<string> allTracks;

        public MainPageVM()
        {
            ShownTracks = new List<string>();
            IP = Preferences.Get("IP", "192.168.1.1");
            socket = new SPClientSocket(IP, 55555);
            socket.OnMessageReceived += OnMessageReceived;
            socket.OnConnected += OnConnected;
            socket.StartReceiving();
            IsConnected = false;

            connectionChecker = new Timer(500);
            connectionChecker.Elapsed += CheckConnection;
            connectionChecker.Enabled = true;
        }



        private void CheckConnection(Object source, ElapsedEventArgs e)
        {
            if (socket.Connected)
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
            RequestAllTracksList();
        }


        private void ProcessSearch()
        {
            if (allTracks != null)
            {
                ShownTracks = string.IsNullOrEmpty(SearchString) ? allTracks : allTracks.FindAll(s => s.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }


        private void RequestAllTracksList()
        {
            socket.SendMessage("RequestTracksList");
        }

        private void SelectTrackDir(string itemName)
        {
            socket.SendMessage($"SelectTrackDir|{itemName}");
        }

        private void OnMessageReceived(string message)
        {
            if (message.StartsWith("TracksList"))
            {

                TrackListReceived(message.Substring(11));
            }

            OpenedTrack = message;
        }

        private void TrackListReceived(string tracks)
        {
            allTracks = tracks.Split('|').ToList();
            allTracks.Insert(0, "...");
            ProcessSearch();
        }

    }
}
