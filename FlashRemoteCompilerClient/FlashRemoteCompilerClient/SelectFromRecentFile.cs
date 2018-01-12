using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashRemoteCompilerClient
{
    public partial class SelectFromRecentFile : Form
    {
        private Action<String[]> Callback;

        private List<FileItem> list;

        public SelectFromRecentFile(List<FileItem> fileList, Action<String[]> action)
        {
            InitializeComponent();

            list = fileList;    

            Callback = action;
        }


        private void Form_Load(object sender, EventArgs e)
        {
            lbFileList.Items.Clear();
            lbFileList.DisplayMember = "displayName";
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i]);
                lbFileList.Items.Add(list[i]);
            }
        }


        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (lbFileList.SelectedItems.Count <= 0)
            {
                this.Close();
                return;
            }

            String[] selectedFileList = new String[lbFileList.SelectedItems.Count];
            for (int i = 0; i < lbFileList.SelectedItems.Count; i++)
            {
                selectedFileList[i] = (lbFileList.SelectedItems[i] as FileItem).realPath;
            }

            Callback(selectedFileList);
            this.Close();
        }


        private void lbFileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileItem item = lbFileList.SelectedItem as FileItem;
            if (item != null)
            {
                txtRealPath.Text = item.realPath;
            }
        }
    }
}
