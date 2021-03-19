using NewSongsProject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Services
{
    public class SPServerSocket
    {

        private Socket socket;

        private List<SPClient> activeClients;

        public SPServerSocket(int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            activeClients = new List<SPClient>();

            /*try
            {*/
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Bind(endPoint);
                socket.Listen(10);

                ReceiveCommands();
/*            }
            catch
            {
                return;
            }*/
        }

        private async void ReceiveCommands()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    bool isSocketExist = false;
                    var newSocket = socket.Accept();
                    foreach (var client in activeClients)
                    {
                        if (client.Socket.Handle == newSocket.Handle)
                            isSocketExist = true;
                    }
                    if (!isSocketExist)
                        activeClients.Add(new SPClient(newSocket));
                    Debug.WriteLine(activeClients.Count);
                }
            });
        }
    }
}
