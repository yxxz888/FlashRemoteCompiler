using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            server = new SocketServer();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            TestClient form = new TestClient();
            form.Show();
        }
    }
}
