using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlashRemoteCompilerServer
{
    class ClientObject
    {
        public TcpClient client;
        public byte[] buffer;
        public String message;

        public ClientObject(TcpClient client)
        {
            this.client = client;
            buffer = new byte[1024];
            message = "";
        }
    }
}
