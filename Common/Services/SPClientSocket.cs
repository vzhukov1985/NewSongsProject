using EzSockets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Common.Services
{

    public class SPClientSocket
    {
        public bool Connected { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        private readonly object connLocker = new object();

        private EzSocket socket;
        private Timer connCheckTimer;
        private bool connecting;


        public SPClientSocket(string ip, int port)
        {
            IP = ip;
            Port = port;
            connCheckTimer = new Timer(100);
            connCheckTimer.Elapsed += connCheck;
            connCheckTimer.Start();
        }

        public async void Connect()
        {

            await Task.Run(() =>
            {
                lock (connLocker)
                {
                    connecting = true;

                    socket = new EzSocket(IP, Port, new EzEventsListener()
                    {
                        OnMessageReadHandler = OnMessageReceived,
                        OnConnectionClosedHandler = OnConnectionClosed
                    });

                    if (socket.Connected)
                    {
                        socket.StartReadingMessages();
                        Connected = true;
                    }

                    connecting = false;
                }
            });
        }

        public void connCheck(object sender, ElapsedEventArgs e)
        {
            if (!socket.Connected && !connecting)
            {
                Connect();
            }
        }

        private void OnMessageReceived(EzSocket socket, byte[] data)
        {
            Connected = true;
        }

        private void OnConnectionClosed(EzSocket socket)
        {
            Connected = false;
        }
    }
}
