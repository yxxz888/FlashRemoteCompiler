using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlashRemoteCompilerServer
{
    class SocketServer
    {
        private String ip = "127.0.0.1";
        private int port = 14141;
        private TcpListener listener;

        public SocketServer()
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ip), port));
            listener.Start();
            listener.BeginAcceptTcpClient(onAcceptTcpClient,listener);
        }


        private void onAcceptTcpClient(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);

            ClientObject obj = new ClientObject(client);

            handleRead(obj);
        }


        private void handleRead(ClientObject obj)
        {
            obj.client.GetStream().BeginRead(obj.buffer, 0, obj.buffer.Length, onReadBack, obj);
        }


        private void onReadBack(IAsyncResult ar)
        {
            ClientObject obj = (ClientObject)ar.AsyncState;
            
        }


        private void onWriteData(IAsyncResult ar)
        {
            ((TcpClient)ar.AsyncState).GetStream().EndWrite(ar);
            Console.WriteLine("EndWrite");
        }
    }
}
