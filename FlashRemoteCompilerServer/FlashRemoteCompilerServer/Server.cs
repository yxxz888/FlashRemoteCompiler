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

        private List<ClientObject> clientList = new List<ClientObject>();

        private ClientObject curCompileClient;
        private Boolean isCompiling = false;

        private CompileUtil compileUtil = new CompileUtil();
        private UploadUtil uploadUtil = new UploadUtil();

        private String historyPath = Path.Combine(Application.StartupPath, "history");
        private Dictionary<String, String> historyMap = new Dictionary<String, String>();

        private Boolean isClosed = false;

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
            clientList.Add(client);

            if (isCompiling)
                server.sendMessage(client, "已有编译任务，进入等待队列......");
            else
                handleCompile();
        }


        private void handleGetLastestFiles()
        {
            if (isClosed)
                return;


        }


        private void handleCompile()
        {
            if (clientList.Count <= 0)
            {
                isCompiling = false;
                return;
            }

            if (isClosed)
                return;

            curCompileClient = clientList[0];
            clientList.RemoveAt(0);
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
            if (isClosed)
                return;

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
            lbNameList.Items.Clear();
            lbFileList.Items.Clear();

            String path = Path.Combine(historyPath, cbDateList.SelectedItem.ToString());
            String[] lines = File.ReadAllLines(path);
            if (lines.Length == 0)
                return;

            Array.Reverse(lines);
            for (int i = 0;i < lines.Length;i++)
            {
                String[] temp = lines[i].Split(new String[] { ";" },StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length != 2)
                    continue;

                historyMap.Add(temp[0], temp[1]);

                lbNameList.Items.Add(temp[0]);
            }

            if (lbNameList.Items.Count > 0)
                lbNameList.SelectedIndex = 0;
        }


        private void loadFileNames()
        {
            lbFileList.Items.Clear();

            String selected = lbNameList.SelectedItem.ToString();
            String files = historyMap[selected];
            String[] fileList = files.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < fileList.Length; i++)
            {
                lbFileList.Items.Add(fileList[i]);
            }
        }


        private void cbDateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDateNames();
        }


        private void lbNameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadFileNames();
        }


        private void Server_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘
            {
                NotifyIcon.Visible = true;    //显示托盘图标
                this.Hide();    //隐藏窗口
            }
        }


        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            //注意判断关闭事件Reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件
                this.WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果
                NotifyIcon.Visible = true;
                this.Hide();
            }
        }


        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            showWindow();
        }


        private void ShowWindow_Click(object sender, EventArgs e)
        {
            showWindow();
        }


        private void ExitWindow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要退出编译服务端吗？\n这样会导致前端开发无法远程编译Flash文件。", "确定退出", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                isClosed = true;
                notifyAllClient();
                server.dispose();
                Application.Exit();
            }
        }


        private void notifyAllClient()
        {
            for(int i = 0;i < clientList.Count; i++)
            {
                server.sendMessage(clientList[i], "服务端已关闭。");
                clientList[i].client.Close();
            }
        }


        private void showWindow()
        {
            NotifyIcon.Visible = false;
            this.Show();
            WindowState = FormWindowState.Normal;
            this.Focus();
        }
    }
}
