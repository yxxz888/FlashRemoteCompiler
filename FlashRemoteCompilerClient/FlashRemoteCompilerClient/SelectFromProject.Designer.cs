namespace FlashRemoteCompilerClient
{
    partial class SelectFromProject
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
            this.cbDateList = new System.Windows.Forms.ComboBox();
            this.cbUserList = new System.Windows.Forms.ComboBox();
            this.lbFlaList = new System.Windows.Forms.ListBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.txtRealPath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbDateList
            // 
            this.cbDateList.FormattingEnabled = true;
            this.cbDateList.Location = new System.Drawing.Point(13, 13);
            this.cbDateList.Name = "cbDateList";
            this.cbDateList.Size = new System.Drawing.Size(121, 20);
            this.cbDateList.TabIndex = 0;
            this.cbDateList.SelectedIndexChanged += new System.EventHandler(this.cbDateList_SelectedIndexChanged);
            // 
            // cbUserList
            // 
            this.cbUserList.FormattingEnabled = true;
            this.cbUserList.Location = new System.Drawing.Point(151, 12);
            this.cbUserList.Name = "cbUserList";
            this.cbUserList.Size = new System.Drawing.Size(121, 20);
            this.cbUserList.TabIndex = 1;
            this.cbUserList.SelectedIndexChanged += new System.EventHandler(this.cbUserList_SelectedIndexChanged);
            // 
            // lbFlaList
            // 
            this.lbFlaList.FormattingEnabled = true;
            this.lbFlaList.ItemHeight = 12;
            this.lbFlaList.Location = new System.Drawing.Point(13, 40);
            this.lbFlaList.Name = "lbFlaList";
            this.lbFlaList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFlaList.Size = new System.Drawing.Size(259, 148);
            this.lbFlaList.TabIndex = 2;
            this.lbFlaList.SelectedIndexChanged += new System.EventHandler(this.lbFlaList_SelectedIndexChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(106, 230);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "选择";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // txtRealPath
            // 
            this.txtRealPath.Location = new System.Drawing.Point(13, 196);
            this.txtRealPath.Name = "txtRealPath";
            this.txtRealPath.Size = new System.Drawing.Size(259, 31);
            this.txtRealPath.TabIndex = 4;
            // 
            // SelectFromProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.txtRealPath);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lbFlaList);
            this.Controls.Add(this.cbUserList);
            this.Controls.Add(this.cbDateList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SelectFromProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddFromProject";
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDateList;
        private System.Windows.Forms.ComboBox cbUserList;
        private System.Windows.Forms.ListBox lbFlaList;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label txtRealPath;
    }
}