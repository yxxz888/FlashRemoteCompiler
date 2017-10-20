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
            this.cbDate = new System.Windows.Forms.ComboBox();
            this.lbNameList = new System.Windows.Forms.ListBox();
            this.lbFileList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // cbDate
            // 
            this.cbDate.FormattingEnabled = true;
            this.cbDate.Location = new System.Drawing.Point(13, 13);
            this.cbDate.Name = "cbDate";
            this.cbDate.Size = new System.Drawing.Size(133, 20);
            this.cbDate.TabIndex = 0;
            // 
            // lbNameList
            // 
            this.lbNameList.FormattingEnabled = true;
            this.lbNameList.ItemHeight = 12;
            this.lbNameList.Location = new System.Drawing.Point(13, 52);
            this.lbNameList.Name = "lbNameList";
            this.lbNameList.Size = new System.Drawing.Size(200, 340);
            this.lbNameList.TabIndex = 1;
            // 
            // lbFileList
            // 
            this.lbFileList.FormattingEnabled = true;
            this.lbFileList.ItemHeight = 12;
            this.lbFileList.Location = new System.Drawing.Point(254, 52);
            this.lbFileList.Name = "lbFileList";
            this.lbFileList.Size = new System.Drawing.Size(200, 340);
            this.lbFileList.TabIndex = 2;
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 404);
            this.Controls.Add(this.lbFileList);
            this.Controls.Add(this.lbNameList);
            this.Controls.Add(this.cbDate);
            this.Name = "Server";
            this.Text = "FlashRemoteCompilerServer";
            this.Load += new System.EventHandler(this.Server_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDate;
        private System.Windows.Forms.ListBox lbNameList;
        private System.Windows.Forms.ListBox lbFileList;
    }
}

