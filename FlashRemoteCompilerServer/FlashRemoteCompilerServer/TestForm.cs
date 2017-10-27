using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashRemoteCompilerServer
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //String jsflPath = @"D:\vstsworkspace\mmo\source\tools\FlashRemoteCompiler\FlashRemoteCompilerServer\FlashRemoteCompilerServer\bin\Debug\CompileTool_temRunScript.jsfl";
            //Process p = new Process();
            //p.StartInfo.FileName = "\"" + jsflPath + "\"";
            //p.Start();
            //p.WaitForExit();
            //p.Close();

            String jsflPath = @"D:\vstsworkspace\mmo\source\tools\FlashRemoteCompiler\FlashRemoteCompilerServer\FlashRemoteCompilerServer\bin\Debug\buildsomeswc.jsfl";
            Process p = new Process();
            p.StartInfo.FileName = "\"" + jsflPath + "\"";
            p.Start();
            p.WaitForExit();
            p.Close();

            Console.WriteLine("finish");
            MessageBox.Show("finish");
        }
    }
}
