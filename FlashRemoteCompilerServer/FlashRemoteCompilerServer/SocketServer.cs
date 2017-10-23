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
        private int port = 44444;
        private TcpListener listener;

        private String separator = "->";

        public delegate void FileListReceiveCallback(ClientObject obj);

        private FileListReceiveCallback onFileListReceive;

        public SocketServer(FileListReceiveCallback callback)
        {

            listener = new TcpListener(IPAddress.Any,port);
            listener.Start();
            listener.BeginAcceptTcpClient(onAcceptTcpClient,null);

            onFileListReceive += callback;
        }


        public void dispose()
        {
            listener.Stop();
        }


        private void onAcceptTcpClient(IAsyncResult ar)
        {
            try
            {
                TcpClient client = listener.EndAcceptTcpClient(ar);
                listener.BeginAcceptTcpClient(onAcceptTcpClient, null);

                ClientObject obj = new ClientObject(client);

                handleRead(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                listener.Stop();
            }
        }


        private void handleRead(ClientObject obj)
        {
            obj.client.GetStream().BeginRead(obj.buffer, 0, obj.buffer.Length, onReadBack, obj);
        }


        private void onReadBack(IAsyncResult ar)
        {
            ClientObject obj = (ClientObject)ar.AsyncState;
            try
            {
                int len = obj.client.GetStream().EndRead(ar);
                String temp = Encoding.Default.GetString(obj.buffer).Substring(0,len);
                showLog(temp);
                obj.message += temp;
                obj.resetBuffer();
                if (obj.client.GetStream().DataAvailable == false)//读完了
                    handleMessage(obj);
                handleRead(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                obj.client.Close();
            }
        }


        private void handleMessage(ClientObject obj)
        {
            String[] temp = obj.message.Split(new String[] { separator },StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length != 2)
                return;

            String cmd = temp[0].Trim();
            String msg = temp[1].Trim();

            if (cmd == "name")
                obj.name = msg;
            else if(cmd == "list")
            {
                if (obj.isFinished())
                {
                    String[] list = msg.Split(',');
                    obj.fileList = list;
                    onFileListReceive(obj);
                }
            }

            obj.clearMessage();           
        }


        public void sendMessage(ClientObject client, String message)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(message);
                client.client.GetStream().BeginWrite(buffer, 0, buffer.Length, onWriteData, client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


        private void onWriteData(IAsyncResult ar)
        {
            try
            {
                ClientObject client = ar.AsyncState as ClientObject;
                client.client.GetStream().EndWrite(ar);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


        private void showLog(String msg)
        {
            Console.WriteLine(msg + "\n");
        }
    }
}
