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
        public delegate void MessageReceivedHandler(string header, string data);
        public delegate void ConnectedHandler();

        public event MessageReceivedHandler OnMessageReceived;
        public event ConnectedHandler OnConnected;    


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
            connCheckTimer.Elapsed += ConnectionCheck;
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
                        OnMessageReadHandler = OnSocketMessageReceived,
                        OnNewConnectionHandler = OnSocketConnected,
                        OnConnectionClosedHandler = OnSocketConnectionClosed
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

        private void ConnectionCheck(object sender, ElapsedEventArgs e)
        {
            if (!socket.Connected && !connecting)
            {
                Connect();
            }
        }

        public void SendMessage(string header, string data = "")
        {
            socket.SendMessageAsync(header + '|' + data);
        }

        private void OnSocketMessageReceived(EzSocket socket, byte[] data)
        {
            Connected = true;

            string msg = Encoding.UTF8.GetString(data);
            string strHeader = msg.Substring(0, msg.IndexOf('|'));
            string strData = msg.Substring(msg.IndexOf('|') + 1);

            OnMessageReceived?.Invoke(strHeader, strData);
        }

        private void OnSocketConnected(EzSocket socket)
        {
            OnConnected?.Invoke();
        }

        private void OnSocketConnectionClosed(EzSocket socket)
        {
            Connected = false;
        }
    }
}
