using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Models
{
    public class SPClient
    {
        public Socket Socket { get; set; }
        public DateTime LastPingTime { get; set; }

        public SPClient(Socket socket)
        {
            Socket = socket;
            CommandsProcessAsync();
        }

        private async void CommandsProcessAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (Socket.Available > 0)
                    {
                        byte[] msg = new byte[100];
                        Socket.Receive(msg);

                        int i = 0;
                        while (msg[i] != 0)
                        {
                            i++;
                        }
                        msg = msg.Take(i).ToArray();

                        if (msg.Length == 1 && msg[0] == 0xFF)
                        {
                            Socket.Send(new byte[] { 0xFF });
                            LastPingTime = DateTime.Now;
                        }
                    }
                }
            });
        }

    }
}
