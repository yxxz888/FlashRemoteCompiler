using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashRemoteCompilerServer
{
    class SocketServer
    {
        private Label txtMsg;

        private String ip = "127.0.0.1";
        private int port = 14141;
        private TcpListener listener;

        public SocketServer()
        {

            listener = new TcpListener(IPAddress.Any,port);
            listener.Start();
            listener.BeginAcceptTcpClient(onAcceptTcpClient,null);
        }


        public void sendMessage(TcpClient client,String message)
        {
            byte[] buffer = Encoding.Default.GetBytes(message);
            client.GetStream().BeginWrite(buffer, 0, buffer.Length, onWriteData, client);
        }


        private void onAcceptTcpClient(IAsyncResult ar)
        {
            TcpClient client = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(onAcceptTcpClient, null);

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
            int len = obj.client.GetStream().EndRead(ar);
            String temp = Encoding.Default.GetString(obj.buffer);
            showLog(temp);
            obj.message += temp;
            if (obj.client.GetStream().DataAvailable == false)//读完了
            {
                showLog("收到消息：" + obj.message);
                sendMessage(obj.client, "服务端已收到文件！");
                obj.resetBuffer();
            }             
            handleRead(obj);
        }


        private void onWriteData(IAsyncResult ar)
        {
            ((TcpClient)ar.AsyncState).GetStream().EndWrite(ar);
        }


        private void showLog(String msg)
        {
            Console.WriteLine(msg + "\n");
        }
    }
}
