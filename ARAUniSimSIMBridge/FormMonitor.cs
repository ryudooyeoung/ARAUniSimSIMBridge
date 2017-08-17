using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARAUniSimSIMBridge.Data;
using OpcCom;
using Opc;
using Opc.Da;
using System.Drawing.Drawing2D;
namespace ARAUniSimSIMBridge
{
    /// <summary>
    /// 데이터 교환 속도와 매핑 정보를 출력
    /// </summary>
    public partial class FormMonitor : Form
    {
        private static FormMonitor _instance = null;
        /// <summary>
        /// 싱글톤 instance
        /// </summary>
        public static FormMonitor Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new FormMonitor();
                }
                return _instance;
            }
        }


        private List<OPCServer> myOPCServers = null;
        private List<OTSDataTable> myReadDTs = null;
        private List<OTSDataTable> myWriteDTs = null;

        private List<float> elapsedTimes = null;
        private List<int> accessTimes = null;
        private List<float> gapTimes = null;


        private PrivateController CurrentController = null;

        /// <summary>
        /// 생성자
        /// </summary>
        public FormMonitor()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Monitor 기능 준비, 해당 Controller의 Data 교환 수행 속도와 Mapping 정보를 준비한다.
        /// </summary>
        /// <param name="controller"></param>
        public void SetMonitor(PrivateController controller)
        {
            this.CurrentController = controller;


            this.Text = this.CurrentController.UniqueID;

            this.elapsedTimes = this.CurrentController.ElapsedTimes;
            this.accessTimes = this.CurrentController.AccessTimes;
            this.gapTimes = this.CurrentController.GapTimes;
            this.myOPCServers = CommonController.Instance.GetOPCServers(this.CurrentController);
            this.myReadDTs = CommonController.Instance.GetOTSReadDatatables(this.CurrentController);
            this.myWriteDTs = CommonController.Instance.GetOTSWriteDatatables(this.CurrentController);


            this.toolStripComboBoxOPCServer.Items.Clear();
            for (int i = 0; i < CommonController.Instance.Controllers.Count; i++)
            {
                this.toolStripComboBoxOPCServer.Items.Add(CommonController.Instance.Controllers[i].UniqueID);
            }
        }



        private void toolStripComboBoxOPCServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetMonitor(CommonController.Instance.Controllers[toolStripComboBoxOPCServer.SelectedIndex]);
            this.RefreshMonitor();
            this.Text = this.CurrentController.UniqueID;
        }

        private void FormMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        /// <summary>
        /// 화면 갱신.
        /// </summary>
        public void RefreshMonitor()
        {
            if (this.Visible)
                this.panelMonitor.Invalidate();
        }


        private void PrintServerList(Graphics g)
        {
            try
            {
                /* string txtttt = string.Format("{0} {1} {2}", this.myOPCServers.Count, this.myReadDTs.Count, this.myWriteDTs.Count);
                 g.DrawString(txtttt, this.tooltipFont, Brushes.Black, 500, 10);
                 */
                int gy = 0;
                for (int i = 0; i < this.myReadDTs.Count; i++)
                {
                    OTSDataTable odt = this.myReadDTs[i];
                    g.DrawString(i + ", " + odt.ToString(), this.tooltipFont, Brushes.Blue, 600, gy);
                    gy += 13;

                    for (int j = 0; j < odt.TagNames.Length; j++)
                    {
                        string txt = string.Format("{0,15:0.000} {1}", odt.TagValues[j], odt.TagNames[j]);
                        //string txt = string.Format("{0} ", odt.TagNames[j]);
                        g.DrawString(txt, this.tooltipFont, Brushes.Black, 615, gy);
                        gy += 13;
                    }

                }
                for (int i = 0; i < this.myWriteDTs.Count; i++)
                {
                    OTSDataTable odt = this.myWriteDTs[i];
                    g.DrawString(i + ", " + odt.ToString(), this.tooltipFont, Brushes.Blue, 600, gy);
                    gy += 13;

                    for (int j = 0; j < odt.TagNames.Length; j++)
                    {
                        string txt = string.Format("{0,15:0.000} {1}", odt.TagValues[j], odt.TagNames[j]);
                        //string txt = string.Format("{0}", odt.TagNames[j]);
                        g.DrawString(txt, this.tooltipFont, Brushes.Black, 615, gy);
                        gy += 13;
                    }
                }



                gy = 0;
                for (int i = 0; i < this.myOPCServers.Count; i++)
                {
                    OPCServer osg = this.myOPCServers[i];
                    g.DrawString(osg.ToString(), this.tooltipFont, Brushes.Blue, 0, gy);
                    gy += 13;


                    if (osg.CommandSubscription != null)
                    {
                        OPCSubscription os = osg.CommandSubscription;

                        string txt = string.Format("{0} {1} {2} {3} {4} {5}", os.Type, os.Name, os.ConnectedDataTableIndex, os.ConnectedDataTableName, os.ConnectedServerName, os.ConnectedSubscriptionName);

                        g.DrawString(txt, this.tooltipFont, Brushes.Black, 15, gy);
                        gy += 13;
                        for (int k = 0; k < os.Subscription.Items.Length; k++)
                        {
                            txt = string.Format("{0}", os.Subscription.Items[k].ItemName);
                            g.DrawString(txt, this.tooltipFont, Brushes.Black, 30, gy);
                            gy += 13;
                        }
                    }


                    for (int j = 0; j < osg.ReadSubscriptions.Count; j++)
                    {
                        OPCSubscription os = osg.ReadSubscriptions[j];
                        string txt = string.Format("{0} {1} {2} {3} {4} {5} {6}", j, os.Type, os.Name, os.ConnectedDataTableIndex, os.ConnectedDataTableName, os.ConnectedServerName, os.ConnectedSubscriptionName);

                        g.DrawString(txt, this.tooltipFont, Brushes.Black, 15, gy);
                        gy += 13;
                        for (int k = 0; k < os.Subscription.Items.Length; k++)
                        {
                            //txt = string.Format("{0}", os.Subscription.Items[k].ItemName); 
                            txt = string.Format("[{0,10}] {1,15:0.000} {2}", os.ItemTypes[k].Name, os.ItemValues[k].Value, os.Subscription.Items[k].ItemName);

                            g.DrawString(txt, this.tooltipFont, Brushes.Black, 30, gy);
                            gy += 13;
                        }
                    }

                    for (int j = 0; j < osg.WriteSubscriptions.Count; j++)
                    {
                        OPCSubscription os = osg.WriteSubscriptions[j];
                        string txt = string.Format("{0} {1} {2} {3} {4} {5} {6}", j, os.Type, os.Name, os.ConnectedDataTableIndex, os.ConnectedDataTableName, os.ConnectedServerName, os.ConnectedSubscriptionName);
                        g.DrawString(txt, this.tooltipFont, Brushes.Black, 15, gy);
                        gy += 13;
                        for (int k = 0; k < os.Subscription.Items.Length; k++)
                        {
                            //txt = string.Format("{0}", os.Subscription.Items[k].ItemName);
                            txt = string.Format("[{0,10}] {1,15:0.000} {2}", os.ItemTypes[k].Name, os.ItemValues[k].Value, os.Subscription.Items[k].ItemName);

                            g.DrawString(txt, this.tooltipFont, Brushes.Black, 30, gy);
                            gy += 13;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonController.Instance.PrintLog(ex.StackTrace);
            }
        }

        private Font tooltipFont = new Font("Consolas", 8);
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle drawarea = new Rectangle(0, 0, this.panelMonitor.Width, this.panelMonitor.Height);
            float hh = this.panelMonitor.Height * 0.333f;
            using (SolidBrush bab = new SolidBrush(Color.FromArgb(100, Color.Black)))
            {
                g.FillRectangle(bab, drawarea);
            }

            if (this.toolStripMenuItemElapsedTime.Checked)
            {
                this.PrintServerList(g);
                return;
            }

            if (this.elapsedTimes.Count <= 2) return;

            try
            {
                List<float> mdata = this.elapsedTimes;


                float mxmax = mdata.Count;
                float mxmin = 0;
                float mxdistance = (drawarea.Width - 10) / (mxmax - mxmin);

                float mymax = mdata.Max();
                float mymin = mdata.Min();
                //float mymax = 600, mymin = 0;
                float mydistance = (hh - 10) / (mymax - mymin);

                using (Pen temp = new Pen(Color.WhiteSmoke, 1))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        PointF[] points = new PointF[mdata.Count];
                        for (int i = 0; i < mdata.Count; i++)
                        {
                            float x = (i * mxdistance) - (mxmin * mxdistance) + 5;
                            float y = 0 + (hh - (mdata[i] - mymin) * mydistance) - 5;
                            points[i] = new PointF(x, y);
                        }
                        path.AddLines(points);
                        g.DrawPath(temp, path);

                        using (StringFormat strf = new StringFormat())
                        {
                            strf.LineAlignment = StringAlignment.Far;
                            strf.Alignment = StringAlignment.Far;
                            g.DrawString(string.Format("execute elapsed {0:0}", mdata[mdata.Count - 1]), tooltipFont, Brushes.Black, this.panelMonitor.Width, hh - 13, strf);

                            strf.LineAlignment = StringAlignment.Near;
                            strf.Alignment = StringAlignment.Near;
                            g.DrawString(string.Format("{0}", (long)mymax), tooltipFont, Brushes.White, 0, 5, strf);

                            strf.LineAlignment = StringAlignment.Far;
                            strf.Alignment = StringAlignment.Near;
                            g.DrawString(string.Format("{0}", (long)mymin), tooltipFont, Brushes.White, 0, hh - 5, strf);
                        }
                    }
                }


                List<int> tdata = this.accessTimes;

                float txmax = tdata.Count;
                float txmin = 0;
                float txdistance = (drawarea.Width - 10) / (txmax - txmin);

                float tymax = tdata.Max();
                float tymin = tdata.Min();
                //float tymax = 600, tymin = 0;
                if (tymax == tymin)
                {
                    tymax++;
                    tymin--;
                }
                float tydistance = (hh - 10) / (tymax - tymin);

                using (Pen temp = new Pen(Color.Pink, 1))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        PointF[] points = new PointF[tdata.Count];
                        for (int i = 0; i < tdata.Count; i++)
                        {
                            float x = (i * txdistance) - (txmin * txdistance) + 5;
                            float y = hh + (hh - (tdata[i] - tymin) * tydistance) - 5;
                            points[i] = new PointF(x, y);
                        }
                        path.AddLines(points);
                        g.DrawPath(temp, path);

                        using (StringFormat strf = new StringFormat())
                        {
                            strf.LineAlignment = StringAlignment.Far;
                            strf.Alignment = StringAlignment.Far;
                            g.DrawString(string.Format("{0}", tdata[tdata.Count - 1]), tooltipFont, Brushes.Black, this.panelMonitor.Width, hh + hh - 13, strf);

                            strf.LineAlignment = StringAlignment.Near;
                            strf.Alignment = StringAlignment.Near;
                            g.DrawString(string.Format("{0}", (long)tymax), tooltipFont, Brushes.Pink, 0, hh + 5, strf);

                            strf.LineAlignment = StringAlignment.Far;
                            strf.Alignment = StringAlignment.Near;
                            g.DrawString(string.Format("{0}", (long)tymin), tooltipFont, Brushes.Pink, 0, hh + hh - 5, strf);
                        }
                    }
                }



                List<float> cdata = this.gapTimes;

                float cxmax = cdata.Count;
                float cxmin = 0;
                float cxdistance = (drawarea.Width - 10) / (cxmax - cxmin);

                float cymax = cdata.Max();
                float cymin = cdata.Min();
                //float cymax = 600, cymin = 0;
                if (cymax == cymin)
                {
                    cymax++;
                    cymin--;
                }
                float cydistance = (hh - 10) / (cymax - cymin);

                using (Pen temp = new Pen(Color.Gold, 1))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        PointF[] points = new PointF[cdata.Count];
                        for (int i = 0; i < cdata.Count; i++)
                        {
                            float x = (i * cxdistance) - (cxmin * cxdistance) + 5;
                            float y = hh + hh + (hh - (cdata[i] - cymin) * cydistance) - 5;
                            points[i] = new PointF(x, y);
                        }
                        path.AddLines(points);
                        g.DrawPath(temp, path);

                        using (StringFormat strf = new StringFormat())
                        {
                            strf.LineAlignment = StringAlignment.Far;
                            strf.Alignment = StringAlignment.Far;
                            g.DrawString(string.Format("1cycle elapsed {0}", cdata[cdata.Count - 1]), tooltipFont, Brushes.Black, this.panelMonitor.Width, hh + hh + hh - 13, strf);

                            strf.LineAlignment = StringAlignment.Near;
                            strf.Alignment = StringAlignment.Near;
                            g.DrawString(string.Format("{0}", (long)cymax), tooltipFont, Brushes.Gold, 0, hh + hh + 5, strf);

                            strf.LineAlignment = StringAlignment.Far;
                            strf.Alignment = StringAlignment.Near;
                            g.DrawString(string.Format("{0}", (long)cymin), tooltipFont, Brushes.Gold, 0, hh + hh + hh - 5, strf);
                        }
                    }
                }
            }
            catch  
            {
            }
        }

        private void panelMonitor_MouseDown(object sender, MouseEventArgs e)
        {
            this.RefreshMonitor();
        }


        private void toolStripMenuItemServerInfo_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.myOPCServers.Count; i++)
                {
                    OPCServer osg = this.myOPCServers[i];

                    CommonController.Instance.PrintLog(osg.Name);

                    for (int j = 0; j < osg.ReadSubscriptions.Count; j++)
                    {
                        OPCSubscription os = osg.ReadSubscriptions[j];
                        CommonController.Instance.PrintLog(os.Name);

                        if (os.ItemTypes == null) CommonController.Instance.PrintLog("itemtype null");
                        else
                        {
                            for (int k = 0; k < os.ItemTypes.Length; k++)
                            {
                                CommonController.Instance.PrintLog(string.Format("  {0} {1} {2}", os.ItemTypes[k].Name, os.ItemValues[k], os.Subscription.Items[k].ItemName));
                            }
                        }
                    }

                    for (int j = 0; j < osg.WriteSubscriptions.Count; j++)
                    {
                        OPCSubscription os = osg.WriteSubscriptions[j];
                        CommonController.Instance.PrintLog(os.Name);
                        if (os.ItemTypes == null) CommonController.Instance.PrintLog("itemtype null");
                        else
                        {
                            for (int k = 0; k < os.ItemTypes.Length; k++)
                            {
                                CommonController.Instance.PrintLog(string.Format("  {0} {1} {2}", os.ItemTypes[k].Name, os.ItemValues[k], os.Subscription.Items[k].ItemName));
                            }
                        }
                    }
                    CommonController.Instance.PrintLog("===============");
                }
            }
            catch (Exception ex)
            {
                CommonController.Instance.PrintLog(ex.StackTrace);
            }
        }

        private void toolStripMenuItemElapsedTime_Click(object sender, EventArgs e)
        {
            toolStripMenuItemElapsedTime.Checked = !toolStripMenuItemElapsedTime.Checked;
            this.RefreshMonitor();
        }

    }
}
