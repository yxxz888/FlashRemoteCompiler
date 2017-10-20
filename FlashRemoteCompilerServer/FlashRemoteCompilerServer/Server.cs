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

        private String historyPath = Path.Combine(Application.StartupPath, "history");
        private Dictionary<String, String> historyMap = new Dictionary<String, String>();

        public Server()
        {
            InitializeComponent();
        }


        private void Server_Load(object sender, EventArgs e)
        {
            //不规范做法，但是做工具偷个懒算了
            CheckForIllegalCrossThreadCalls = false;

            ConfigInfo.initConfig();
            initSocket();
            loadDateFiles();
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
            writeHistory(curCompileClient);
            finishCompile();
        }


        private void writeHistory(ClientObject client)
        {
            String today = (DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day) + ".txt";
            String path = Path.Combine(historyPath, today);

            String history = client.name + " " + DateTime.Now.ToString("HH:mm:ss") + ";";
            for(int i = 0;i < client.fileList.Length;i++)
            {
                history += client.fileList[i].Substring(ConfigInfo.assets.Length);
                if (i < client.fileList.Length - 1)
                    history += ",";
            }

            File.AppendAllLines(path,new String[] { history });

            loadDateFiles();
        }


        private void loadDateFiles()
        {
            cbDateList.Items.Clear();

            String[] files = Directory.GetFiles(historyPath, "*.txt", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return;

            Array.Reverse(files);
            for (int i = 0;i < files.Length;i++)
            {
                FileInfo info = new FileInfo(files[i]);
                cbDateList.Items.Add(info.Name);
            }

            cbDateList.SelectedIndex = 0;
        }


        private void loadDateNames()
        {
            historyMap.Clear();

            String path = Path.Combine(historyPath, cbDateList.SelectedItem.ToString());
            String[] lines = File.ReadAllLines(path);
            if (lines.Length == 0)
                return;

            for(int i = 0;i < lines.Length;i++)
            {
                String[] temp = lines[i].Split(new String[] { ";" },StringSplitOptions.RemoveEmptyEntries);
                historyMap.Add(temp[0], temp[1]);

                lbNameList.Items.Add(temp[0]);
            }

            if (historyMap.Count > 0)
                lbNameList.SelectedIndex = 0;
        }


        private void cbDateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDateNames();
        }


        private void lbNameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbFileList.Items.Clear();

            String selected = lbNameList.SelectedItem.ToString();
            String files = historyMap[selected];
            String[] fileList = files.Split(new String[] { "," },StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0;i < fileList.Length;i++)
            {
                lbFileList.Items.Add(fileList[i]);
            }
        }
    }
}
