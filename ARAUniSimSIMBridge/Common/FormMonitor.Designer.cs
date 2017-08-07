namespace ARAUniSimSIMBridge
{
    partial class FormMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMonitor));
            this.panelMonitor = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripComboBoxOPCServer = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemServerInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemElapsedTime = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMonitor
            // 
            this.panelMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMonitor.Location = new System.Drawing.Point(0, 27);
            this.panelMonitor.Name = "panelMonitor";
            this.panelMonitor.Size = new System.Drawing.Size(822, 496);
            this.panelMonitor.TabIndex = 0;
            this.panelMonitor.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panelMonitor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMonitor_MouseDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxOPCServer,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(822, 27);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripComboBoxOPCServer
            // 
            this.toolStripComboBoxOPCServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxOPCServer.DropDownWidth = 200;
            this.toolStripComboBoxOPCServer.Name = "toolStripComboBoxOPCServer";
            this.toolStripComboBoxOPCServer.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBoxOPCServer.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxOPCServer_SelectedIndexChanged);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemServerInfo,
            this.toolStripMenuItemElapsedTime});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(78, 23);
            this.toolStripMenuItem1.Text = "Infomation";
            // 
            // toolStripMenuItemServerInfo
            // 
            this.toolStripMenuItemServerInfo.Name = "toolStripMenuItemServerInfo";
            this.toolStripMenuItemServerInfo.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemServerInfo.Text = "Server";
            this.toolStripMenuItemServerInfo.Click += new System.EventHandler(this.toolStripMenuItemServerInfo_Click);
            // 
            // toolStripMenuItemElapsedTime
            // 
            this.toolStripMenuItemElapsedTime.Name = "toolStripMenuItemElapsedTime";
            this.toolStripMenuItemElapsedTime.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemElapsedTime.Text = "Elapsed Time";
            this.toolStripMenuItemElapsedTime.Click += new System.EventHandler(this.toolStripMenuItemElapsedTime_Click);
            // 
            // FormMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 523);
            this.Controls.Add(this.panelMonitor);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMonitor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMonitor_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMonitor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxOPCServer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemServerInfo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemElapsedTime;
    }
}