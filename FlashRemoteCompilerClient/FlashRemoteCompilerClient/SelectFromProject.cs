using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FlashRemoteCompilerClient
{


    public partial class SelectFromProject : Form
    {
        private Action<String[]> Callback;


        public SelectFromProject()
        {
            InitializeComponent();
        }


        public void setCallback(Action<String[]> action)
        {
            Callback += action;
        }


        private void Form_Load(object sender, EventArgs e)
        {
            lbFlaList.Items.Clear();
            lbFlaList.DisplayMember = "displayName";

            loadProjectDates();
        }


        private void loadProjectDates()
        {
            cbDateList.Items.Clear();
            String[] folders = Directory.GetDirectories(FlashRemoteCompilerClient.projectPath, "*", SearchOption.TopDirectoryOnly);
            Array.Reverse(folders);
            for (int i = 0;i < folders.Length;i++)
            {
                DirectoryInfo info = new DirectoryInfo(folders[i]);
                try
                {
                    int.Parse(info.Name);
                    cbDateList.Items.Add(info.Name);
                }
                catch (Exception)
                {

                }
            }
            cbDateList.SelectedIndex = 0;
        }


        private void loadUserProjects()
        {
            cbUserList.Items.Clear();
            string folder = Path.Combine(FlashRemoteCompilerClient.projectPath, cbDateList.SelectedItem.ToString());
            string[] fileNames = Directory.GetFiles(folder, "*.flp", SearchOption.TopDirectoryOnly);

            if (fileNames.Length == 0)
                return ;

            int myIndex = -1;
            for (int i = 0;i < fileNames.Length;i++)
            {
                FileInfo info = new FileInfo(fileNames[i]);
                cbUserList.Items.Add(info.Name);

                if (info.Name.IndexOf(System.Environment.UserName) >= 0)
                    myIndex = i;
            }

            if (myIndex > -1)
                cbUserList.SelectedIndex = myIndex;
            else
                cbUserList.SelectedIndex = 0;
        }


        private void loadSelectedUserProject()
        {
            String projectName = Path.Combine(FlashRemoteCompilerClient.projectPath, cbDateList.SelectedItem.ToString(), cbUserList.SelectedItem.ToString());
            XmlDocument doc = new XmlDocument();
            doc.Load(projectName);
            XmlNodeList list = doc.SelectNodes("/flash_project/project_file");
            lbFlaList.Items.Clear();
            lbFlaList.DisplayMember = "displayName";
            for(int i = 0;i < list.Count;i++)
            {
                FileItem item = FileItem.fromXmlNode(list[i]);
                if(item != null)
                    lbFlaList.Items.Add(item);
            }
        }


        private void cbDateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadUserProjects();
        }


        private void cbUserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadSelectedUserProject();
        }


        private void btnSelect_Click(object sender, EventArgs e)
        {
            if(lbFlaList.SelectedItems.Count <= 0)
            {
                this.Close();
                return;
            }

            String[] selectedFileList = new String[lbFlaList.SelectedItems.Count];
            for(int i = 0;i < lbFlaList.SelectedItems.Count;i++)
            {
                selectedFileList[i] = (lbFlaList.SelectedItems[i] as FileItem).realPath;
            }
            Callback(selectedFileList);
            this.Close();
        }


        private void lbFlaList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileItem item = lbFlaList.SelectedItem as FileItem;
            if (item != null)
            {
                txtRealPath.Text = item.realPath;
            }
        }
    }
}
