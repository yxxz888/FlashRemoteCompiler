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

namespace FlashRemoteCompilerClient
{
    public partial class FlashRemoteCompilerClient : Form
    {

        public static String mmoPath = @"D:\vstsworkspace\mmo\";
        public static String sourcePath = Path.Combine(mmoPath, @"source\");
        public static String assetsPath = Path.Combine(sourcePath, @"assets\");
        public static String projectPath = Path.Combine(sourcePath, @"project\");

        private TcpClient client;
        private String ip = "127.0.0.1";
        private int port = 14141;
        private Boolean isSending = false;
        private ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        private String lastReadMessage = "";

        public FlashRemoteCompilerClient()
        {
            InitializeComponent();
        }


        private void Form_Load(object sender, EventArgs e)
        {
            initClient();

            lbFlaList.Items.Clear();
            lbFlaList.DisplayMember = "displayName";

            //不规范做法，但是做工具偷个懒算了
            CheckForIllegalCrossThreadCalls = false;

            manualResetEvent.Reset();
        }


        private void Form_Closed(object sender, FormClosedEventArgs e)
        {

        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = assetsPath;
            ofd.Multiselect = true;
            ofd.Filter = "fla文件(*.fla)|*.fla";
            if (ofd.ShowDialog() == DialogResult.OK)
                addFile(ofd.FileNames);
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


        private void btnAddFromProject_Click(object sender, EventArgs e)
        {
            SelectFromProject form = new SelectFromProject();
            Action<String[]> action = (fileList) => addFile(fileList);
            form.setCallback(action);
            form.ShowDialog();
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
            }
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
                flaListStr += item.realPath;
                if (i < lbFlaList.Items.Count - 1)
                    flaListStr += ",";
            }

            handleSendToServer(flaListStr);
        }


        private void handleSendToServer(String msg)
        {
            if (msg == null || msg.Length == 0)
                return;

            if (isSending)
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
            catch{ }
            finally
            {
                manualResetEvent.Set();
            }

            manualResetEvent.WaitOne(5 * 1000);
            if (client.Connected)
            {
                showLog("连接服务端成功！");
                handleSendName();
                handleRead();
            }
            else
            {
                showLog("连接失败，请移步编译机查看服务端是否开启！");
                client = null;
            }
        }


        private void handleSendName()
        {
            String myName = System.Environment.MachineName;
            handleSend("name:" + myName);
        }


        private void handleSendFile(String msg)
        {
            if (isConnecting() == false)
                return;

            showLog("发送文件中......");
            handleSend("list:" + msg);
        }


        private void handleSend(String message)
        {
            if (isConnecting() == false)
                return;

            byte[] bytes = Encoding.Default.GetBytes(message);
            try
            {
                client.GetStream().BeginWrite(bytes, 0, bytes.Length, onWriteDataBack, null);
            }
            catch
            {
                client.Close();
                client = null;
            }
        }


        private void onWriteDataBack(IAsyncResult ar)
        {
            client.GetStream().EndWrite(ar);
        }


        private void handleRead()
        {
            byte[] buffer = new byte[8];
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
                    lastReadMessage = "";
                }
                handleRead();
            }
            catch
            {
                client.Close();
                client = null;
            }
        }


        private void showLog(String msg)
        {
            txtLog.AppendText(msg + "\n");
        }


        private Boolean isConnecting()
        {
            return client != null && client.Connected;
        }


        private void compileFlaList()
        {
            String[] flaList = {
                "assets/activity/20170929/nightflycompete.fla",
                "assets/activity/20170929/nightflycompetegame.fla",
            };
            String sourcePathForJSFL = "file:///D|/vstsworkspace/mmo/source/";
            String tempFilePath = Path.Combine(Application.StartupPath, "compileflatemp.jsfl");
            FileInfo tempFi = new FileInfo(tempFilePath);
            Boolean readOnly = tempFi.IsReadOnly;
            tempFi.IsReadOnly = false;
            StreamReader sr = new StreamReader(File.Open(tempFilePath, FileMode.Open), Encoding.UTF8);
            String script = sr.ReadToEnd();
            sr.Close();
            tempFi.IsReadOnly = readOnly;
            String listScript = "";
            for (int i = 0; i < flaList.Length; i++)
            {
                listScript += String.Format("\"{0}\",\r\n", sourcePathForJSFL + flaList[i]);
            }
            script = script.Replace("/替代列表/", listScript);
            String jsflName = Path.Combine(Application.StartupPath, "compilefla.jsfl");
            FileStream fs = File.Create(jsflName);
            fs.Close();
            StreamWriter sw = new StreamWriter(jsflName, false, Encoding.UTF8);
            sw.Write(script);
            sw.Flush();
            sw.Close();

            Process p = new Process();
            p.StartInfo.FileName = Path.Combine(Application.StartupPath, "compilefla.bat");
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;//不要问为啥。。反正不加这行就没反应(zjf.2013.10.12)
            p.Start();
            p.WaitForExit();
            p.Close();

            MessageBox.Show("Finish!");
        }
    }
}
