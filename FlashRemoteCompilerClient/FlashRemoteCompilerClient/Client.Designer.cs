namespace FlashRemoteCompilerClient
{
    partial class FlashRemoteCompilerClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlashRemoteCompilerClient));
            this.lbFlaList = new System.Windows.Forms.ListBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnCompile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddFromProject = new System.Windows.Forms.Button();
            this.txtRealPath = new System.Windows.Forms.Label();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnAddFromRecent = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbFlaList
            // 
            this.lbFlaList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFlaList.Font = new System.Drawing.Font("宋体", 10F);
            this.lbFlaList.FormattingEnabled = true;
            this.lbFlaList.Location = new System.Drawing.Point(12, 32);
            this.lbFlaList.Name = "lbFlaList";
            this.lbFlaList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFlaList.Size = new System.Drawing.Size(365, 186);
            this.lbFlaList.TabIndex = 1;
            this.lbFlaList.SelectedIndexChanged += new System.EventHandler(this.lbFlaList_SelectedIndexChanged);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.SystemColors.Control;
            this.txtLog.Font = new System.Drawing.Font("宋体", 10F);
            this.txtLog.Location = new System.Drawing.Point(12, 275);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(470, 185);
            this.txtLog.TabIndex = 2;
            this.txtLog.WordWrap = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(385, 32);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(99, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "添加文件";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(385, 128);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(99, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "删除文件";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveAll.Location = new System.Drawing.Point(385, 160);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(99, 23);
            this.btnRemoveAll.TabIndex = 5;
            this.btnRemoveAll.Text = "删除全部";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompile.Location = new System.Drawing.Point(385, 195);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(99, 23);
            this.btnCompile.TabIndex = 6;
            this.btnCompile.Text = "开始编译上传";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "待编译列表：";
            // 
            // btnAddFromProject
            // 
            this.btnAddFromProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFromProject.Location = new System.Drawing.Point(385, 64);
            this.btnAddFromProject.Name = "btnAddFromProject";
            this.btnAddFromProject.Size = new System.Drawing.Size(99, 23);
            this.btnAddFromProject.TabIndex = 8;
            this.btnAddFromProject.Text = "从变更集添加";
            this.btnAddFromProject.UseVisualStyleBackColor = true;
            this.btnAddFromProject.Click += new System.EventHandler(this.btnAddFromProject_Click);
            // 
            // txtRealPath
            // 
            this.txtRealPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRealPath.Location = new System.Drawing.Point(13, 231);
            this.txtRealPath.Name = "txtRealPath";
            this.txtRealPath.Size = new System.Drawing.Size(365, 30);
            this.txtRealPath.TabIndex = 9;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLog.Location = new System.Drawing.Point(385, 245);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(98, 24);
            this.btnClearLog.TabIndex = 10;
            this.btnClearLog.Text = "清除日志";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnAddFromRecent
            // 
            this.btnAddFromRecent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFromRecent.Location = new System.Drawing.Point(385, 96);
            this.btnAddFromRecent.Name = "btnAddFromRecent";
            this.btnAddFromRecent.Size = new System.Drawing.Size(99, 23);
            this.btnAddFromRecent.TabIndex = 8;
            this.btnAddFromRecent.Text = "从最近文件添加";
            this.btnAddFromRecent.UseVisualStyleBackColor = true;
            this.btnAddFromRecent.Click += new System.EventHandler(this.btnAddFromRecent_Click);
            // 
            // FlashRemoteCompilerClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 472);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.txtRealPath);
            this.Controls.Add(this.btnAddFromRecent);
            this.Controls.Add(this.btnAddFromProject);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCompile);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lbFlaList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(510, 510);
            this.Name = "FlashRemoteCompilerClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FlashRemoteCompiler";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Closed);
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox lbFlaList;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddFromProject;
        private System.Windows.Forms.Label txtRealPath;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnAddFromRecent;
    }
}

