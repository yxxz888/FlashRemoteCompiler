namespace FlashRemoteCompilerServer
{
    partial class Server
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cbDateList = new System.Windows.Forms.ComboBox();
            this.lbNameList = new System.Windows.Forms.ListBox();
            this.lbFileList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbDateList
            // 
            this.cbDateList.FormattingEnabled = true;
            this.cbDateList.Location = new System.Drawing.Point(13, 41);
            this.cbDateList.Name = "cbDateList";
            this.cbDateList.Size = new System.Drawing.Size(133, 20);
            this.cbDateList.TabIndex = 0;
            this.cbDateList.SelectedIndexChanged += new System.EventHandler(this.cbDateList_SelectedIndexChanged);
            // 
            // lbNameList
            // 
            this.lbNameList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbNameList.FormattingEnabled = true;
            this.lbNameList.ItemHeight = 12;
            this.lbNameList.Location = new System.Drawing.Point(12, 75);
            this.lbNameList.Name = "lbNameList";
            this.lbNameList.Size = new System.Drawing.Size(190, 328);
            this.lbNameList.TabIndex = 1;
            this.lbNameList.SelectedIndexChanged += new System.EventHandler(this.lbNameList_SelectedIndexChanged);
            // 
            // lbFileList
            // 
            this.lbFileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFileList.FormattingEnabled = true;
            this.lbFileList.ItemHeight = 12;
            this.lbFileList.Location = new System.Drawing.Point(217, 75);
            this.lbFileList.Name = "lbFileList";
            this.lbFileList.Size = new System.Drawing.Size(320, 328);
            this.lbFileList.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "编译历史：";
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 415);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbFileList);
            this.Controls.Add(this.lbNameList);
            this.Controls.Add(this.cbDateList);
            this.Name = "Server";
            this.Text = "FlashRemoteCompilerServer";
            this.Load += new System.EventHandler(this.Server_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDateList;
        private System.Windows.Forms.ListBox lbNameList;
        private System.Windows.Forms.ListBox lbFileList;
        private System.Windows.Forms.Label label1;
    }
}

