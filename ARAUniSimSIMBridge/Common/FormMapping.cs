using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using ARAUniSimSIMBridge.Data;
using System.IO;

namespace ARAUniSimSIMBridge
{
    /// <summary>
    /// mapping list 편집창
    /// </summary>
    public partial class FormMapping : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private static FormMapping _instance = null;

        /// <summary>
        /// 싱글톤 instance
        /// </summary>
        public static FormMapping Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new FormMapping();
                }
                return _instance;
            }
        }

        /// <summary>
        /// singltone 방식이기에 dialogresult를 쓸수 없으므로 bool변수로 대체
        /// </summary>
        public bool ResultOK { get; set; }


        private TreeModel treemodelOPC = null;
        private TreeModel treemodelTag = null;

        private UniSimDesign.Flowsheet flowsheet = null;


        private List<OPCServer> OPCServerList = null;
        private System.Data.DataTable OTSTable = null;
        private System.Data.DataTable OPCTable = null;
        private List<MappingData> MappingList = null;

        private List<string> opcLocalServers = null;

        private PrivateController CurrentController = null;


        /// <summary>
        /// 생성자
        /// </summary>
        public FormMapping()
        {
            InitializeComponent();
        }


        /// <summary>
        /// mapping form 준비
        /// </summary>
        public void SetBrowser()
        {

            this.flowsheet = CommonController.Instance.flowsheet;
            this.OPCServerList = CommonController.Instance.OPCServerList;
            this.treemodelOPC = CommonController.Instance.TreemodelOPC;
            this.treemodelTag = CommonController.Instance.TreemodelTag;

            treeViewAdvOPC.SuspendLayout();
            this.treeViewAdvOPC.Model = CommonController.Instance.TreemodelOPC;
            this.treeViewAdvOPC.ExpandAll();
            treeViewAdvOPC.ResumeLayout();

            treeViewAdvTag.SuspendLayout();
            this.treeViewAdvTag.Model = CommonController.Instance.TreemodelTag;
            this.treeViewAdvTag.ExpandAll();
            treeViewAdvTag.ResumeLayout();


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

            this.MappingList = new List<MappingData>();
            //this.dataGridViewOTS.DataSource = Controller.Instance.HYSYSTagList;
            //this.dataGridViewOPC.DataSource = this.OPCItemlist;

            this.dataGridViewOTS.Font = new Font("Tahoma", 7);
            this.dataGridViewOPC.Font = new Font("Tahoma", 7);
            this.dataGridViewMapping.Font = new Font("Tahoma", 7);

        }

        /// <summary>
        /// Extension controller에 맞게 내용 구성
        /// </summary>
        /// <param name="pc">Extension controller</param>
        public void SetController(PrivateController pc)
        {
            this.CurrentController = pc;

            this.groupBoxOPC.Text = string.Format("{0} Tags", pc.OPCServerName);
            this.Text = string.Format("Edit Mapping - {0}", pc.OPCServerName);
            this.OPCServerList = CommonController.Instance.GetOPCServers(pc);
            this.ResultOK = false;
        }

        private void FormMapping_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        /// <summary>
        /// show dialog 에서 ctrl+i 로 데이터 교환속도 조회 \n
        /// esc 키로는 창을 닫는다.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.I))
            {
                if (this.treeViewAdvOPC.Visible)
                {
                    this.treeViewAdvOPC.Visible = false;
                    this.treeViewAdvTag.Visible = false;

                    this.dataGridViewOPC.Visible = true;
                    this.dataGridViewOTS.Visible = true;
                }
                else
                {
                    this.treeViewAdvOPC.Visible = true;
                    this.treeViewAdvTag.Visible = true; ;

                    this.dataGridViewOPC.Visible = false;
                    this.dataGridViewOTS.Visible = false;
                }
            }
            else if (keyData == Keys.Escape)
            {
                bool flag = true;
                if (this.dataGridViewMapping.SelectedCells.Count > 0)
                {
                    this.dataGridViewMapping.ClearSelection();
                    flag = false;
                }

                if (this.dataGridViewOPC.SelectedRows.Count > 0)
                {
                    this.dataGridViewOPC.ClearSelection();
                    flag = false;
                }

                if (this.dataGridViewOTS.SelectedRows.Count > 0)
                {
                    this.dataGridViewOTS.ClearSelection();
                    flag = false;
                }

                if (flag)
                    this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void UpdateOTSTable()
        {
            try
            {
                this.dataGridViewOTS.DataSource = null;
                this.OTSTable.Rows.Clear();

                for (int i = 0; i < CommonController.Instance.OTSTagList.Count; i++)
                {
                    OTSTagData hd = CommonController.Instance.OTSTagList[i];
                    this.OTSTable.Rows.Add(new object[] { hd.Sheet, hd.Type, hd.TagName, hd.Parameter, hd.Unit, hd.value });
                }
                this.dataGridViewOTS.DataSource = OTSTable;

                for (int i = 0; i < this.dataGridViewOTS.Columns.Count; i++)
                {
                    this.dataGridViewOTS.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    if (i == this.dataGridViewOTS.Columns.Count - 1)
                        this.dataGridViewOTS.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch { }
        }

        private void UpdateOPCTable()
        {
            try
            {
                this.dataGridViewOPC.DataSource = null;
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
                this.dataGridViewOPC.DataSource = OPCTable;



                for (int i = 0; i < this.dataGridViewOPC.Columns.Count; i++)
                {
                    this.dataGridViewOPC.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    if (i == this.dataGridViewOPC.Columns.Count - 1)
                        this.dataGridViewOPC.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; ;
                }
            }
            catch { }
        }


        private void UpdateMappingTable()
        {
            try
            {
                if (this.CurrentController == null) return;

                this.dataGridViewMapping.DataSource = null;
                this.MappingList.Clear();

                for (int i = 0; i < this.CurrentController.MappingList.Count; i++)
                {
                    this.MappingList.Add(this.CurrentController.MappingList[i].Clone() as MappingData);
                }
                if (this.MappingList.Count > 0)
                    this.dataGridViewMapping.DataSource = this.MappingList;


                for (int i = 0; i < this.dataGridViewMapping.Columns.Count; i++)
                {
                    this.dataGridViewMapping.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch { }
        }

        /// <summary>
        /// Mapping list 갱신
        /// </summary>
        public void UpdateTable()
        {
            this.UpdateOTSTable();
            this.UpdateOPCTable();
            this.UpdateMappingTable();
        }


        private void SetMappingColumns()
        {
            for (int i = 0; i < this.dataGridViewMapping.Columns.Count; i++)
            {
                this.dataGridViewMapping.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
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

                            CommonController.Instance.BrowseChildren(newserver, svrnode.Nodes, null);

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



        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewOTS.SelectedRows.Count == 0 || this.dataGridViewOPC.SelectedRows.Count == 0)
                return;

            int length = 0;
            if (this.dataGridViewOPC.SelectedRows.Count > this.dataGridViewOTS.SelectedRows.Count) // 적게 선택한쪽을 따른다.
            {
                length = this.dataGridViewOTS.SelectedRows.Count;
            }
            else
            {
                length = this.dataGridViewOPC.SelectedRows.Count;
            }


            DataGridViewSelectedRowCollection selectedots = this.dataGridViewOTS.SelectedRows;
            DataGridViewSelectedRowCollection selectedopc = this.dataGridViewOPC.SelectedRows;


            this.dataGridViewMapping.DataSource = null;
            try
            {
                for (int i = 0; i < length; i++)
                {
                    MappingData md = new MappingData();

                    string otsname = this.dataGridViewOTS["Name", selectedots[i].Index].Value.ToString();
                    string otsparam = this.dataGridViewOTS["Param", selectedots[i].Index].Value.ToString();
                    string opctag = this.dataGridViewOPC["Name", selectedopc[i].Index].Value.ToString();


                    md.FromType = "HYSYS";
                    md.FromName = string.Format("{0}.{1}", otsname, otsparam);

                    md.ToType = this.CurrentController.OPCServerName;
                    md.ToName = string.Format("{0}", opctag);

                    this.MappingList.Add(md);
                }
            }
            catch { }


            this.dataGridViewMapping.DataSource = this.MappingList;
            this.SetMappingColumns();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            DataGridViewSelectedCellCollection selectedcells = this.dataGridViewMapping.SelectedCells;

            List<int> indexs = new List<int>();
            for (int i = 0; i < selectedcells.Count; i++)
            {
                indexs.Add(selectedcells[i].RowIndex);
            }
            indexs = indexs.Distinct().ToList();
            indexs.Sort();

            this.dataGridViewMapping.DataSource = null;
            for (int i = indexs.Count - 1; i >= 0; i--)
            {
                this.MappingList.RemoveAt(indexs[i]);
            }

            this.dataGridViewMapping.DataSource = this.MappingList;
            this.SetMappingColumns();
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection selectedcells = this.dataGridViewMapping.SelectedCells;

            List<int> indexs = new List<int>();
            for (int i = 0; i < selectedcells.Count; i++)
            {
                indexs.Add(selectedcells[i].RowIndex);
            }
            indexs = indexs.Distinct().ToList();

            for (int i = 0; i < indexs.Count; i++)
            {
                int idx = indexs[i];
                MappingData md = this.MappingList[idx];

                string temp = md.ToType;
                md.ToType = md.FromType;
                md.FromType = temp;

                temp = md.ToName;
                md.ToName = md.FromName;
                md.FromName = temp;
            }
            this.dataGridViewMapping.DataSource = null;
            this.dataGridViewMapping.DataSource = this.MappingList;
            this.SetMappingColumns();

            for (int i = 0; i < indexs.Count; i++)
            {
                this.dataGridViewMapping.Rows[indexs[i]].Selected = true;
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.CurrentController.MappingList.Clear();
            for (int i = 0; i < this.MappingList.Count; i++)
            {
                this.CurrentController.MappingList.Add(this.MappingList[i].Clone() as MappingData);
            }
            this.ResultOK = true;
            this.Hide();
        }



        private void btnArrange_Click(object sender, EventArgs e)
        {
            this.groupBoxOPC.Width = (this.Width - 10) / 2;

            this.groupBoxMapping.Height = (this.Height - 10 - 24) / 3;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (checkMapping.Checked)
                {
                    string path = string.Format("{0}\\ARASIMBridge_Mapping_{1}.csv", fbd.SelectedPath, DateTime.Now.ToString("yyyyMMddhhmmss"));

                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
                    StreamWriter tw = new StreamWriter(fs);	// over-write

                    tw.WriteLine("FromType,FromName,ToType,ToName");

                    StringBuilder strb = new StringBuilder();
                    foreach (MappingData acc in MappingList)
                    {
                        strb.Append(acc.FromType).Append(",");
                        strb.Append(acc.FromName).Append(",");
                        strb.Append(acc.ToType).Append(",");
                        strb.Append(acc.ToName);
                        tw.WriteLine(strb.ToString());
                        strb.Length = 0;
                    }
                    tw.Close();
                    fs.Close();
                }

                if (checkOPC.Checked)
                {
                    string path = string.Format("{0}\\ARASIMBridge_OPC_{1}.csv", fbd.SelectedPath, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
                    StreamWriter tw = new StreamWriter(fs);	// over-write

                    tw.WriteLine("Type,Name,Access");

                    StringBuilder strb = new StringBuilder();
                    foreach (DataRow acc in OPCTable.Rows)
                    {
                        strb.Append(acc["Type"]).Append(",");
                        strb.Append(acc["Name"]).Append(",");
                        strb.Append(acc["Access"]);
                        tw.WriteLine(strb.ToString());
                        strb.Length = 0;
                    }
                    tw.Close();
                    fs.Close();
                }

                if (checkOTS.Checked)
                {
                    string path = string.Format("{0}\\ARASIMBridge_OTS_{1}.csv", fbd.SelectedPath, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
                    StreamWriter tw = new StreamWriter(fs);	// over-write

                    tw.WriteLine("Sheet,Type,Name", "Param", "Unit");

                    StringBuilder strb = new StringBuilder();
                    foreach (DataRow acc in OTSTable.Rows)
                    {
                        strb.Append(acc["Sheet"]).Append(",");
                        strb.Append(acc["Type"]).Append(",");
                        strb.Append(acc["Name"]).Append(",");
                        strb.Append(acc["Param"]).Append(",");
                        strb.Append(acc["Unit"]);
                        tw.WriteLine(strb.ToString());
                        strb.Length = 0;
                    }
                    tw.Close();
                    fs.Close();
                }

            }
        }
    }
}
