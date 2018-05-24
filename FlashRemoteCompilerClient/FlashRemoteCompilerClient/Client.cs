using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml;

namespace FlashRemoteCompilerClient
{
    public partial class FlashRemoteCompilerClient : Form
    {

        public static String mmoPath = @"D:\vstsworkspace\mmo\";
        public static String sourcePath = Path.Combine(mmoPath, @"source\");
        public static String assetsPath = Path.Combine(sourcePath, @"assets\");
        public static String projectPath = Path.Combine(sourcePath, @"project\");

        private TcpClient client;
        private String ip;
        private int port;
        private String lastReadMessage = "";
        private Boolean isStartCompile = false;

        //下述字符串用作客户端判断任务是否完成的标记，修改的话请同步修改服务端。
        private String endCompileMark = "\r\n本次操作结束。";

        private String separator = "->";

        private List<FileItem> recentlyFileItems = new List<FileItem>();

        public FlashRemoteCompilerClient()
        {
            InitializeComponent();
        }


        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            handleGetLastestTool();
        }


        private void handleGetLastestTool()
        {
            String script = "@echo off" + "\r\n";
            script += "tf get \"%1\" /r" + "\r\n";
            String fileName = "getLastest.bat";
            StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8);
            sw.Flush();
            sw.Write(script);
            sw.Close();

            Process p = new Process();
            p.StartInfo.FileName = Path.Combine(Application.StartupPath, fileName);
            String path = Path.Combine(Application.StartupPath, "FlashRemoteCompilerClient.exe");
            p.StartInfo.Arguments = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.Close();
        }


        private void Form_Load(object sender, EventArgs e)
        {
            readConfig();
            initClient();

            lbFlaList.Items.Clear();
            lbFlaList.DisplayMember = "displayName";

            //不规范做法，但是做工具偷个懒算了
            CheckForIllegalCrossThreadCalls = false;
        }


        private void readConfig()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            ip = doc.SelectSingleNode("config/ip").FirstChild.Value;
            port = int.Parse(doc.SelectSingleNode("config/port").FirstChild.Value);
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = assetsPath;
            ofd.Multiselect = true;
            //ofd.Filter = "fla文件(*.fla)|*.fla";
            if (ofd.ShowDialog() == DialogResult.OK)
                addFile(ofd.FileNames);
        }


        private void btnAddFromProject_Click(object sender, EventArgs e)
        {
            SelectFromProject form = new SelectFromProject();
            Action<String[]> action = (fileList) => addFile(fileList);
            form.setCallback(action);
            form.ShowDialog();
        }


        private void btnAddFromRecent_Click(object sender, EventArgs e)
        {
            SelectFromRecentFile form = new SelectFromRecentFile(recentlyFileItems, (fileList) => addFile(fileList));
            form.ShowDialog();
        }


        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbFlaList.SelectedItems.Count <= 0)
                return;

            FileItem[] items = new FileItem[lbFlaList.SelectedItems.Count];
            lbFlaList.SelectedItems.CopyTo(items,0);
            string deleteNames = "\n";
            for (int i = 0; i < items.Length; i++)
                deleteNames += items[i].fileName + "\n";
            if (MessageBox.Show("确定要删除以下" + items.Length + "个文件吗？" + deleteNames, "确认删除",MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                for (int i = 0; i < items.Length; i++)
                    lbFlaList.Items.Remove(items[i]);
                txtRealPath.Text = "";
            }
        }


        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            if (lbFlaList.Items.Count <= 0)
                return;

            if (MessageBox.Show("确定要全部删除吗？", "确认删除", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                lbFlaList.Items.Clear();
                txtRealPath.Text = "";
            }
        }


        private void btnCompile_Click(object sender, EventArgs e)
        {
            handleCompile();
        }


        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }


        private void lbFlaList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileItem item = lbFlaList.SelectedItem as FileItem;
            if (item != null)
            {
                txtRealPath.Text = item.realPath;
            }
        }


        private void addFile(string[] fileNames)
        {
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (checkIfExist(fileNames[i]))
                    continue;

                FileItem item = new FileItem(fileNames[i]);
                lbFlaList.Items.Add(item);

                addFileToRecentList(item);
            }
        }


        private void addFileToRecentList(FileItem file)
        {
            for(int i = recentlyFileItems.Count - 1; i >= 0; i--)
            {
                if(recentlyFileItems[i].realPath == file.realPath)
                {
                    recentlyFileItems.RemoveAt(i);
                    break;
                }
            }
            recentlyFileItems.Insert(0, file);
        }


        private Boolean checkIfExist(string realPath)
        {
            Boolean result = false;
            for(int i = 0;i < lbFlaList.Items.Count;i++)
            {
                if ((lbFlaList.Items[i] as FileItem).realPath == realPath)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


        private void handleCompile()
        {
            if (isStartCompile)
            {
                showNextLineLog("正在远程编译，请耐心等候......");
                return;
            }

            if (lbFlaList.Items.Count <= 0)
                return;

            String flaListStr = "";
            for (int i = 0; i < lbFlaList.Items.Count; i++)
            {
                FileItem item = lbFlaList.Items[i] as FileItem;
                if(item.fileName.ToLower() != item.fileName)
                {
                    MessageBox.Show("发现文件“" + item.fileName + "”不是全小写，停止编译。");
                    return;
                }
                flaListStr += item.relativePath;
                if (i < lbFlaList.Items.Count - 1)
                    flaListStr += ",";
            }

            handleSendToServer(flaListStr);
        }


        private void handleSendToServer(String msg)
        {
            if (msg == null || msg.Length == 0)
                return;

            if (isConnecting() == false)
                initClient();

            handleSendFile(msg);
        }


        private void initClient()
        {
            client = new TcpClient();
            try
            {
                client.Connect(ip, port);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (client.Connected)
                {
                    showNextLineLog("连接服务端成功！");
                    handleSendName();
                    handleRead();
                }
                else
                {
                    showNextLineLog("连接失败，请移步编译机查看服务端是否开启！");
                    client = null;
                    isStartCompile = false;
                }
            }
        }


        private void handleSendName()
        {
            String myName = System.Environment.MachineName;
            handleSend("name" + separator + myName);
            Thread.Sleep(1000);
        }


        private void handleSendFile(String msg)
        {
            if (isConnecting() == false)
                return;

            isStartCompile = true;
            showNextLineLog("发送文件中......");
            handleSend("list" + separator + msg);
        }


        private void handleSend(String message)
        {
            byte[] bytes = Encoding.Default.GetBytes(message);
            try
            {
                client.GetStream().BeginWrite(bytes, 0, bytes.Length, onWriteDataBack, null);
            }
            catch (Exception e)
            {
                showNextLineLog("已断开与服务端的连接。");
                Console.WriteLine(e.StackTrace);
                client.Close();
                client = null;
                isStartCompile = false;
            }
        }


        private void onWriteDataBack(IAsyncResult ar)
        {
            try
            {
                client.GetStream().EndWrite(ar);
            }
            catch (Exception e)
            {
                showNextLineLog("已断开与服务端的连接。");
                Console.WriteLine(e.StackTrace);
                client.Close();
                client = null;
                isStartCompile = false;
            }
        }


        private void handleRead()
        {
            byte[] buffer = new byte[128];
            client.GetStream().BeginRead(buffer, 0, buffer.Length, onReadDateBack, buffer);
        }


        private void onReadDateBack(IAsyncResult ar)
        {
            try
            {
                client.GetStream().EndRead(ar);
                byte[] buffer = (byte[])ar.AsyncState;
                lastReadMessage += Encoding.Default.GetString(buffer);
                if (client.GetStream().DataAvailable == false)//读完了
                {
                    showLog(lastReadMessage);
                    if (lastReadMessage.IndexOf(endCompileMark) > -1)//成功
                        handleCompileFinished();
                    lastReadMessage = "";
                }
                handleRead();
            }
            catch (Exception e)
            {
                showNextLineLog("已断开与服务端的连接。");
                Console.WriteLine(e.StackTrace);
                client.Close();
                client = null;
                isStartCompile = false;
            }
        }


        private void handleCompileFinished()
        {
            lbFlaList.Items.Clear();
            handleFinishCompile();
        }


        private void handleFinishCompile()
        {
            isStartCompile = false;
            showNextLineLog("=========================================== " + DateTime.Now.ToString("HH:mm:ss"));
        }


        private void showNextLineLog(String msg)
        {
            showLog("\r\n" + msg);
        }


        private void showLog(String msg)
        {
            txtLog.AppendText(msg);
        }


        private Boolean isConnecting()
        {
            return client != null && client.Connected;
        }
    }
}
