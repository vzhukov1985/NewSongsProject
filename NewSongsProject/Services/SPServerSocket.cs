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

        public delegate void MessageReceivedHandler(EzSocket socket, string header, string data);

        public event MessageReceivedHandler OnMessageReceived;

        public SPServerSocket(int port)
        {
            socket = new EzSocketListener(new EzEventsListener()
            {
                OnNewConnectionHandler = OnNewConnectionHandler,
                OnMessageReadHandler = OnSocketMessageReceived,
                OnConnectionClosedHandler = OnConnectionClosedHandler
            });

            activeClients = new List<SPClient>();

            socket.ListenAsync(55555);
        }

        public void SendMessageToClient(EzSocket socket, string header, string data)
        {
            if (string.IsNullOrEmpty(header))
                header = "";
            if (string.IsNullOrEmpty(data))
                data = "";

            socket.SendMessageAsync(Encoding.UTF8.GetBytes(header + "|" + data));
        }

        public void BroadcastMessage(string header, string data)
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
        }

        private void OnConnectionClosedHandler(EzSocket socket)
        {
            activeClients.RemoveAll(c => !c.Socket.Connected);
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
