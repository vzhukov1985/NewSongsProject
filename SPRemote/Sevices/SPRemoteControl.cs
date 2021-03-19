using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SPRemote.Sevices
{
    public class SPRemoteControl
    {
        private string _ip;
        public string IP
        {
            get => _ip;
            set
            {
                _ip = value;
                Connect();
            }
        }

        public bool Connected
        {
            get
            {
                if (socket != null && socket.Connected && pingStatus)
                    return true;
                return false;
            }
        }

        public delegate void MessageReceived(string message);
        public delegate void ConnectedFunc();
        public event MessageReceived OnMessageReceived;
        public event ConnectedFunc OnConnected;

        private Socket socket;

        private int port;
        private bool pingStatus;
        private Timer connectionCheckTimer;
        private Timer pingCheckTimer;

        public SPRemoteControl(string ip, int port)
        {
            this.port = port;
            _ip = ip;
            connectionCheckTimer = new Timer(500);
            connectionCheckTimer.Elapsed += CheckConnection;
            connectionCheckTimer.Start();

            pingCheckTimer = new Timer(1000);
            pingCheckTimer.Elapsed += PingTimeOut;            
        }

        private async void CheckConnection(Object source, ElapsedEventArgs e)
        {
            if (socket != null && socket.Connected)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        byte[] ping = new byte[1] { 0xFF };
                        socket.Send(ping);
                        pingCheckTimer.Stop();
                        pingCheckTimer.Start();
                    }
                    catch
                    {
                        return;
                    }
                });
            }
            else
            {
                Connect();
            }
        }

        private void PingTimeOut(Object source, ElapsedEventArgs e)
        {
            pingStatus = false;
        }

        public async void Connect()
        {
            await Task.Run(() =>
            {
                if (Connected)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    catch
                    {
                       
                    }
                }

                IPAddress serverIp;
                if (IPAddress.TryParse(_ip, out serverIp))
                {

                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        socket.Connect(new IPEndPoint(serverIp, port));
                        OnConnected?.Invoke();
                    }
                    catch
                    {
                        return;
                    }
                }
            });
        }

        public async void SendMessage(string message)
        {
            await Task.Run(() =>
            {
             /*   try
                {
                    byte[] msg = Encoding.ASCII.GetBytes(message);
                    socket.Send(msg);
                }
                catch
                {                
                    return;
                }*/
            });
        }

        public async void StartReceiving()
        {
            await Task.Run(() =>
            {
                
                while (true)
                {
                    try
                    {
                        if (socket != null && socket.Connected && socket.Available > 0)
                        {
                            byte[] msg = new byte[100];
                            socket.Receive(msg);

                            int i = 0;
                            while (msg[i] != 0)
                            {
                                i++;
                            }

                            msg = msg.Take(i).ToArray();

                            if (msg.Length == 1 && msg[0] == 0xFF)
                            {
                                pingStatus = true;
                            }
                            else
                            {
                                OnMessageReceived?.Invoke(Encoding.ASCII.GetString(msg));
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            });
        }
    }
}
