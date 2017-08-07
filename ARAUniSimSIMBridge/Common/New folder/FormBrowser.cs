using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using OpcCom;
using Opc;
using ARAUniSimSIMBridge.Data;
using Opc.Da;
using System.Drawing.Drawing2D;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace ARAUniSimSIMBridge
{
    public partial class FormBrowser : Form
    {
        private static FormBrowser _instance = null;
        public static FormBrowser Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new FormBrowser();
                }
                return _instance;
            }
        }
         

        private TreeModel treemodelOPC = null;
        private TreeModel treemodelTag = null;

        private UniSimDesign.Flowsheet flowsheet = null;


        private List<OPCServer> OPCServerList = null;
        public System.Data.DataTable OTSTable = null;
        public System.Data.DataTable OPCTable = null;
        public List<MappingData> MappingList = null;

        public List<string> opcLocalServers = null;

        public bool ResultOK { get; set; }

        private PrivateController CurrentController = null;


        public FormBrowser()
        {
            InitializeComponent();
        }

        private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void FormBrowser_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.UpdateTable();
                this.ResultOK = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.I))
            {
                if (this.treeViewAdvOPC.Visible)
                {
                    this.treeViewAdvOPC.Visible = false;
                    this.treeViewAdvOPC.Dock = DockStyle.Right;
                    this.treeViewAdvTag.Visible = false;
                    this.treeViewAdvTag.Dock = DockStyle.Left;

                    this.gridControlOPC.Visible = true;
                    this.gridControlOTS.Visible = true;
                }
                else
                {
                    this.treeViewAdvOPC.Visible = true;
                    this.treeViewAdvOPC.Dock = DockStyle.Fill;
                    this.treeViewAdvTag.Visible = true;
                    this.treeViewAdvTag.Dock = DockStyle.Fill;

                    this.gridControlOPC.Visible = false;
                    this.gridControlOTS.Visible = false;
                }
            }
            else if (keyData == Keys.Escape)
            {
                bool flag = true;
                if (this.gridViewMapping.GetSelectedCells().Count() > 0)
                {
                    this.gridViewMapping.ClearSelection();
                    flag = false;
                }

                if (this.gridViewOPC.SelectedRowsCount > 0)
                {
                    this.gridViewOPC.ClearSelection();
                    flag = false;
                }

                if (this.gridViewOTS.SelectedRowsCount > 0)
                {
                    this.gridViewOTS.ClearSelection();
                    flag = false;
                }

                if (flag)
                    this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        public bool isSet = false;
        public void SetBrowser()
        {
            if (isSet == false)
            {
                gridViewOPC.Appearance.FocusedRow.BackColor = Color.FromArgb(95, 171, 233);
                gridViewOPC.Appearance.SelectedRow.BackColor = Color.FromArgb(95, 171, 233);
                gridViewOPC.Appearance.HideSelectionRow.BackColor = Color.FromArgb(95, 171, 233);

                gridViewOTS.Appearance.FocusedRow.BackColor = Color.FromArgb(95, 171, 233);
                gridViewOTS.Appearance.SelectedRow.BackColor = Color.FromArgb(95, 171, 233);
                gridViewOTS.Appearance.HideSelectionRow.BackColor = Color.FromArgb(95, 171, 233);

                gridViewMapping.Appearance.FocusedRow.BackColor = Color.FromArgb(95, 171, 233);
                gridViewMapping.Appearance.SelectedRow.BackColor = Color.FromArgb(95, 171, 233);
                gridViewMapping.Appearance.HideSelectionRow.BackColor = Color.FromArgb(95, 171, 233);

                this.flowsheet = CommonController.Instance.flowsheet;
                this.OPCServerList = CommonController.Instance.OPCServerList;
                this.treemodelOPC = CommonController.Instance.TreemodelOPC;
                this.treemodelTag = CommonController.Instance.TreemodelTag;
                this.treeViewAdvOPC.Model = CommonController.Instance.TreemodelOPC;
                this.treeViewAdvTag.Model = CommonController.Instance.TreemodelTag;
                this.treeViewAdvTag.ExpandAll();
                this.treeViewAdvOPC.ExpandAll();

                this.opcLocalServers = CommonController.Instance.OPCLocalServerNames;

                OPCTable = new DataTable();
                //OPCTable.Columns.Add("Server", typeof(string));
                OPCTable.Columns.Add("Type", typeof(System.Type));
                OPCTable.Columns.Add("Name", typeof(string));
                OPCTable.Columns.Add("Access", typeof(string));

                OTSTable = new DataTable();
                OTSTable.Columns.Add("Sheet", typeof(string));
                OTSTable.Columns.Add("Type", typeof(string));
                OTSTable.Columns.Add("Name", typeof(string));
                OTSTable.Columns.Add("Param", typeof(string));
                OTSTable.Columns.Add("Unit", typeof(string));
                OTSTable.Columns.Add("Value", typeof(double));


                this.gridControlOPC.DataSource = OPCTable;
                this.gridControlOTS.DataSource = OTSTable;

                this.MappingList = new List<MappingData>();
                //this.dataGridViewOTS.DataSource = Controller.Instance.HYSYSTagList;
                //this.dataGridViewOPC.DataSource = this.OPCItemlist;
                isSet = true;
            }
        }
        public void SetController(PrivateController pc)
        {
            this.CurrentController = pc;

            this.groupControlOPC.Text = string.Format("{0} Tags", pc.OPCServerName);
            this.Text = string.Format("Edit Mapping - {0}", pc.OPCServerName);
            this.OPCServerList = CommonController.Instance.GetOPCServers(pc);
        }


        public void UpdateOTSTable()
        {
            this.OTSTable.Rows.Clear();

            for (int i = 0; i < CommonController.Instance.OTSTagList.Count; i++)
            {
                OTSTagData hd = CommonController.Instance.OTSTagList[i];
                this.OTSTable.Rows.Add(new object[] { hd.Sheet, hd.Type, hd.TagName, hd.Parameter, hd.Unit, hd.value });
            }
            this.gridControlOTS.DataSource = OTSTable;
            this.gridViewOTS.BestFitColumns();
        }
        public void UpdateOPCTable()
        {
            this.OPCTable.Rows.Clear();

            for (int maini = 0; maini < this.OPCServerList.Count; maini++)
            {
                OPCServer osg = this.OPCServerList[maini];

                for (int i = 0; i < osg.OPCItemlist.Count; i++)
                {
                    Opc.Da.BrowseElement hd = osg.OPCItemlist[i];


                    System.Type type = null;
                    string access = string.Empty;
                    for (int j = 0; j < hd.Properties.Length; j++)
                    {
                        if (hd.Properties[j].Description.Equals("Item Value", StringComparison.CurrentCultureIgnoreCase))
                        {
                            type = hd.Properties[j].DataType;
                        }
                        else if (hd.Properties[j].Description.Equals("Item Access Rights", StringComparison.CurrentCultureIgnoreCase))
                        {
                            access = hd.Properties[j].Value.ToString();
                        }
                    }

                    OPCTable.Rows.Add(new object[] { type, hd.ItemName, access });
                }
            }
            this.gridControlOPC.DataSource = OPCTable;
            this.gridViewOPC.BestFitColumns();
        }
        public void UpdateMappingTable()
        {
            if (this.CurrentController == null) return;

            this.MappingList.Clear();
            for (int i = 0; i < this.CurrentController.MappingList.Count; i++)
            {
                this.MappingList.Add(this.CurrentController.MappingList[i].Clone() as MappingData);
            }

            this.gridControlMapping.DataSource = this.MappingList;
            this.gridViewMapping.BestFitColumns();
        }

        public void UpdateTable()
        {
            this.UpdateOTSTable();
            this.UpdateOPCTable();
            this.UpdateMappingTable();

            for (int i = 0; i < this.gridViewMapping.Columns.Count; i++)
            {
                this.gridViewMapping.Columns[i].AppearanceCell.Font = new Font("Tahoma", 7);
            }
            for (int i = 0; i < this.gridViewOPC.Columns.Count; i++)
            {
                this.gridViewOPC.Columns[i].AppearanceCell.Font = new Font("Tahoma", 7);
            }
            for (int i = 0; i < this.gridViewOTS.Columns.Count; i++)
            {
                this.gridViewOTS.Columns[i].AppearanceCell.Font = new Font("Tahoma", 7);
            }
        }



        public List<TreeNodeAdv> GetOPCNodelist()
        {
            return this.treeViewAdvOPC.AllNodes.ToList();
        }

        private void treeViewAdv1_DoubleClick(object sender, EventArgs e)
        {
            TreeNodeAdv selected = treeViewAdvOPC.SelectedNode;
            if (selected == null) return;

            Node svrnode = selected.Tag as Node;
            if (svrnode.Tag is Opc.Server)
            {
                Opc.Server opcserver = svrnode.Tag as Opc.Server;
                Opc.Da.Server daserver = (Opc.Da.Server)opcserver;
                if (svrnode.IsChecked == false)
                {
                    if (daserver.IsConnected == false)
                    {
                        try
                        {
                            daserver.Connect();
                            OPCServer newserver = new OPCServer(this.CurrentController);
                            newserver.SetServer(daserver);
                            this.OPCServerList.Add(newserver);

                            CommonController.Instance.BrowseChildren(newserver, daserver, svrnode.Nodes, null);

                            //svrnode.Image = this.imageList1.Images[3];
                            svrnode.Image = Properties.Resources.server_link;
                            selected.ExpandAll();
                            selected.Collapse();
                        }
                        catch
                        {
                            if (daserver.IsConnected) daserver.Disconnect();
                            //svrnode.Image = this.imageList1.Images[10];
                            svrnode.Image = Properties.Resources.server_error;
                            svrnode.IsChecked = true;
                        }
                    }
                }
            }


        }

        private void treeViewAdv1_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            /*
            return;
            TreeNodeAdv selected = e.Node;
            if (selected == null) return;

            Node node = selected.Tag as Node;
            if (node == null) return;

            if (node.Tag is Opc.Da.BrowseElement)
            {
                Opc.Da.BrowseElement el = node.Tag as Opc.Da.BrowseElement;
                try
                {
                    if (node.Nodes.Count == 1 && node.Nodes[0].Text == "blank")
                    {
                        node.Nodes.RemoveAt(0);
                        if (el.HasChildren)
                        {
                            Opc.Server myserver = this.FindServerNode(node.Parent);
                            BrowseChildren((Opc.Da.Server)myserver, node.Nodes, el.ItemName);
                        }
                    }

                }
                catch (Exception ex)
                {
                    //hyContainer.Trace(ex.StackTrace, false);
                }
            }*/
        }

        private Opc.Server FindServerNode(Node node)
        {
            Opc.Server result = null;

            if (node.Tag is Opc.Server)
            {
                Opc.Server sd = node.Tag as Opc.Server;
                for (int i = 0; i < this.OPCServerList.Count; i++)
                {
                    if (this.OPCServerList[i].Server.Name == sd.Name)
                    {
                        result = this.OPCServerList[i].Server;
                        return result;
                    }
                }
            }
            else
            {
                if (node.Parent != null)
                    result = this.FindServerNode(node.Parent);
            }

            return result;
        }



        private void btnMappingRowDelete_Click(object sender, EventArgs e)
        {

            DevExpress.XtraGrid.Views.Base.GridCell[] selectedcells = this.gridViewMapping.GetSelectedCells();


            List<int> selectedrows = selectedcells.Select(x => x.RowHandle).Distinct().ToList();

            for (int i = 0; i < selectedrows.Count; i++)
            {
                selectedrows[i] = this.gridViewMapping.GetDataSourceRowIndex(selectedrows[i]);
            }

            selectedrows.Sort();

            for (int i = selectedrows.Count - 1; i >= 0; i--)
            {
                this.MappingList.RemoveAt(selectedrows[i]);
            }

            this.gridControlMapping.RefreshDataSource();
        }

        private void btnMappingOK_Click(object sender, EventArgs e)
        {
            this.CurrentController.MappingList.Clear();
            for (int i = 0; i < this.MappingList.Count; i++)
            {
                this.CurrentController.MappingList.Add(this.MappingList[i].Clone() as MappingData);
            }
            ResultOK = true;
            this.Hide();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (checkEditMapping.Checked)
                {
                    string path = string.Format("{0}\\ARASIMBridge_Mapping_{1}.csv", fbd.SelectedPath, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    this.gridControlMapping.ExportToCsv(path);
                }

                if (checkEditOPC.Checked)
                {
                    string path = string.Format("{0}\\ARASIMBridge_OPC_{1}.csv", fbd.SelectedPath, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    this.gridControlOPC.ExportToCsv(path);
                }

                if (checkEditOTS.Checked)
                {
                    string path = string.Format("{0}\\ARASIMBridge_OTS_{1}.csv", fbd.SelectedPath, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    this.gridControlOTS.ExportToCsv(path);
                }

            }
        }

        private void btnAddMapping_Click(object sender, EventArgs e)
        {
            if (this.gridViewOTS.SelectedRowsCount == 0 || this.gridViewOPC.SelectedRowsCount == 0)
                return;

            int length = 0;
            if (this.gridViewOPC.SelectedRowsCount > this.gridViewOTS.SelectedRowsCount) // 적게 선택한쪽을 따른다.
            {
                length = this.gridViewOTS.SelectedRowsCount;
            }
            else
            {
                length = this.gridViewOPC.SelectedRowsCount;
            }


            int[] selectedots = this.gridViewOTS.GetSelectedRows();
            int[] selectedopc = this.gridViewOPC.GetSelectedRows();

            /*
            for (int i = 0; i < selectedots.Length; i++)
            {
                selectedots[i] = this.gridViewOTS.GetDataSourceRowIndex(selectedots[i]);

                this.gridViewOTS.GetRowCellValue(
            }

            for (int i = 0; i < selectedopc.Length; i++)
            {
                selectedopc[i] = this.gridViewOPC.GetDataSourceRowIndex(selectedopc[i]);
            }

            Array.Sort(selectedots);
            Array.Sort(selectedopc);*/


            for (int i = 0; i < length; i++)
            {
                MappingData md = new MappingData();

                md.FromType = "HYSYS";
                md.FromName = string.Format("{0}.{1}",
                    this.gridViewOTS.GetRowCellValue(selectedots[i], this.gridViewOTS.Columns["Name"]).ToString(),
                this.gridViewOTS.GetRowCellValue(selectedots[i], this.gridViewOTS.Columns["Param"]).ToString());


                md.ToType = this.CurrentController.OPCServerName;
                //md.ToDataType = this.gridViewOPC.GetRowCellValue(selectedopc[i], this.gridViewOPC.Columns["Type"]) as System.Type;
                md.ToName = this.gridViewOPC.GetRowCellValue(selectedopc[i], this.gridViewOPC.Columns["Name"]).ToString();


                this.MappingList.Add(md);
            }

            this.gridControlMapping.RefreshDataSource();
        }

        private void btnMappingExchange_Click(object sender, EventArgs e)
        {
            //DevExpress.XtraGrid.Views.Base.GridCell[] selectedcell = this.gridViewMapping.GetSelectedCells();

            //List<int> selectdRow = selectedcell.Select(x => x.RowHandle).Distinct().ToList();


            int[] selectedmapping = this.gridViewMapping.GetSelectedRows();


            for (int i = 0; i < selectedmapping.Length; i++)
            {
                selectedmapping[i] = this.gridViewMapping.GetDataSourceRowIndex(selectedmapping[i]);

                MappingData md = this.MappingList[selectedmapping[i]];

                string temp = md.ToType;
                md.ToType = md.FromType;
                md.FromType = temp;

                temp = md.ToName;
                md.ToName = md.FromName;
                md.FromName = temp;
            }
            this.gridControlMapping.RefreshDataSource();
        }




    }
}
