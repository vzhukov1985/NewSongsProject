using EzSockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

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

        private EzSocket socket;
        private Timer connCheckTimer;
        private bool connecting;

        private DateTime lastPingTime;

        public SPClientSocket(string ip, int port)
        {
            Connected = false;
            EzSocket.MaxMessageSize = int.MaxValue;
            IP = ip;
            Port = port;
            Thread connThread = new Thread(Connect);
            connThread.Start();
            connCheckTimer = new Timer(500);
            connCheckTimer.Elapsed += ConnectionCheck;
            connCheckTimer.Start();
        }

        public void Connect()
        {
            socket = new EzSocket(IP, Port, new EzEventsListener()
            {
                OnMessageReadHandler = OnSocketMessageReceived,
                OnConnectionClosedHandler = OnSocketConnectionClosed
            });

            if (socket.Connected)
            {
                socket.StartReadingMessages();
                OnConnected?.Invoke();
                Connected = true;
            }

            connecting = false;
        }

        public void Disconnect()
        {
            connCheckTimer.Stop();
            if (socket != null)
            {
                socket.StopReadingMessages();
                socket.Close();
            }
        }

        private void ConnectionCheck(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - lastPingTime).TotalSeconds >= 2)
            {
                Disconnect();
                connCheckTimer.Start();
            }

            if ((socket == null && !connecting) || (!socket.Connected && !connecting))
            {
                connecting = true;

                Thread connThread = new Thread(Connect);
                connThread.Start();
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

            if (strHeader == "P")
                lastPingTime = DateTime.Now;

            OnMessageReceived?.Invoke(strHeader, strData);
        }

        private void OnSocketConnectionClosed(EzSocket socket)
        {
            Connected = false;
        }
    }
}
