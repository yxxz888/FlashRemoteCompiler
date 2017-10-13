using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashRemoteCompilerServer
{
    public partial class TestClient : Form
    {

        private TcpClient client;

        public TestClient()
        {
            InitializeComponent();
        }

        private void TestClient_Load(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            client.Connect("127.0.0.1", 14141);
            showLog("连接成功");
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            String msg = txtInput.Text;
            byte[] temp = Encoding.Default.GetBytes(msg);
            client.GetStream().Write(temp, 0, temp.Length);
            showLog("发送成功");
            txtInput.Text = "";
        }


        private void showLog(String msg)
        {
            txtLog.AppendText(msg + "\n");
        }
    }
}
