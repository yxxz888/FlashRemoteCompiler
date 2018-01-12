namespace FlashRemoteCompilerClient
{
    partial class SelectFromRecentFile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelect = new System.Windows.Forms.Button();
            this.txtRealPath = new System.Windows.Forms.Label();
            this.lbFileList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(103, 234);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "选择";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // txtRealPath
            // 
            this.txtRealPath.Location = new System.Drawing.Point(11, 195);
            this.txtRealPath.Name = "txtRealPath";
            this.txtRealPath.Size = new System.Drawing.Size(259, 31);
            this.txtRealPath.TabIndex = 5;
            // 
            // lbFileList
            // 
            this.lbFileList.FormattingEnabled = true;
            this.lbFileList.ItemHeight = 12;
            this.lbFileList.Location = new System.Drawing.Point(13, 13);
            this.lbFileList.Name = "lbFileList";
            this.lbFileList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFileList.Size = new System.Drawing.Size(259, 172);
            this.lbFileList.TabIndex = 6;
            this.lbFileList.SelectedIndexChanged += new System.EventHandler(this.lbFileList_SelectedIndexChanged);
            // 
            // SelectFromRecentFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.lbFileList);
            this.Controls.Add(this.txtRealPath);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SelectFromRecentFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SelectFromRecentFile";
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label txtRealPath;
        private System.Windows.Forms.ListBox lbFileList;
    }
}