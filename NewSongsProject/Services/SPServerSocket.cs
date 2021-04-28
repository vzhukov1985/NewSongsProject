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

        private readonly object locker = new object();

        public SPServerSocket(int port)
        {
            EzSocket.MaxMessageSize = int.MaxValue;

            socket = new EzSocketListener(new EzEventsListener()
            {
                OnNewConnectionHandler = OnNewConnectionHandler,
                OnMessageReadHandler = OnSocketMessageReceived,
                OnConnectionClosedHandler = OnConnectionClosedHandler,
                OnExceptionHandler = OnSocketException
            });

            activeClients = new List<SPClient>();

            socket.ListenAsync(port);
            pingTimer = new Timer(100);
            pingTimer.Elapsed += OnPingTimerElapsed;
            pingTimer.Start();
        }

        private ExceptionHandlerResponse OnSocketException(EzSocket socket, Exception e)
        {
            return ExceptionHandlerResponse.CloseSocket;
        }

        private void OnPingTimerElapsed(object sender, ElapsedEventArgs s)
        {
            BroadcastMessage("P");
        }

        public async void SendMessageToClient(EzSocket socket, string header, string data)
        {
            if (string.IsNullOrEmpty(header))
                header = "";
            if (string.IsNullOrEmpty(data))
                data = "";

            await socket.SendMessageAsync(Encoding.UTF8.GetBytes(header + "|" + data));
        }

        public void BroadcastMessage(string header, string data = "")
        {
            Task.Run(() =>
            {
                lock (locker)
                {
                    activeClients.RemoveAll(c => c.IsDead);
                    foreach (var client in activeClients)
                    {
                        client.Socket.SendMessage(Encoding.UTF8.GetBytes(header + "|" + data));
                    }
                }
            });
        }

        private void OnNewConnectionHandler(EzSocket socket)
        {
            socket.StartReadingMessages();
            activeClients.Add(new SPClient(socket));
            Debug.WriteLine("nc - " + activeClients.Count(c => !c.IsDead).ToString());
        }

        private void OnConnectionClosedHandler(EzSocket socket)
        {
            activeClients.FirstOrDefault(c => c.Socket == socket).IsDead = true;
            Debug.WriteLine("cc - " + activeClients.Count(c => !c.IsDead).ToString());
        }

        private void OnSocketMessageReceived(EzSocket socket, byte[] data)
        {
            string msg = Encoding.UTF8.GetString(data);
            string strHeader = msg.Substring(0, msg.IndexOf('|'));
            string strData = msg.Substring(msg.IndexOf('|') + 1);

            OnMessageReceived?.Invoke(socket, strHeader, strData);
        }
    }
}
