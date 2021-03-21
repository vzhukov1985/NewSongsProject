using System;
using System.Timers;
using Common.Services;

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

        public SPClientSocket Socket { get; set; }
        private Timer tmrConnectionChecker;

        public MainPageVM()
        {
            Socket = new SPClientSocket("10.211.55.4", 55555);
            Socket.Connect();
            tmrConnectionChecker = new Timer(500);
            tmrConnectionChecker.Elapsed += OnConnectionCheckerElapsed;
            tmrConnectionChecker.Start();            
        }

        private void OnConnectionCheckerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Socket.Connected)
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        }
    }
}
