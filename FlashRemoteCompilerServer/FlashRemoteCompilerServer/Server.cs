using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashRemoteCompilerServer
{
    public partial class Server : Form
    {
        private SocketServer server;

        private List<ClientObject> compileList = new List<ClientObject>();

        private ClientObject curCompileClient;
        private Boolean isCompiling = false;

        private CompileUtil util = new CompileUtil();

        public Server()
        {
            InitializeComponent();
        }


        private void Server_Load(object sender, EventArgs e)
        {
            initSocket();
        }


        private void initSocket()
        {
            server = new SocketServer(onFileListReceive);
        }


        private void onFileListReceive(ClientObject client)
        {
            compileList.Add(client);

            if (isCompiling)
                server.sendMessage(client, "进入编译队列……");
            else
                handleCompile();
        }


        private void handleCompile()
        {
            if (compileList.Count <= 0)
                return ;

            curCompileClient = compileList[0];
            server.sendMessage(curCompileClient, "开始编译……");

            List<FlaItem> itemList = new List<FlaItem>();
            for (int i = 0; i < curCompileClient.flaList.Length; i++)
            {
                itemList.Add(new FlaItem(curCompileClient.flaList[i]));
            }
            util.compileFla(itemList.ToArray());
        }


        


        private void button1_Click(object sender, EventArgs e)
        {
            TestClient form = new TestClient();
            form.Show();
        }
    }


    class FlaItem
    {
        public String dis;
        public String assetsPathForJsfl;//以assets/(不含assets/)为根目录的路径，用/分隔符
        public String fullPath;//全路径
        public String assetsPath;//以assets/(不含assets/)为根目录的路径
        public FlaItem()
        {
        }

        public FlaItem(String fullPath)
        {
            FileInfo info = new FileInfo(fullPath);
            this.dis = info.Name;
            this.fullPath = fullPath;

            String assetsName = ConfigInfo.assets;
            int index = fullPath.IndexOf(assetsName);
            if (index != -1)
            {
                assetsPath = fullPath.Substring(index + assetsName.Length);
                if (assetsPath.IndexOf("\\") == 0)
                {
                    assetsPath = assetsPath.Substring(1);//去掉前面的\\符号，这样才能成功合并路径
                }
            }
            else
            {
                assetsPath = dis;
            }

            assetsPathForJsfl = assetsPath.Replace("\\", "/");
        }

        public override string ToString()
        {
            return dis.ToString();
        }
    }
}
