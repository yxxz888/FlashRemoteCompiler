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
        public String name;
        public TcpClient client;
        public byte[] buffer;
        public String message;

        public ClientObject(TcpClient client)
        {
            this.client = client;
            resetBuffer();
        }


        public void resetBuffer()
        {
            buffer = new byte[128];
            message = "";
        }
    }
}
