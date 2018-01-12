using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashRemoteCompilerServer
{
    class GetLastestUtil
    {
        public delegate void GetFinishedCallback(String message);

        private GetFinishedCallback onGetFinished;


        public void getLastestFiles(GetFinishedCallback callback)
        {
            onGetFinished = callback;

            String batPath = Application.StartupPath + @"\getfile.bat";
            String result = runBatch(batPath);
            onGetFinished(result);
        }


        private string runBatch(string path)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = ConfigInfo.source;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            p.Close();
            return output;
        }
    }
}
