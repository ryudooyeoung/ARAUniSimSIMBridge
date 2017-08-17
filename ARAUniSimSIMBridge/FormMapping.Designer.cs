namespace ARAUniSimSIMBridge
{
    partial class FormMapping
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapping));
            this.groupBoxUniSim = new System.Windows.Forms.GroupBox();
            this.dataGridViewOTS = new System.Windows.Forms.DataGridView();
            this.treeViewAdvTag = new Aga.Controls.Tree.TreeViewAdv();
            this.nodeIcon2 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.groupBoxMapping = new System.Windows.Forms.GroupBox();
            this.dataGridViewMapping = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnArrange = new System.Windows.Forms.Button();
            this.checkMapping = new System.Windows.Forms.CheckBox();
            this.checkOTS = new System.Windows.Forms.CheckBox();
            this.checkOPC = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnReverse = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.groupBoxOPC = new System.Windows.Forms.GroupBox();
            this.dataGridViewOPC = new System.Windows.Forms.DataGridView();
            this.treeViewAdvOPC = new Aga.Controls.Tree.TreeViewAdv();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.groupBoxUniSim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOTS)).BeginInit();
            this.groupBoxMapping.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMapping)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBoxOPC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOPC)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxUniSim
            // 
            this.groupBoxUniSim.Controls.Add(this.dataGridViewOTS);
            this.groupBoxUniSim.Controls.Add(this.treeViewAdvTag);
            this.groupBoxUniSim.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxUniSim.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxUniSim.Location = new System.Drawing.Point(460, 0);
            this.groupBoxUniSim.Name = "groupBoxUniSim";
            this.groupBoxUniSim.Size = new System.Drawing.Size(434, 262);
            this.groupBoxUniSim.TabIndex = 1;
            this.groupBoxUniSim.TabStop = false;
            this.groupBoxUniSim.Text = "UniSim";
            // 
            // dataGridViewOTS
            // 
            this.dataGridViewOTS.AllowUserToAddRows = false;
            this.dataGridViewOTS.AllowUserToResizeRows = false;
            this.dataGridViewOTS.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewOTS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewOTS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOTS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOTS.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewOTS.Name = "dataGridViewOTS";
            this.dataGridViewOTS.ReadOnly = true;
            this.dataGridViewOTS.RowHeadersVisible = false;
            this.dataGridViewOTS.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOTS.Size = new System.Drawing.Size(428, 242);
            this.dataGridViewOTS.TabIndex = 4;
            // 
            // treeViewAdvTag
            // 
            this.treeViewAdvTag.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdvTag.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewAdvTag.DefaultToolTipProvider = null;
            this.treeViewAdvTag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAdvTag.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdvTag.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdvTag.Location = new System.Drawing.Point(3, 17);
            this.treeViewAdvTag.Model = null;
            this.treeViewAdvTag.Name = "treeViewAdvTag";
            this.treeViewAdvTag.NodeControls.Add(this.nodeIcon2);
            this.treeViewAdvTag.NodeControls.Add(this.nodeTextBox2);
            this.treeViewAdvTag.SelectedNode = null;
            this.treeViewAdvTag.Size = new System.Drawing.Size(428, 242);
            this.treeViewAdvTag.TabIndex = 3;
            this.treeViewAdvTag.Text = "treeViewAdv1";
            this.treeViewAdvTag.Visible = false;
            this.treeViewAdvTag.DoubleClick += new System.EventHandler(this.treeViewAdv1_DoubleClick);
            // 
            // nodeIcon2
            // 
            this.nodeIcon2.DataPropertyName = "Image";
            this.nodeIcon2.LeftMargin = 1;
            this.nodeIcon2.ParentColumn = null;
            this.nodeIcon2.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "Text";
            this.nodeTextBox2.IncrementalSearchEnabled = true;
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = null;
            // 
            // groupBoxMapping
            // 
            this.groupBoxMapping.Controls.Add(this.dataGridViewMapping);
            this.groupBoxMapping.Controls.Add(this.panel1);
            this.groupBoxMapping.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxMapping.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxMapping.Location = new System.Drawing.Point(0, 272);
            this.groupBoxMapping.Name = "groupBoxMapping";
            this.groupBoxMapping.Size = new System.Drawing.Size(894, 200);
            this.groupBoxMapping.TabIndex = 2;
            this.groupBoxMapping.TabStop = false;
            this.groupBoxMapping.Text = "Mapping List";
            // 
            // dataGridViewMapping
            // 
            this.dataGridViewMapping.AllowUserToAddRows = false;
            this.dataGridViewMapping.AllowUserToResizeRows = false;
            this.dataGridViewMapping.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewMapping.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMapping.Location = new System.Drawing.Point(3, 46);
            this.dataGridViewMapping.Name = "dataGridViewMapping";
            this.dataGridViewMapping.RowHeadersVisible = false;
            this.dataGridViewMapping.Size = new System.Drawing.Size(888, 151);
            this.dataGridViewMapping.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnArrange);
            this.panel1.Controls.Add(this.checkMapping);
            this.panel1.Controls.Add(this.checkOTS);
            this.panel1.Controls.Add(this.checkOPC);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnReverse);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(888, 29);
            this.panel1.TabIndex = 0;
            // 
            // btnArrange
            // 
            this.btnArrange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArrange.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArrange.Location = new System.Drawing.Point(530, 3);
            this.btnArrange.Name = "btnArrange";
            this.btnArrange.Size = new System.Drawing.Size(70, 23);
            this.btnArrange.TabIndex = 8;
            this.btnArrange.Text = "Arrange";
            this.btnArrange.UseVisualStyleBackColor = true;
            this.btnArrange.Click += new System.EventHandler(this.btnArrange_Click);
            // 
            // checkMapping
            // 
            this.checkMapping.AutoSize = true;
            this.checkMapping.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkMapping.Location = new System.Drawing.Point(193, 6);
            this.checkMapping.Name = "checkMapping";
            this.checkMapping.Size = new System.Drawing.Size(59, 15);
            this.checkMapping.TabIndex = 7;
            this.checkMapping.Text = "Mapping";
            this.checkMapping.UseVisualStyleBackColor = true;
            // 
            // checkOTS
            // 
            this.checkOTS.AutoSize = true;
            this.checkOTS.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkOTS.Location = new System.Drawing.Point(128, 6);
            this.checkOTS.Name = "checkOTS";
            this.checkOTS.Size = new System.Drawing.Size(54, 15);
            this.checkOTS.TabIndex = 6;
            this.checkOTS.Text = "UniSim";
            this.checkOTS.UseVisualStyleBackColor = true;
            // 
            // checkOPC
            // 
            this.checkOPC.AutoSize = true;
            this.checkOPC.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkOPC.Location = new System.Drawing.Point(74, 6);
            this.checkOPC.Name = "checkOPC";
            this.checkOPC.Size = new System.Drawing.Size(45, 15);
            this.checkOPC.TabIndex = 5;
            this.checkOPC.Text = "OPC";
            this.checkOPC.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Location = new System.Drawing.Point(3, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(70, 23);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(601, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(70, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(672, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(70, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnReverse
            // 
            this.btnReverse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReverse.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReverse.Location = new System.Drawing.Point(743, 3);
            this.btnReverse.Name = "btnReverse";
            this.btnReverse.Size = new System.Drawing.Size(70, 23);
            this.btnReverse.TabIndex = 1;
            this.btnReverse.Text = "Reverse R/W";
            this.btnReverse.UseVisualStyleBackColor = true;
            this.btnReverse.Click += new System.EventHandler(this.btnReverse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(814, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 262);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(894, 10);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(450, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(10, 262);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // groupBoxOPC
            // 
            this.groupBoxOPC.Controls.Add(this.dataGridViewOPC);
            this.groupBoxOPC.Controls.Add(this.treeViewAdvOPC);
            this.groupBoxOPC.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBoxOPC.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxOPC.Location = new System.Drawing.Point(0, 0);
            this.groupBoxOPC.Name = "groupBoxOPC";
            this.groupBoxOPC.Size = new System.Drawing.Size(450, 262);
            this.groupBoxOPC.TabIndex = 5;
            this.groupBoxOPC.TabStop = false;
            this.groupBoxOPC.Text = "OPC";
            // 
            // dataGridViewOPC
            // 
            this.dataGridViewOPC.AllowUserToAddRows = false;
            this.dataGridViewOPC.AllowUserToResizeRows = false;
            this.dataGridViewOPC.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewOPC.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewOPC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOPC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOPC.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewOPC.Name = "dataGridViewOPC";
            this.dataGridViewOPC.ReadOnly = true;
            this.dataGridViewOPC.RowHeadersVisible = false;
            this.dataGridViewOPC.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOPC.Size = new System.Drawing.Size(444, 242);
            this.dataGridViewOPC.TabIndex = 5;
            // 
            // treeViewAdvOPC
            // 
            this.treeViewAdvOPC.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdvOPC.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewAdvOPC.DefaultToolTipProvider = null;
            this.treeViewAdvOPC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAdvOPC.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdvOPC.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdvOPC.Location = new System.Drawing.Point(3, 17);
            this.treeViewAdvOPC.Model = null;
            this.treeViewAdvOPC.Name = "treeViewAdvOPC";
            this.treeViewAdvOPC.NodeControls.Add(this.nodeIcon1);
            this.treeViewAdvOPC.NodeControls.Add(this.nodeTextBox1);
            this.treeViewAdvOPC.SelectedNode = null;
            this.treeViewAdvOPC.Size = new System.Drawing.Size(444, 242);
            this.treeViewAdvOPC.TabIndex = 3;
            this.treeViewAdvOPC.Text = "treeViewAdv1";
            this.treeViewAdvOPC.Visible = false;
            this.treeViewAdvOPC.DoubleClick += new System.EventHandler(this.treeViewAdv1_DoubleClick);
            // 
            // nodeIcon1
            // 
            this.nodeIcon1.DataPropertyName = "Image";
            this.nodeIcon1.LeftMargin = 1;
            this.nodeIcon1.ParentColumn = null;
            this.nodeIcon1.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Text";
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = null;
            // 
            // FormMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 472);
            this.Controls.Add(this.groupBoxUniSim);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.groupBoxOPC);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBoxMapping);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMapping";
            this.Text = "Edit Mapping";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMapping_FormClosing);
            this.groupBoxUniSim.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOTS)).EndInit();
            this.groupBoxMapping.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMapping)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBoxOPC.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOPC)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxUniSim;
        private System.Windows.Forms.GroupBox groupBoxMapping;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.GroupBox groupBoxOPC;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkMapping;
        private System.Windows.Forms.CheckBox checkOTS;
        private System.Windows.Forms.CheckBox checkOPC;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnReverse;
        private System.Windows.Forms.Button btnOK;
        private Aga.Controls.Tree.TreeViewAdv treeViewAdvTag;
        private Aga.Controls.Tree.TreeViewAdv treeViewAdvOPC;
        private System.Windows.Forms.DataGridView dataGridViewOTS;
        private System.Windows.Forms.DataGridView dataGridViewMapping;
        private System.Windows.Forms.DataGridView dataGridViewOPC;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon2;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private System.Windows.Forms.Button btnArrange;

    }
}