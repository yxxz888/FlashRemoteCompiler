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

        private CompileUtil compileUtil = new CompileUtil();
        private UploadUtil uploadUtil = new UploadUtil();

        public Server()
        {
            InitializeComponent();
        }


        private void Server_Load(object sender, EventArgs e)
        {
            ConfigInfo.initConfig();
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
                server.sendMessage(client, "已有编译任务，进入等待队列......");
            else
                handleCompile();
        }


        private void handleCompile()
        {
            if (compileList.Count <= 0)
            {
                isCompiling = false;
                return;
            }

            curCompileClient = compileList[0];
            compileList.RemoveAt(0);
            server.sendMessage(curCompileClient, "开始编译......");

            List<FlaItem> itemList = new List<FlaItem>();
            for (int i = 0; i < curCompileClient.flaList.Length; i++)
            {
                itemList.Add(new FlaItem(curCompileClient.flaList[i]));
            }
            if (itemList.Count > 0)
                compileUtil.compileFla(itemList.ToArray(), onCompileEnd);
            else
                handleUpload();
        }


        private void onCompileEnd()
        {
            String logPath = ConfigInfo.getCSharpPath(ConfigInfo.compileLog);

            if (File.Exists(logPath) == false)//因为jsfl每次编译前都会删除log文件，如果log不存在，则编译无错误
            {
                handleUpload();
            }
            else
            {
                String compileResult = File.ReadAllText(logPath);
                server.sendMessage(curCompileClient, compileResult + "\r\n出现异常，编译终止。");
                finishCompile();
            }
        }


        private void finishCompile()
        {
            curCompileClient.finishCompile();
            handleCompile();
        }


        private void handleUpload()
        {
            server.sendMessage(curCompileClient, "\r\n编译成功，正在上传......");
            uploadUtil.uploadFile(curCompileClient.fileList, onUploadFinished);
        }


        private void onUploadFinished(Boolean isSuc,String message)
        {
            server.sendMessage(curCompileClient, message + "\r\n编译任务完成。");
            finishCompile();
        }


        private void loadDateFile()
        {
            String path = Path.Combine(Application.StartupPath, "history");
        }
    }
}
