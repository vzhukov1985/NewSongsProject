using EzSockets;
using NewSongsProject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NewSongsProject.Services
{
    public class SPServerSocket
    {
        private EzSocketListener socket;
        private List<SPClient> activeClients;

        private Timer pingTimer;

        public delegate void MessageReceivedHandler(EzSocket socket, string header, string data);

        public event MessageReceivedHandler OnMessageReceived;

        public SPServerSocket(int port)
        {
            EzSocket.MaxMessageSize = int.MaxValue;

            socket = new EzSocketListener(new EzEventsListener()
            {
                OnNewConnectionHandler = OnNewConnectionHandler,
                OnMessageReadHandler = OnSocketMessageReceived,
                OnConnectionClosedHandler = OnConnectionClosedHandler
            });

            activeClients = new List<SPClient>();

            socket.ListenAsync(port);
            pingTimer = new Timer(100);
            pingTimer.Elapsed += OnPingTimerElapsed;
            pingTimer.Start();
        }

        private void OnPingTimerElapsed(object sender, ElapsedEventArgs s)
        {
            BroadcastMessage("P");
        }

        public void SendMessageToClient(EzSocket socket, string header, string data)
        {
            if (string.IsNullOrEmpty(header))
                header = "";
            if (string.IsNullOrEmpty(data))
                data = "";

            socket.SendMessageAsync(Encoding.UTF8.GetBytes(header + "|" + data));
        }

        public void BroadcastMessage(string header, string data = "")
        {
            foreach (var client in activeClients)
            {
                client.Socket.SendMessageAsync(Encoding.UTF8.GetBytes(header + "|" + data));
            }
        }

        private void OnNewConnectionHandler(EzSocket socket)
        {
            socket.StartReadingMessages();
            activeClients.Add(new SPClient(socket));
            Debug.WriteLine(activeClients.Count);
        }

        private void OnConnectionClosedHandler(EzSocket socket)
        {
            activeClients.RemoveAll(c => !c.Socket.Connected);
            Debug.WriteLine(activeClients.Count);
        }

        private void OnSocketMessageReceived(EzSocket socket, byte[] data)
        {
            string msg = Encoding.UTF8.GetString(data);
            string strHeader = msg.Substring(0, msg.IndexOf('|'));
            string strData = msg.Substring(msg.IndexOf('|')+1);

            OnMessageReceived?.Invoke(socket, strHeader, strData);
        }
    }
}
