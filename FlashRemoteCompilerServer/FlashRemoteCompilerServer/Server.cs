﻿using System;
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
        private GetLastestUtil getLastestUtil = new GetLastestUtil();

        private String historyPath = Path.Combine(Application.StartupPath, "history");
        private Dictionary<String, String> historyMap = new Dictionary<String, String>();

        //下述字符串用作客户端判断是否完成的标记，修改的话请同步修改客户端。
        private String endCompileMark = "\r\n本次操作结束。";

        //编译swc出现失败时的提示，用于区分是swc编译失败还是swf编译失败, 修改需要同步修改buildsomeswc.jsfl文件的exportSWC()方法中的提示语。
        private String compileSWCFailMark = "编译swc出现异常";

        private Boolean isClosed = false;

        private Object lockObj = new Object();

        private System.Timers.Timer timer;
        private String lastFileName = "";

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

            timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += onTimer;
        }


        private void onTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            if (curCompileClient == null)
                return;

            String logPath = ConfigInfo.getCSharpPath(ConfigInfo.compileHistoryPath);
            if (File.Exists(logPath))
            {
                String[] fileNames = File.ReadAllLines(logPath);
                String fileName = ConfigInfo.getCSharpPath(fileNames[fileNames.Length - 1]);
                if (lastFileName != fileName)
                {
                    lastFileName = fileName;
                    server.sendMessage(curCompileClient, "\r\n正在编译" + fileName.Substring(ConfigInfo.assets.Length));
                }
            }
        }


        private void initSocket()
        {
            server = new SocketServer(onFileListReceive);
        }


        private void onFileListReceive(ClientObject client)
        {
            lock(lockObj)
            {
                clientList.Add(client);
            }

            if (isCompiling)
                server.sendMessage(client, "\r\n已有编译任务，进入等待队列，" + "前面还有" + clientList.Count + "人和" + getFileCounts() + "个文件......");
            else
                beginTask();
            
        }


        private void beginTask()
        {
            if (clientList.Count <= 0)
            {
                isCompiling = false;
                return;
            }

            if (isClosed)
                return;

            isCompiling = true;
            curCompileClient = clientList[0];
            clientList.RemoveAt(0);

            for (int i = 0; i < clientList.Count; i++)
                server.sendMessage(clientList[i], "\r\n前面还有" + (i + 1) + "人和" + getFileCounts() + "个文件......");

            if (ConfigInfo.checkIsBvt())//编译机是要全获source文件夹的，开发机做这个操作的话会爆炸！
                handleGetLastestFiles();
            else
                handleCompile();
        }


        private void handleGetLastestFiles()
        {
            if (isClosed)
                return;

            server.sendMessage(curCompileClient, "\r\n获取文件中......");
            getLastestUtil.getLastestFiles(onGetLastestEnd);
        }


        private void onGetLastestEnd(string message)
        {
            if (message.IndexOf("成功") > -1)
            {
                server.sendMessage(curCompileClient, "\r\n获取文件成功。");
                handleCompile();
            }
            else
            {
                server.sendMessage(curCompileClient, "\r\n" + message);
                finishTask();
            }
        }


        private void handleCompile()
        {    
            server.sendMessage(curCompileClient, "\r\n开始编译......");
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

            if (File.Exists(logPath) == false)//每次编译前都会删除log文件，如果log不存在，则编译无错误
            {
                handleUpload();
            }
            else
            {

                String compileResult = File.ReadAllText(logPath);
                server.sendMessage(curCompileClient, compileResult + "\r\n");
                if (compileResult.IndexOf(compileSWCFailMark) > -1)//编译swc出错，终止上传
                {
                    server.sendMessage(curCompileClient, "\r\n上传终止。" + endCompileMark);
                    finishTask();
                }
                else
                {
                    String failLogPath = ConfigInfo.getCSharpPath(ConfigInfo.failFileLog);
                    String[] failFileList = File.ReadAllLines(failLogPath);
                    String[] originFileList = curCompileClient.fileList;
                    foreach(String fileName in failFileList)
                    {
                        originFileList = removeValueFromList(originFileList, ConfigInfo.getCSharpPath(fileName));
                    }
                    if (originFileList.Length > 0)
                        handleUpload(originFileList);
                    else
                    {
                        server.sendMessage(curCompileClient, "\r\n没有可上传的文件，上传终止。" + endCompileMark);
                        finishTask();
                    }
                }
            }
        }


        private String[] removeValueFromList(String[] list, String value)
        {
            List<String> lists = new List<String>(list);
            for(int i = lists.Count - 1;i >= 0;i--)
            {
                if(lists[i] == value)
                {
                    lists.RemoveAt(i);
                    break;
                }
            }
            return lists.ToArray();
        }


        private void finishTask()
        {
            curCompileClient.finishCompile();
            curCompileClient = null;
            lastFileName = "";
            deleteLogs();
            beginTask();
        }


        private void handleUpload()
        {
            handleUpload(curCompileClient.fileList);
        }


        private void handleUpload(String[] fileList)
        {
            if (isClosed)
                return;

            server.sendMessage(curCompileClient, "\r\n编译完成，正在上传编译通过的文件......");
            curCompileClient.uploadFileList = fileList;
            uploadUtil.uploadFile(fileList, onUploadFinished);
        }


        private void onUploadFinished(Boolean isSuc,String message)
        {
            server.sendMessage(curCompileClient, "\r\n" + message + endCompileMark);
            writeHistory(curCompileClient);
            finishTask();
        }


        private void writeHistory(ClientObject client)
        {
            String today = (DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day) + ".txt";
            String path = Path.Combine(historyPath, today);

            String history = client.name + " " + DateTime.Now.ToString("HH:mm:ss") + ";";
            for(int i = 0;i < client.uploadFileList.Length;i++)
            {
                history += client.uploadFileList[i].Substring(ConfigInfo.assets.Length);
                if (i < client.uploadFileList.Length - 1)
                    history += ",";
            }

            File.AppendAllLines(path,new String[] { history });

            loadDateFiles();
        }


        private int getFileCounts()
        {
            int result = 0;
            for (int i = 0; i < clientList.Count; i++)
            {
                result += clientList[i].flaList.Length;
            }
            return result;
        }


        private void deleteLogs()
        {
            List<String> paths = new List<String>();
            paths.Add(ConfigInfo.getCSharpPath(ConfigInfo.compileLog));
            paths.Add(ConfigInfo.getCSharpPath(ConfigInfo.failFileLog));
            paths.Add(ConfigInfo.getCSharpPath(ConfigInfo.compileHistoryPath));
            foreach (String path in paths)
            {
                if(File.Exists(path))
                    File.Delete(path);
            }
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
                server.sendMessage(clientList[i], "\r\n服务端已关闭。");
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
