using EzSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Models
{
    public class SPClient
    {
        public EzSocket Socket { get; set; }
        public bool IsDead { get; set; }

        public SPClient(EzSocket socket)
        {
            Socket = socket;
            IsDead = false;
        }
    }
}
