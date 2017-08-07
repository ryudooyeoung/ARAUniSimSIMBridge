namespace ARAUniSimSIMBridge
{
    partial class FormBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBrowser));
            this.treeViewAdvOPC = new Aga.Controls.Tree.TreeViewAdv();
            this.nodeStateIcon1 = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeStateIcon2 = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.treeViewAdvTag = new Aga.Controls.Tree.TreeViewAdv();
            this.gridControlMapping = new DevExpress.XtraGrid.GridControl();
            this.gridViewMapping = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControlOTS = new DevExpress.XtraGrid.GridControl();
            this.gridViewOTS = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControlOPC = new DevExpress.XtraGrid.GridControl();
            this.gridViewOPC = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.groupControlOPC = new DevExpress.XtraEditors.GroupControl();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.splitterControl2 = new DevExpress.XtraEditors.SplitterControl();
            this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
            this.checkEditOPC = new DevExpress.XtraEditors.CheckEdit();
            this.checkEditOTS = new DevExpress.XtraEditors.CheckEdit();
            this.checkEditMapping = new DevExpress.XtraEditors.CheckEdit();
            this.btnMappingOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnMappingRowDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnMappingExchange = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddMapping = new DevExpress.XtraEditors.SimpleButton();
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMapping)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMapping)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlOTS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewOTS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlOPC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewOPC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControlOPC)).BeginInit();
            this.groupControlOPC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
            this.groupControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditOPC.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditOTS.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditMapping.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // treeViewAdvOPC
            // 
            this.treeViewAdvOPC.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdvOPC.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewAdvOPC.DefaultToolTipProvider = null;
            this.treeViewAdvOPC.Dock = System.Windows.Forms.DockStyle.Right;
            this.treeViewAdvOPC.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdvOPC.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdvOPC.Location = new System.Drawing.Point(339, 21);
            this.treeViewAdvOPC.Model = null;
            this.treeViewAdvOPC.Name = "treeViewAdvOPC";
            this.treeViewAdvOPC.NodeControls.Add(this.nodeStateIcon1);
            this.treeViewAdvOPC.NodeControls.Add(this.nodeTextBox1);
            this.treeViewAdvOPC.SelectedNode = null;
            this.treeViewAdvOPC.Size = new System.Drawing.Size(103, 338);
            this.treeViewAdvOPC.TabIndex = 2;
            this.treeViewAdvOPC.Text = "treeViewAdv1";
            this.treeViewAdvOPC.Visible = false;
            this.treeViewAdvOPC.DoubleClick += new System.EventHandler(this.treeViewAdv1_DoubleClick);
            // 
            // nodeStateIcon1
            // 
            this.nodeStateIcon1.DataPropertyName = "Image";
            this.nodeStateIcon1.LeftMargin = 1;
            this.nodeStateIcon1.ParentColumn = null;
            this.nodeStateIcon1.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Text";
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = null;
            // 
            // nodeStateIcon2
            // 
            this.nodeStateIcon2.DataPropertyName = "Image";
            this.nodeStateIcon2.LeftMargin = 1;
            this.nodeStateIcon2.ParentColumn = null;
            this.nodeStateIcon2.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "Text";
            this.nodeTextBox2.IncrementalSearchEnabled = true;
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = null;
            // 
            // treeViewAdvTag
            // 
            this.treeViewAdvTag.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdvTag.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewAdvTag.DefaultToolTipProvider = null;
            this.treeViewAdvTag.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeViewAdvTag.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdvTag.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdvTag.Location = new System.Drawing.Point(2, 21);
            this.treeViewAdvTag.Model = null;
            this.treeViewAdvTag.Name = "treeViewAdvTag";
            this.treeViewAdvTag.NodeControls.Add(this.nodeStateIcon2);
            this.treeViewAdvTag.NodeControls.Add(this.nodeTextBox2);
            this.treeViewAdvTag.SelectedNode = null;
            this.treeViewAdvTag.Size = new System.Drawing.Size(99, 338);
            this.treeViewAdvTag.TabIndex = 3;
            this.treeViewAdvTag.Text = "treeViewAdv2";
            this.treeViewAdvTag.Visible = false;
            // 
            // gridControlMapping
            // 
            this.gridControlMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControlMapping.Location = new System.Drawing.Point(5, 53);
            this.gridControlMapping.MainView = this.gridViewMapping;
            this.gridControlMapping.Name = "gridControlMapping";
            this.gridControlMapping.Size = new System.Drawing.Size(915, 221);
            this.gridControlMapping.TabIndex = 9;
            this.gridControlMapping.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMapping});
            // 
            // gridViewMapping
            // 
            this.gridViewMapping.GridControl = this.gridControlMapping;
            this.gridViewMapping.Name = "gridViewMapping";
            this.gridViewMapping.OptionsFind.AlwaysVisible = true;
            this.gridViewMapping.OptionsFind.FindDelay = 100;
            this.gridViewMapping.OptionsSelection.MultiSelect = true;
            this.gridViewMapping.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gridViewMapping.OptionsView.ShowGroupPanel = false;
            // 
            // gridControlOTS
            // 
            this.gridControlOTS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlOTS.Location = new System.Drawing.Point(101, 21);
            this.gridControlOTS.MainView = this.gridViewOTS;
            this.gridControlOTS.Name = "gridControlOTS";
            this.gridControlOTS.Size = new System.Drawing.Size(373, 338);
            this.gridControlOTS.TabIndex = 10;
            this.gridControlOTS.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewOTS});
            // 
            // gridViewOTS
            // 
            this.gridViewOTS.GridControl = this.gridControlOTS;
            this.gridViewOTS.Name = "gridViewOTS";
            this.gridViewOTS.OptionsBehavior.Editable = false;
            this.gridViewOTS.OptionsBehavior.ReadOnly = true;
            this.gridViewOTS.OptionsFind.AlwaysVisible = true;
            this.gridViewOTS.OptionsFind.FindDelay = 100;
            this.gridViewOTS.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewOTS.OptionsSelection.MultiSelect = true;
            this.gridViewOTS.OptionsView.ShowGroupPanel = false;
            // 
            // gridControlOPC
            // 
            this.gridControlOPC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlOPC.Location = new System.Drawing.Point(2, 21);
            this.gridControlOPC.MainView = this.gridViewOPC;
            this.gridControlOPC.Name = "gridControlOPC";
            this.gridControlOPC.Size = new System.Drawing.Size(337, 338);
            this.gridControlOPC.TabIndex = 11;
            this.gridControlOPC.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewOPC});
            // 
            // gridViewOPC
            // 
            this.gridViewOPC.GridControl = this.gridControlOPC;
            this.gridViewOPC.Name = "gridViewOPC";
            this.gridViewOPC.OptionsBehavior.Editable = false;
            this.gridViewOPC.OptionsBehavior.ReadOnly = true;
            this.gridViewOPC.OptionsFind.AlwaysVisible = true;
            this.gridViewOPC.OptionsFind.FindDelay = 100;
            this.gridViewOPC.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewOPC.OptionsSelection.MultiSelect = true;
            this.gridViewOPC.OptionsSelection.UseIndicatorForSelection = false;
            this.gridViewOPC.OptionsView.ShowGroupPanel = false;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.gridControlOTS);
            this.groupControl1.Controls.Add(this.treeViewAdvTag);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(449, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(476, 361);
            this.groupControl1.TabIndex = 10;
            this.groupControl1.Text = "UniSim Tags";
            // 
            // groupControlOPC
            // 
            this.groupControlOPC.Controls.Add(this.gridControlOPC);
            this.groupControlOPC.Controls.Add(this.treeViewAdvOPC);
            this.groupControlOPC.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupControlOPC.Location = new System.Drawing.Point(0, 0);
            this.groupControlOPC.Name = "groupControlOPC";
            this.groupControlOPC.Size = new System.Drawing.Size(444, 361);
            this.groupControlOPC.TabIndex = 15;
            this.groupControlOPC.Text = "OPC";
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(444, 0);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(5, 361);
            this.splitterControl1.TabIndex = 16;
            this.splitterControl1.TabStop = false;
            // 
            // splitterControl2
            // 
            this.splitterControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterControl2.Location = new System.Drawing.Point(0, 361);
            this.splitterControl2.Name = "splitterControl2";
            this.splitterControl2.Size = new System.Drawing.Size(925, 5);
            this.splitterControl2.TabIndex = 17;
            this.splitterControl2.TabStop = false;
            // 
            // groupControl3
            // 
            this.groupControl3.Controls.Add(this.checkEditOPC);
            this.groupControl3.Controls.Add(this.checkEditOTS);
            this.groupControl3.Controls.Add(this.checkEditMapping);
            this.groupControl3.Controls.Add(this.btnMappingOK);
            this.groupControl3.Controls.Add(this.btnMappingRowDelete);
            this.groupControl3.Controls.Add(this.btnMappingExchange);
            this.groupControl3.Controls.Add(this.btnAddMapping);
            this.groupControl3.Controls.Add(this.btnExport);
            this.groupControl3.Controls.Add(this.gridControlMapping);
            this.groupControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupControl3.Location = new System.Drawing.Point(0, 366);
            this.groupControl3.Name = "groupControl3";
            this.groupControl3.Size = new System.Drawing.Size(925, 279);
            this.groupControl3.TabIndex = 18;
            this.groupControl3.Text = "Mapping List";
            // 
            // checkEditOPC
            // 
            this.checkEditOPC.Location = new System.Drawing.Point(143, 27);
            this.checkEditOPC.Name = "checkEditOPC";
            this.checkEditOPC.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.checkEditOPC.Properties.Appearance.Options.UseFont = true;
            this.checkEditOPC.Properties.Caption = "OPC";
            this.checkEditOPC.Size = new System.Drawing.Size(45, 19);
            this.checkEditOPC.TabIndex = 21;
            // 
            // checkEditOTS
            // 
            this.checkEditOTS.Location = new System.Drawing.Point(86, 27);
            this.checkEditOTS.Name = "checkEditOTS";
            this.checkEditOTS.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.checkEditOTS.Properties.Appearance.Options.UseFont = true;
            this.checkEditOTS.Properties.Caption = "UniSim";
            this.checkEditOTS.Size = new System.Drawing.Size(58, 19);
            this.checkEditOTS.TabIndex = 20;
            // 
            // checkEditMapping
            // 
            this.checkEditMapping.Location = new System.Drawing.Point(194, 27);
            this.checkEditMapping.Name = "checkEditMapping";
            this.checkEditMapping.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.checkEditMapping.Properties.Appearance.Options.UseFont = true;
            this.checkEditMapping.Properties.Caption = "Mapping";
            this.checkEditMapping.Size = new System.Drawing.Size(75, 19);
            this.checkEditMapping.TabIndex = 19;
            // 
            // btnMappingOK
            // 
            this.btnMappingOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMappingOK.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.btnMappingOK.Appearance.Options.UseFont = true;
            this.btnMappingOK.Location = new System.Drawing.Point(845, 24);
            this.btnMappingOK.Name = "btnMappingOK";
            this.btnMappingOK.Size = new System.Drawing.Size(75, 23);
            this.btnMappingOK.TabIndex = 18;
            this.btnMappingOK.Text = "OK";
            this.btnMappingOK.Click += new System.EventHandler(this.btnMappingOK_Click);
            // 
            // btnMappingRowDelete
            // 
            this.btnMappingRowDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMappingRowDelete.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.btnMappingRowDelete.Appearance.Options.UseFont = true;
            this.btnMappingRowDelete.Location = new System.Drawing.Point(683, 24);
            this.btnMappingRowDelete.Name = "btnMappingRowDelete";
            this.btnMappingRowDelete.Size = new System.Drawing.Size(75, 23);
            this.btnMappingRowDelete.TabIndex = 17;
            this.btnMappingRowDelete.Text = "Delete";
            this.btnMappingRowDelete.Click += new System.EventHandler(this.btnMappingRowDelete_Click);
            // 
            // btnMappingExchange
            // 
            this.btnMappingExchange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMappingExchange.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.btnMappingExchange.Appearance.Options.UseFont = true;
            this.btnMappingExchange.Location = new System.Drawing.Point(764, 24);
            this.btnMappingExchange.Name = "btnMappingExchange";
            this.btnMappingExchange.Size = new System.Drawing.Size(75, 23);
            this.btnMappingExchange.TabIndex = 16;
            this.btnMappingExchange.Text = "Reverse R/W";
            this.btnMappingExchange.Click += new System.EventHandler(this.btnMappingExchange_Click);
            // 
            // btnAddMapping
            // 
            this.btnAddMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddMapping.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.btnAddMapping.Appearance.Options.UseFont = true;
            this.btnAddMapping.Location = new System.Drawing.Point(602, 24);
            this.btnAddMapping.Name = "btnAddMapping";
            this.btnAddMapping.Size = new System.Drawing.Size(75, 23);
            this.btnAddMapping.TabIndex = 15;
            this.btnAddMapping.Text = "Add";
            this.btnAddMapping.Click += new System.EventHandler(this.btnAddMapping_Click);
            // 
            // btnExport
            // 
            this.btnExport.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.btnExport.Appearance.Options.UseFont = true;
            this.btnExport.Location = new System.Drawing.Point(5, 24);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 14;
            this.btnExport.Text = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);



            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 645);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.groupControlOPC);
            this.Controls.Add(this.splitterControl2);
            this.Controls.Add(this.groupControl3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Mapping";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBrowser_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FormBrowser_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMapping)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMapping)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlOTS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewOTS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlOPC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewOPC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControlOPC)).EndInit();
            this.groupControlOPC.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
            this.groupControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEditOPC.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditOTS.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditMapping.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv treeViewAdvOPC;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon2;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
        private Aga.Controls.Tree.TreeViewAdv treeViewAdvTag;
        private DevExpress.XtraGrid.GridControl gridControlMapping;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMapping;
        private DevExpress.XtraGrid.GridControl gridControlOTS;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewOTS;
        private DevExpress.XtraGrid.GridControl gridControlOPC;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewOPC;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.GroupControl groupControlOPC;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraEditors.SplitterControl splitterControl2;
        private DevExpress.XtraEditors.GroupControl groupControl3;
        private DevExpress.XtraEditors.SimpleButton btnExport;
        private DevExpress.XtraEditors.SimpleButton btnMappingOK;
        private DevExpress.XtraEditors.SimpleButton btnMappingRowDelete;
        private DevExpress.XtraEditors.SimpleButton btnMappingExchange;
        private DevExpress.XtraEditors.SimpleButton btnAddMapping;
        private DevExpress.XtraEditors.CheckEdit checkEditOPC;
        private DevExpress.XtraEditors.CheckEdit checkEditOTS;
        private DevExpress.XtraEditors.CheckEdit checkEditMapping;
    }
}