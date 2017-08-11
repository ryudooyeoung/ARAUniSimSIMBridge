using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ARAUniSimSIMBridge.Data;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using Aga.Controls.Tree;
using OpcCom;
using Opc;
using System.Net.Sockets;
using UniSimDesign;
using Microsoft.Win32;
using System.Linq.Expressions;

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using Opc.Da;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ARAUniSimSIMBridge
{
    public enum extensionStatus
    {
        Disconnected,
        Connected,
        hasError,
        Paused,
        Running,
    }
    public class PrivateController
    {
        public List<MappingData> MappingList { get; set; }

        public InternalTextFlexVariable txtLocalServers { get; set; }
        public InternalTextFlexVariable txtNetworkServers { get; set; }
        private InternalTextVariable txtUniqueID;
        private InternalTextVariable txtCreateDate;


        /// <summary>
        /// opc server 선택 구분
        /// </summary>
        private InternalRealVariable dblOPCServerSelected;

        /// <summary>
        /// 현재 선택한 network opc server
        /// </summary>
        private InternalTextVariable txtNetworkServerSelected;
        /// <summary>
        /// 현재 선택한 local opc server
        /// </summary>
        private InternalTextVariable txtLocalServerSelected;


        //olga opc execute
        public InternalTextVariable txtOLGASnapshot { get; set; }
        public InternalTextVariable txtOLGAModel { get; set; }
        private InternalTextVariable txtOLGAExecutable;

        //mapping list
        private InternalTextFlexVariable txtFromTypes;
        private InternalTextFlexVariable txtFromNames;
        private InternalTextFlexVariable txtToTypes;
        private InternalTextFlexVariable txtToNames;

        //olga opc server is running
        public InternalRealVariable dblConnectedOPC { get; set; }
        public InternalRealVariable dblRunningSim { get; set; }


        //배속
        public InternalRealVariable dblOLGARunInterval { get; set; }
        public InternalRealVariable dblRealTimeFactor { get; set; }
        public double preRealTimeFactor { get; set; }


        //상태표시
        public InternalTextVariable txtOLGAMessage { get; set; }
        public InternalTextVariable txtSimStatus { get; set; }
        public InternalRealVariable dblSimStatus { get; set; }



        public InternalTextFlexVariable txtCPUClockTimes { get; set; }
        public InternalTextFlexVariable txtUniSimTimes { get; set; }
        public InternalTextFlexVariable txtOLGATimes { get; set; }




        public Thread threadDataExchange { get; set; }

        public extensionStatus status = extensionStatus.Disconnected;


        private string MappingPath = string.Empty;

        /// <summary>
        /// 실제로 데이터 ots-opc 데이터 교환을 할 수 있는지.
        /// </summary>
        public bool IsStartDataExchange { get; set; }
        public bool IsApplyMapping { get; set; }
        public bool IsTerminated { get; set; }

        public string UniqueID
        {
            get
            {
                return this.txtUniqueID.Value;
            }
            set
            {
                this.txtUniqueID.Value = value;
            }
        }

        public PrivateController()
        {
            this.IsTerminated = false;
            this.MappingList = new List<MappingData>();
        }


        private ExtnUnitOperationContainer hyContainer;
        public void SetContainer(ExtnUnitOperationContainer hyContainer)
        {
            this.hyContainer = hyContainer;

            this.txtOLGAExecutable = (InternalTextVariable)hyContainer.FindVariable("txtOLGAExecutable").Variable;

            this.txtNetworkServers = (InternalTextFlexVariable)hyContainer.FindVariable("txtNetworkServers").Variable;
            this.txtLocalServers = (InternalTextFlexVariable)hyContainer.FindVariable("txtLocalServers").Variable;

            this.txtCPUClockTimes = (InternalTextFlexVariable)hyContainer.FindVariable("txtCPUClockTimes").Variable;
            this.txtUniSimTimes = (InternalTextFlexVariable)hyContainer.FindVariable("txtUniSimTimes").Variable;
            this.txtUniSimTimes.SetBounds(3);
            this.txtCPUClockTimes.SetBounds(3);



            this.txtCreateDate = (InternalTextVariable)hyContainer.FindVariable("txtCreateDate").Variable;
            this.txtUniqueID = (InternalTextVariable)hyContainer.FindVariable("txtUniqueID").Variable;

            this.dblOPCServerSelected = (InternalRealVariable)hyContainer.FindVariable("dblOPCServerSelected").Variable;
            this.dblOPCServerSelected.Value = 20;


            this.txtNetworkServerSelected = (InternalTextVariable)hyContainer.FindVariable("txtNetworkServerSelected").Variable;
            this.txtLocalServerSelected = (InternalTextVariable)hyContainer.FindVariable("txtLocalServerSelected").Variable;

            this.txtOLGASnapshot = (InternalTextVariable)hyContainer.FindVariable("txtOLGASnapshot").Variable;
            this.txtOLGAModel = (InternalTextVariable)hyContainer.FindVariable("txtOLGAModel").Variable;



            this.txtFromTypes = (InternalTextFlexVariable)hyContainer.FindVariable("txtFromTypes").Variable;
            this.txtFromNames = (InternalTextFlexVariable)hyContainer.FindVariable("txtFromNames").Variable;
            this.txtToTypes = (InternalTextFlexVariable)hyContainer.FindVariable("txtToTypes").Variable;
            this.txtToNames = (InternalTextFlexVariable)hyContainer.FindVariable("txtToNames").Variable;

            this.txtSimStatus = (InternalTextVariable)hyContainer.FindVariable("txtSimStatus").Variable;
            this.dblSimStatus = (InternalRealVariable)hyContainer.FindVariable("dblSimStatus").Variable;

            this.dblConnectedOPC = (InternalRealVariable)hyContainer.FindVariable("dblConnectedOPC").Variable;
            this.dblRunningSim = (InternalRealVariable)hyContainer.FindVariable("dblRunningSim").Variable;

            this.txtOLGATimes = (InternalTextFlexVariable)hyContainer.FindVariable("txtOLGATimes").Variable;
            this.txtOLGATimes.SetBounds(3);

            this.dblOLGARunInterval = (InternalRealVariable)hyContainer.FindVariable("dblOLGARunInterval").Variable;
            this.dblRealTimeFactor = (InternalRealVariable)hyContainer.FindVariable("dblRealTimeFactor").Variable;
            this.SetStatus(extensionStatus.Disconnected); //초기설정


            this.txtOLGAMessage = (InternalTextVariable)hyContainer.FindVariable("txtOLGAMessage").Variable;

            //this.myPlotName = (InternalTextVariable)hyContainer.FindVariable("plotName").Variable;
            //this.executeData = (InternalRealFlexVariable)hyContainer.FindVariable("executeData").Variable;
            //this.timeData = (InternalRealFlexVariable)hyContainer.FindVariable("timeData").Variable;
            //this.CreatePlot();


            CommonController.Instance.RegisterController(this);
            CommonController.Instance.SetContainer(hyContainer);

            this.txtLocalServers.SetBounds(CommonController.Instance.OPCLocalServerNames.Count);
            this.txtLocalServers.Values = CommonController.Instance.OPCLocalServerNames.ToArray();
        }



        public void SelectLocalServer()
        {
            //CommonController.Instance.PrintLog(txtLocalServerSelected.Value + ", " + this.GetOPCServerName());
            if (this.txtLocalServerSelected.Value.Equals("OLGA OPCServer"))
            {
                this.dblOPCServerSelected.Value = 20;
            }
            else
            {
                this.dblOPCServerSelected.Value = 22;
            }
        }

        public void SelectNetworkServer()
        {

        }



        private string GetOLGAMessage()
        {
            string result = string.Empty;

            List<OPCServer> myOPCServers = CommonController.Instance.GetOPCServers(this);
            for (int i = 0; i < myOPCServers.Count; i++)
            {
                OPCServer osg = myOPCServers[i];
                if (osg.Server.Name.StartsWith("SPT."))
                {
                    OPCSubscription os = osg.CommandSubscription;
                    Opc.Da.Subscription cg = os.Subscription; //cmd
                    os.ItemValues = cg.Read(cg.Items);

                    result = (string)os.ItemValues[7].Value; ;
                    break;
                }
            }

            return result;
        }

        public bool SaveOLGASnapshot(string path)
        {
            bool result = false;
            List<OPCServer> myOPCServers = CommonController.Instance.GetOPCServers(this);
            for (int i = 0; i < myOPCServers.Count; i++)
            {
                OPCServer osg = myOPCServers[i];
                if (osg.Server.Name.StartsWith("SPT."))
                {
                    OPCSubscription os = osg.CommandSubscription; //cmd
                    Opc.Da.Subscription cg = os.Subscription; //cmd

                    Opc.Da.ItemValue file = new Opc.Da.ItemValue(cg.Items[6]);
                    file.Value = path;

                    Opc.Da.ItemValue flag = new Opc.Da.ItemValue(cg.Items[5]);
                    flag.Value = true;
                    cg.Write(new Opc.Da.ItemValue[] { file, flag });

                    os.ItemValues = cg.Read(cg.Items);
                    while ((string)os.ItemValues[2].Value == "STATE_COMMAND")
                    {
                        Thread.Sleep(1);
                        os.ItemValues = cg.Read(cg.Items);
                    }

                    result = true;
                    break;
                }
            }

            return result;
        }

        public void LoadOLGASanpshot(string path)
        {
            List<OPCServer> myOPCServers = CommonController.Instance.GetOPCServers(this);
            for (int i = 0; i < myOPCServers.Count; i++)
            {
                OPCServer osg = myOPCServers[i];
                if (osg.Server.Name.StartsWith("SPT."))
                {
                    OPCSubscription os = osg.CommandSubscription;
                    Opc.Da.Subscription cg = os.Subscription; //cmd

                    Opc.Da.ItemValue file = new Opc.Da.ItemValue(cg.Items[4]);
                    file.Value = path;

                    Opc.Da.ItemValue flag = new Opc.Da.ItemValue(cg.Items[3]);
                    flag.Value = true;
                    cg.Write(new Opc.Da.ItemValue[] { file, flag });

                    os.ItemValues = cg.Read(cg.Items);
                    while ((string)os.ItemValues[2].Value == "STATE_COMMAND")
                    {
                        Thread.Sleep(1);
                        os.ItemValues = cg.Read(cg.Items);
                    }

                    break;
                }
            }

        }


        private string PathModelConfigXML = string.Empty;
        private string PathLeastmappingXML = string.Empty;
        public void SetBaseDocument()
        {
            if (string.IsNullOrEmpty(this.txtCreateDate.Value))
            {
                this.txtCreateDate.Value = DateTime.Now.ToString("yyyyMMddhhmmssff");//새로만들어짐
            }
            else
            {
                //기존에 있었음 
            }

            if (string.IsNullOrEmpty(this.txtUniqueID.Value))
            {
                this.txtUniqueID.Value = this.hyContainer.name;
            }

            this.PathModelConfigXML = string.Format("{0}\\{1}.xml", CommonController.Instance.PathModelWorkDirectory, this.txtUniqueID.Value);
            this.PathLeastmappingXML = string.Format("{0}\\{1}_Mapping.xml", CommonController.Instance.PathModelWorkDirectory, this.txtUniqueID.Value);
        }


        ////// set the config 
        public void TakeSnapshot()
        {
            if (this.status != extensionStatus.Connected) return;

            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                CommonController.Instance.simCase.SaveSnapshotFile(sfd.FileName);
                this.SaveOLGASnapshot(sfd.FileName);
            }
        }

        public void ShowMappingEditor()
        {
            FormMapping.Instance.SetBrowser();
            FormMapping.Instance.SetController(this);
            FormMapping.Instance.UpdateTable();

            FormMapping.Instance.ShowDialog();

            if (FormMapping.Instance.ResultOK)
            {
                this.IsApplyMapping = false; //ShowBrowser
                this.SetMappingTable(); //show browser
            }
        }

        public void ShowMonitor()
        {
            FormMonitor.Instance.SetMonitor(this);
            FormMonitor.Instance.Show();
        }






        //모듈 삭제시.
        public void RemoveData()
        {
            CommonController.Instance.UnregisterController(this);

            this.IsTerminated = true;
            this.IsStartDataExchange = false; //RemoveData

            CommonController.Instance.RemoveOPCServers(this);

            this.KillMyOLGA();

            this.threadDataExchange.Abort();
            this.threadDataExchange = null;
        }



        //맵핑 리스트 가져오기.
        public void LoadMappingList(string path)
        {
            bool isLoad = false;
            if (string.IsNullOrEmpty(path))
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "XML Files (.xml)|*.xml";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    isLoad = true;
                    path = ofd.FileName;
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    isLoad = true;
                }
            }


            if (isLoad)
            {
                this.MappingPath = path;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<MappingData>));
                using (StreamReader reader = new StreamReader(this.MappingPath))
                {
                    this.MappingList.Clear();
                    try
                    {
                        this.MappingList = (List<MappingData>)xmlSerializer.Deserialize(reader);
                        this.MappingList = this.MappingList.OrderBy(x => x.FromType).ThenBy(x => x.ToType).ToList();
                    }
                    catch
                    {
                        this.MappingList = new List<MappingData>();
                    }

                    this.SetMappingTable(); //load
                }
            }
            this.IsApplyMapping = false; //load
        }


        public void ResetMappingList()
        {
            this.IsApplyMapping = false; // reset
            this.MappingList.Clear();
            this.SetMappingTable();
        }

        public void SaveMappingList(bool autoSave = false)
        {
            string path = string.Empty;
            if (autoSave)
            {
                path = this.PathLeastmappingXML;
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "XML Files (.xml)|*.xml";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    path = sfd.FileName;
                }
            }


            if (string.IsNullOrEmpty(path) == false)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<MappingData>));
                using (StreamWriter reader = new StreamWriter(path))
                {
                    xmlSerializer.Serialize(reader, this.MappingList);
                }
            }
        }

        /// <summary>
        /// 설정환경 불러오기.
        /// </summary>
        public void LoadConfiguration()
        {
            //string doc = string.Format("{0}\\ARASIMBridge\\Config.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            this.txtOLGAExecutable.Value = CommonController.Instance.PathInstalledOLGAEXE;


            if (File.Exists(this.PathModelConfigXML))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
                using (StreamReader reader = new StreamReader(this.PathModelConfigXML))
                {
                    Configuration cf = (Configuration)xmlSerializer.Deserialize(reader);

                    this.txtOLGAModel.Value = cf.PathOLGAGenkey;
                    if (string.IsNullOrEmpty(cf.PathOLGAGenkey) == false)
                    {
                        this.GetModelINfoFromGenkey(cf.PathOLGAGenkey);
                    }

                    if (File.Exists(cf.PathOLGASnapshot))
                    {
                        this.txtOLGASnapshot.Value = cf.PathOLGASnapshot;
                    }

                    if (File.Exists(this.PathLeastmappingXML))
                    {
                        this.LoadMappingList(this.PathLeastmappingXML);
                    }


                    this.txtLocalServerSelected.Value = cf.OPCServerName;
                    if (cf.OPCServerName.StartsWith("SPT."))
                    {
                        this.dblOPCServerSelected.SetValue(20);
                    }
                    else
                    {
                        this.dblOPCServerSelected.SetValue(22);
                    }

                    this.dblRealTimeFactor.Value = cf.Foctor;
                    this.preRealTimeFactor = cf.Foctor;

                    this.dblOLGARunInterval.Value = cf.RunInterval;
                }
            }
        }




        public string OLGAModelName { get; set; }
        public string OPCServerName { get; set; }
        private void GetModelINfoFromGenkey(string path)
        {

            this.OPCServerName = "OLGAOPCServer";
            this.OLGAModelName = "ServerDemo";

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(fs);


            StringBuilder strb = new StringBuilder();
            string strLine;
            bool check = false;
            while ((strLine = reader.ReadLine()) != null)
            {
                if (Regex.IsMatch(strLine, "! .*"))
                {
                    if (strLine.Equals("! Global keywords", StringComparison.CurrentCultureIgnoreCase))
                    {
                        check = true;
                        continue;
                    }
                    else
                    {
                        check = false;
                    }
                }

                if (check)
                {
                    if (strLine.StartsWith("        "))
                    {
                        //이어붙기
                        strb.Append(strLine);
                    }
                    else
                    {
                        strb.AppendFormat("!@#{0}", strLine);
                    }
                }
            }
            fs.Close();
            reader.Close();


            string data = strb.ToString();
            data = Regex.Replace(data, "\\\\        ", "");

            string[] datas = Regex.Split(data, "!@#");
            for (int maini = 0; maini < datas.Length; maini++)
            {
                string line = datas[maini];

                if (line.StartsWith("SERVEROPTIONS"))
                {
                    string[] strarr = line.Split(',');
                    for (int i = 0; i < strarr.Length; i++)
                    {

                        if (strarr[i].Contains("SERVERNAME"))
                        {
                            string[] svrdata = strarr[i].Split('=');
                            if (svrdata.Length > 0)
                                this.OPCServerName = svrdata[1].Trim();
                        }

                        if (strarr[i].Contains("MODELNAME"))
                        {
                            string[] svrdata = strarr[i].Split('=');
                            if (svrdata.Length > 0)
                                this.OLGAModelName = svrdata[1].Trim();
                        }
                    }
                    break;
                }
            }

            this.OPCServerName = "SPT." + this.OPCServerName;

            //CommonController.Instance.PrintLog(this.OPCServerName);
            if (CommonController.Instance.CheckDuplicatedOLGAOPCServer(this))
            {
                CommonController.Instance.PrintLog(string.Format("OLGA OPC Server {0} is duplicated", this.OPCServerName));
                this.OPCServerName = string.Empty;
                this.OLGAModelName = string.Empty;
                this.txtOLGAModel.Value = string.Empty;
            }

        }


        public void SaveConfiguration()
        {
            Configuration cf = new Configuration()
            {
                PathOLGAGenkey = this.txtOLGAModel.Value,
                PathOLGASnapshot = this.txtOLGASnapshot.Value,
                PathMapping = this.MappingPath,
                RunInterval = (int)this.dblOLGARunInterval.Value,
                Foctor = this.dblRealTimeFactor.Value,
                OPCServerName = this.OPCServerName,
            };

            if (this.OPCServerName.StartsWith("SPT.") == false)
            {
                cf.PathOLGAGenkey = string.Empty;
                cf.PathOLGASnapshot = string.Empty;
            }

            //string doc = string.Format("{0}\\ARASIMBridge\\Config.xml", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            using (StreamWriter reader = new StreamWriter(this.PathModelConfigXML))
            {
                xmlSerializer.Serialize(reader, cf);
            }
        }



        private bool ApplyMappingTable()
        {
            bool noError = true;


            this.CheckMappingList();

            //find server name
            List<string> serverNames = (from query in MappingList.AsEnumerable()
                                        select query.FromType).ToList();

            List<string> serverNames2 = (from query in MappingList.AsEnumerable()
                                         select query.ToType).ToList();

            //서버이름 추리기.
            serverNames.AddRange(serverNames2);
            serverNames = serverNames.Distinct().ToList();
            serverNames.Sort();


            for (int i = 0; i < serverNames.Count; i++)
            {
                CommonController.Instance.ConnectOPCServer(serverNames[i], this);
            }

            //태그들이 올바른지 체크하기.
            if (CommonController.Instance.CheckMappingOPC(serverNames, this.MappingList) == false ||
                CommonController.Instance.CheckMappingOTS(serverNames, this.MappingList) == false)
            {
                noError = false;
                this.IsApplyMapping = false; //has error
            }
            else
            {
                if (CommonController.Instance.AddMappingOPCOTS(serverNames, this.MappingList, this) == false)
                {
                    noError = false;
                    this.IsApplyMapping = false; //ApplyMappingTable
                }
                else
                {
                    noError = true;
                    this.IsApplyMapping = true;
                }
            }

            return noError;
        }

        private void CheckMappingList()
        {
            for (int i = this.MappingList.Count - 1; i >= 0; i--)
            {
                MappingData md = this.MappingList[i];

                if (md.FromType.Equals(this.OPCServerName) == false && md.ToType.Equals(this.OPCServerName) == false)
                {
                    this.MappingList.RemoveAt(i);
                }
            }

            this.SetMappingTable();
        }

        private void SetMappingTable()
        {
            txtFromTypes.SetBounds(MappingList.Count);
            txtFromNames.SetBounds(MappingList.Count);
            txtToTypes.SetBounds(MappingList.Count);
            txtToNames.SetBounds(MappingList.Count);

            if (MappingList.Count == 0)
            {
                this.txtFromTypes.Values = new string[] { };
                this.txtFromNames.Values = new string[] { };
                this.txtToTypes.Values = new string[] { };
                this.txtToNames.Values = new string[] { };
            }
            else
            {

                string[] ftypes = new string[MappingList.Count];
                string[] fanmes = new string[MappingList.Count];
                string[] ttypes = new string[MappingList.Count];
                string[] tnames = new string[MappingList.Count];
                for (int i = 0; i < MappingList.Count; i++)
                {
                    ftypes[i] = MappingList[i].FromType;
                    fanmes[i] = MappingList[i].FromName;
                    ttypes[i] = MappingList[i].ToType;
                    tnames[i] = MappingList[i].ToName;
                }
                this.txtFromTypes.Values = ftypes;
                this.txtFromNames.Values = fanmes;
                this.txtToTypes.Values = ttypes;
                this.txtToNames.Values = tnames;
            }
        }



        /// <summary>
        /// 올가 실행파일 설정
        /// </summary>
        public void LoadOLGAExecutable()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "EXE Files (.exe)|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.txtOLGAExecutable.Value = ofd.FileName;
            }
        }
        public void LoadOLGASnapshot()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "RSW Files (.rsw)|*.rsw";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.txtOLGASnapshot.Value = ofd.FileName;
            }
        }

        /// <summary>
        /// 올가 genkey 파일 설정
        /// </summary>
        public void LoadOLGAModel()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Genkey Files (.genkey)|*.genkey";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.txtOLGAModel.Value = ofd.FileName;
                this.txtOLGASnapshot.Value = string.Empty;

                this.GetModelINfoFromGenkey(ofd.FileName); //load model
            }
        }



        public double PreType = -99;
        public bool ConnectOPCServer()
        {
            bool result = false;
            //CommonController.Instance.PrintLog(this.dblOPCServerSelected.Value + ", " + this.txtLocalServerSelected.Value);
            //CommonController.Instance.solver.CanSolve = false;
            //CommonController.Instance.integrator.Mode = UniSimDesign.IntegratorMode_enum.imManual;

            double type = this.dblOPCServerSelected.Value;
            if (type == 20) //olga
            {
                result = this.ConnectOPCOLGA();
            }
            else if (type == 22)
            {
                result = this.connectOPCOther();
            }

            if (PreType != type)
            {
                PreType = type;
                this.IsApplyMapping = false; // 재연결
            }


            return result;
        }


        public bool connectOPCOther()
        {
            bool result = false;
            if (this.txtLocalServerSelected.Value.Equals("Matrikon.OPC.Simulation"))
            {
                this.OPCServerName = "Matrikon.OPC.Simulation";
                this.OLGAModelName = string.Empty;

                if (CommonController.Instance.ConnectOPCServer(this.txtLocalServerSelected.Value, this))
                {
                    this.dblConnectedOPC.SetValue(1);
                    result = false;
                }
                else
                {
                    this.dblConnectedOPC.SetValue(0);
                    result = true;
                }
            }

            return result;
        }

        public bool ConnectOPCOLGA()
        {
            if (File.Exists(this.txtOLGAExecutable.Value) == false)
            {
                CommonController.Instance.PrintLog("Can not find the OLGA Executable, you should check the path");
                return false;
            }

            if (File.Exists(this.txtOLGAModel.Value) == false)
            {
                CommonController.Instance.PrintLog("Can not find the OLGA genkey, you should check the path");
                return false;
            }


            bool loadSnapshot = false;
            if (string.IsNullOrEmpty(this.txtOLGASnapshot.Value) == false)
            {
                if (File.Exists(this.txtOLGASnapshot.Value) == false) //올바른 경로가 아닌경우.
                {
                    CommonController.Instance.PrintLog("Can not find the OLGA Snapshot, you should check the path");
                    return false;
                }
                else
                {
                    loadSnapshot = true;
                }
            }
            else //없는경우
            {
                loadSnapshot = false;
            }


            if (this.IsApplyMapping == false && File.Exists(this.txtOLGAModel.Value))
            {
                this.GetModelINfoFromGenkey(this.txtOLGAModel.Value);
            }



            //현재 서버리스트 지우기
            CommonController.Instance.RemoveOPCServers(this); //execute에서 먼저 지우고 시작.


            if (this.ExecuteOLGA() == false) return false;


            //opc서버 확인 될때까지 
            while (true)
            {
                if (CommonController.Instance.FindOLGAOPCServer(this.OPCServerName) != null)
                {
                    //서버 목록 갱신
                    CommonController.Instance.GetOPCServerList();
                    break;
                }
                Thread.Sleep(10);
            }


            //olga opc 접속

            if (CommonController.Instance.ConnectOPCServer(this.OPCServerName, this))
            {
                if (loadSnapshot) this.LoadOLGASanpshot(this.txtOLGASnapshot.Value);
                this.dblConnectedOPC.SetValue(1);
            }
            else
            {
                this.KillMyOLGA();
                this.dblConnectedOPC.SetValue(0);
                return false;
            }

            return true;
        }


        public void DisconnectOPCServer()
        {
            if (CommonController.Instance.integrator.IsRunning) return;

            //현재 서버리스트 지우기
            CommonController.Instance.RemoveOPCServers(this); //shutdown

            this.KillMyOLGA();
            CommonController.Instance.GetOPCServerList();

            this.SetStatus(extensionStatus.Disconnected); //shutdown
            this.dblConnectedOPC.SetValue(0);
        }




        public void SetStatus(extensionStatus status)
        {
            if (status == extensionStatus.Disconnected)
            {
                this.status = extensionStatus.Disconnected;
                this.txtSimStatus.Value = "Not Connected to OPC Server";
                this.dblSimStatus.SetValue(7);

            }
            else if (status == extensionStatus.Connected)
            {
                this.status = extensionStatus.Connected;
                this.txtSimStatus.Value = string.Format("Connected to {0}", this.OPCServerName);
                this.dblSimStatus.SetValue(1);
            }
        }


        //olga 실행
        private Process OLGAProcess = null;
        private bool ExecuteOLGA()
        {
            this.OLGAProcess = Process.Start(this.txtOLGAExecutable.Value, string.Format("\"{0}\"", this.txtOLGAModel.Value));

            /*
            Process process = new Process();
            ProcessStartInfo cmd = new ProcessStartInfo();

            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.FileName = this.txtOLGAEXE.Value;
            cmd.Arguments = string.Format("\"{0}\"", this.txtOLGAModel.Value);
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = true;

            //cmd.RedirectStandardOutput = true;
            //cmd.RedirectStandardInput = true;
            //cmd.RedirectStandardError = true;

            process.StartInfo = cmd;
            process.Start();
            //string resultmsg = process.StandardOutput.ReadToEnd();
            //process.Close();*/

            return true;
        }

        public bool KillMyOLGA()
        {
            bool result = true;
            if (this.OLGAProcess != null)
            {
                try
                {
                    this.OLGAProcess.Kill();

                    Opc.Server[] servers2 = new OpcCom.ServerEnumerator().GetAvailableServers(Specification.COM_DA_20);

                    Process process = new Process();
                    ProcessStartInfo cmd = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = this.txtOLGAExecutable.Value,
                        CreateNoWindow = true,
                        UseShellExecute = true,
                    };

                    if (servers2 != null)
                    {
                        foreach (Opc.Server server in servers2)
                        {
                            if (server.Name.StartsWith("SPT."))
                            {

                                //Process.Start(this.txtOLGAExecutable.Value, "-unreg " + server.Name);

                                cmd.Arguments = string.Format("-unreg {0}", server.Name);
                                process.StartInfo = cmd;
                                process.Start();
                            }
                        }
                    }

                    /*Process process = new Process();
                    ProcessStartInfo cmd = new ProcessStartInfo();

                    cmd.WindowStyle = ProcessWindowStyle.Hidden;
                    cmd.FileName = this.txtOLGAExecutable.Value;
                    cmd.Arguments = string.Format("-unreg {0}", this.GetOLGAOPCServerName());
                    cmd.CreateNoWindow = false;
                    cmd.UseShellExecute = true;

                    process.StartInfo = cmd;
                    process.Start();*/

                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        //olga exe 종료
        private void KillAllOLGA()
        {
            //이미 실행중이라면 종료

            string olgaprocess = Path.GetFileNameWithoutExtension(this.txtOLGAExecutable.Value);

            Process[] pros = Process.GetProcesses();    //시스템의 모든 프로세스 정보 출력
            foreach (Process p in pros)
            {
                if (p.ProcessName.ToUpper().Contains("OLGA"))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(p.MainModule.FileName);

                    if (fvi.FileDescription.Equals("OLGA simulator", StringComparison.CurrentCultureIgnoreCase))
                    {
                        p.Kill();

                        Process process = new Process();
                        ProcessStartInfo cmd = new ProcessStartInfo();
                        cmd.WindowStyle = ProcessWindowStyle.Hidden;
                        cmd.FileName = this.txtOLGAExecutable.Value;
                        cmd.Arguments = string.Format("-unreg {0}", this.OPCServerName);
                        cmd.CreateNoWindow = true;
                        cmd.UseShellExecute = true;
                        process.StartInfo = cmd;
                        process.Start();
                    }
                }
            }

            Process.Start(this.txtOLGAExecutable.Value, "-unreg " + this.OPCServerName);
        }





        public bool PrepareStartSimulation()
        {

            if (this.status != extensionStatus.Connected)
            {
                if (this.ConnectOPCServer() == false)
                    return false;
            }

            if (this.IsApplyMapping == false)
            {
                this.ApplyMappingTable();
            }

            this.RunInterval = (int)this.dblOLGARunInterval.Value;

            BackDoor bd = (UniSimDesign.BackDoor)CommonController.Instance.integrator;
            this.CalcRealtimeFactor(((RealVariable)bd.get_BackDoorVariable(":ExtraData.107").Variable).Value);

            this.myOPCServers = CommonController.Instance.GetOPCServers(this);
            this.myReadDTs = CommonController.Instance.GetOTSReadDatatables(this);
            this.myWriteDTs = CommonController.Instance.GetOTSWriteDatatables(this);
            this.ElapsedTimes.Clear();
            this.AccessTimes.Clear();
            this.GapTimes.Clear();

            this.TotalExecuteSteps = 0;
            this.preTime = DateTime.Now;
            this.curTime = DateTime.Now;

            this.UpdateTImes(true);


            this.doEventsOPCToOTS = new ManualResetEvent[myOPCServers.Count];
            this.doEventsOTSToOPC = new ManualResetEvent[myReadDTs.Count];
            for (int maini = 0; maini < myOPCServers.Count; maini++) doEventsOPCToOTS[maini] = new ManualResetEvent(true);
            for (int maini = 0; maini < myReadDTs.Count; maini++) doEventsOTSToOPC[maini] = new ManualResetEvent(true);



            return true;
        }

        public void StartSimulation()
        {
            //if (MappingList.Count == 0) return false;
            //if (OPCServerList.Count == 0) return false;



            //데이터 교환 시작.
            this.IsStartDataExchange = true; //start 
            this.threadDataExchange = new Thread(procDataExcahnge);
            this.threadDataExchange.IsBackground = false;
            this.threadDataExchange.Name = "DataExchange";
            this.threadDataExchange.Start();
        }



        public bool StopSimulation()
        {
            bool haserror = false;
            this.IsStartDataExchange = false; //stop 
            return haserror;
        }


        private DateTime preTime;
        private DateTime curTime;
        private int TotalExecuteSteps = 0;

        private double OLGAStartTime;
        private double OLGAEndTime;
        private double OTSStartTime;
        private double OTSEndtime;
        private DateTime CPUStartTime;
        private DateTime CPUEndTime;


        public int RunInterval = 1;
        private int baseAccessTime = 500;
        private int accessTime = 1;
        private bool isControlMain = false;


        private List<OPCServer> myOPCServers = null;
        private List<OTSDataTable> myReadDTs = null;
        private List<OTSDataTable> myWriteDTs = null;
        TimeSpan cycleElapsed;
        private void procDataExcahnge()
        {
            try
            {
                this.isControlMain = false;
                if (CommonController.Instance.GetControllerIndex(this) == CommonController.Instance.GetMinimumIntervalControllerIndex())
                {
                    this.isControlMain = true;
                }

                DateTime cycleStart = DateTime.Now;
                while (IsStartDataExchange && IsTerminated == false)
                {
                    cycleStart = DateTime.Now;

                    this.DataExchange();

                    //check the all finished
                    if (this.isControlMain)
                    {
                        CommonController.Instance.integrator.StepsToExecute += RunInterval;
                    }

                    this.TotalExecuteSteps++;

                    this.cycleElapsed = DateTime.Now - cycleStart;

                    this.ElapsedTimes.Add((float)this.cycleElapsed.TotalMilliseconds);
                    if (this.ElapsedTimes.Count > 600)
                        ElapsedTimes.RemoveAt(0);

                    this.CalcAccesstime();
                    Thread.Sleep(accessTime);

                    //스텝 표시 
                    new Thread(() => this.UpdateTImes()).Start();

                }
            }
            catch 
            {
                //CommonController.Instance.PrintLog(ex.StackTrace);
            }
        }

        private void UpdateTImes(bool isFirst = false)
        {
            try
            {
                if (isFirst)
                {
                    this.CPUStartTime = DateTime.Now;
                    this.OTSStartTime = CommonController.Instance.integrator.CurrentTime.Value;
                }

                for (int i = 0; i < this.myOPCServers.Count; i++)
                {
                    OPCServer osg = this.myOPCServers[i];
                    if (osg.Server.Name.StartsWith("SPT."))
                    {
                        OPCSubscription os = osg.CommandSubscription;
                        Opc.Da.Subscription cg = os.Subscription; //cmd
                        os.ItemValues = cg.Read(cg.Items);

                        if (isFirst) this.OLGAStartTime = (double)os.ItemValues[1].Value;

                        this.OLGAEndTime = (double)os.ItemValues[1].Value;
                        double olgaelapsed = this.OLGAEndTime - this.OLGAStartTime;
                        this.txtOLGATimes.Values = new string[] {
                            this.ConvertTime(this.OLGAStartTime),
                            this.ConvertTime(this.OLGAEndTime),
                            this.ConvertTime(olgaelapsed) };
                        break;
                    }
                }


                //step info 
                double expectedTime = (TotalExecuteSteps * this.StepSize * this.RunInterval) / 1000.0f;
                //this.txtConnectedServer.Value = string.Format("{0}", this.OPCServerName);


                this.txtOLGAMessage.Value = string.Format("[{9}]{0}    {1}  {2} {3}    {4}*{5}/{6}={7}/{8}  {10}",
                   this.OPCServerName, this.UniqueID,
                   TotalExecuteSteps * this.RunInterval, this.ConvertTime(expectedTime),
                   this.StepSize, this.RunInterval, this.dblRealTimeFactor.Value, this.RunInterval, this.baseAccessTime,
                   CommonController.Instance.GetControllerIndex(this), cycleElapsed.TotalMilliseconds);


                //ots time
                this.OTSEndtime = CommonController.Instance.integrator.CurrentTime.Value;
                double otselapsed = this.OTSEndtime - this.OTSStartTime;
                this.txtUniSimTimes.Values = new string[] { 
                    this.ConvertTime(this.OTSStartTime),
                    this.ConvertTime(this.OTSEndtime), 
                    this.ConvertTime(otselapsed) };

                //ots cpu time
                this.CPUEndTime = DateTime.Now;
                TimeSpan sp = this.CPUEndTime - this.CPUStartTime;
                this.txtCPUClockTimes.Values = new string[] { CPUStartTime.ToString("hh:mm:ss"), CPUEndTime.ToString("hh:mm:ss"), this.ConvertTime(sp.TotalSeconds) };


                if (isControlMain)
                {
                    for (int i = 0; i < CommonController.Instance.Controllers.Count; i++)
                    {
                        if (CommonController.Instance.Controllers[i] != this)
                        {
                            CommonController.Instance.Controllers[i].UpdateTImes();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public string ConvertTime(double dsec)
        {
            int sec = (int)dsec;

            int hour = sec / (60 * 60);
            sec %= (60 * 60);

            int min = sec / 60;
            sec %= 60;

            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, min, sec);
        }

        public List<float> ElapsedTimes = new List<float>();
        public List<int> AccessTimes = new List<int>();
        public List<float> GapTimes = new List<float>();
        private double StepSize = 0;
        private void CalcAccesstime()
        {
            // how long spend time.
            preTime = curTime;
            curTime = DateTime.Now;
            TimeSpan gap = curTime - preTime;

            GapTimes.Add((float)gap.TotalMilliseconds);
            if (GapTimes.Count > 600) GapTimes.RemoveAt(0);


            if (gap.TotalMilliseconds > accessTime)
            {
                int gaptime = (int)(gap.TotalMilliseconds - accessTime);

                int tmp = baseAccessTime - gaptime;
                if (tmp < 0)
                {
                    accessTime = 1;
                }
                else
                {
                    accessTime = tmp;
                }

                AccessTimes.Add(accessTime);
                if (AccessTimes.Count > 600) AccessTimes.RemoveAt(0);
            }

            FormMonitor.Instance.RefreshMonitor();
        }

        public void CalcRealtimeFactor(double RTF)
        {
            this.StepSize = CommonController.Instance.integrator.GetStepSize() * 1000;

            this.dblRealTimeFactor.Value = RTF;
            this.accessTime = (int)(this.StepSize * this.dblOLGARunInterval.Value / this.dblRealTimeFactor.Value);
            this.baseAccessTime = this.accessTime;
        }


        private DateTime extime;
        private DateTime newextime;
        private object ConvertOTSToOPC(double value, System.Type type)
        {
            object result;

            if (type == typeof(double))
            {
                result = (double)value;
            }
            else if (type == typeof(float))
            {
                result = (float)value;
            }
            else if (type == typeof(int))
            {
                result = (int)value;
            }
            else if (type == typeof(Single))
            {
                result = (Single)value;
            }
            else if (type == typeof(Int32))
            {
                result = (Int32)value;
            }
            else if (type == typeof(UInt32))
            {
                result = value < 0 ? 0 : (UInt32)value;
            }
            else if (type == typeof(Int16))
            {
                result = (Int16)value;
            }
            else if (type == typeof(string))
            {
                result = value == 1 ? "On" : "Off";
            }
            else if (type == typeof(DateTime))
            {
                result = DateTime.Now;
            }
            else if (type == typeof(bool))
            {
                result = value == 1 ? true : false;
            }
            else if (type == typeof(double[]))
            {
                result = new double[] { (double)value };
            }
            else
            {
                result = System.Convert.ChangeType(value, type);
            }

            return result;
        }

        private double ConvertOPCToOTS(Opc.Da.ItemValueResult value)
        {
            double result = double.NaN;
            if (value.Value is double || value.Value is Double)
            {
                result = (double)value.Value;
            }
            else if (value.Value is float)
            {
                result = (float)value.Value;
            }
            else if (value.Value is int)
            {
                result = (int)value.Value;
            }
            else if (value.Value is Single)
            {
                result = (Single)value.Value;
            }
            else if (value.Value is Int32)
            {
                result = (Int32)value.Value;
            }
            else if (value.Value is Int16)
            {
                result = (Int16)value.Value;
            }
            else if (value.Value is bool || value.Value is Boolean)
            {
                result = ((bool)value.Value) == true ? 1 : 0;
            }
            else
            {
                result = -9999;
            }

            return result;
        }

        private void DataExchangeOTSSelf(object obj)
        {
            ManualResetEvent doevent = (ManualResetEvent)obj;

            OTSDataTable selfRead = null;
            for (int i = 0; i < this.myReadDTs.Count; i++)
            {
                if (this.myReadDTs[i].Type == 2)
                {
                    selfRead = this.myReadDTs[i];
                    break;
                }
            }

            OTSDataTable selfWrite = null;
            for (int i = 0; i < this.myWriteDTs.Count; i++)
            {
                if (this.myWriteDTs[i].Type == 2)
                {
                    selfWrite = this.myWriteDTs[i];
                    break;
                }
            }

            if (selfRead != null && selfWrite != null)
            {
                selfRead.TagValues = (double[])selfRead.DataTable.GetAllValues(false);
                selfWrite.TagValues = selfRead.TagValues;
                selfWrite.DataTable.SetAllValues(selfWrite.TagValues);
            }
            else
            {
                //CommonController.Instance.PrintLog("self not ");
            }


            doevent.Set();
        }

        private void DataExchangeOTS(object obj)
        {
            for (int maini = 0; maini < this.myReadDTs.Count; maini++)
            {
                OTSDataTable odt = this.myReadDTs[maini];

                List<object> args = new List<object>();
                args.Add(odt);

                doEventsOTSToOPC[maini].Reset();
                args.Add(doEventsOTSToOPC[maini]);

                ThreadPool.QueueUserWorkItem(this.DataExchangeOTSProc, args);
            }//end ots -> opc
        }

        private void DataExchangeOTSProc(object obj)
        {
            List<object> args = (List<object>)obj;
            OTSDataTable odt = (OTSDataTable)args[0];
            ManualResetEvent doevent = (ManualResetEvent)args[1];


            if (odt.Type == 1)
            {
                for (int subi = 0; subi < this.myOPCServers.Count; subi++)
                {
                    OPCServer opcSvr = this.myOPCServers[subi];
                    if (odt.ConnectedServerName == opcSvr.Name)
                    {
                        for (int subsubi = 0; subsubi < opcSvr.WriteSubscriptions.Count; subsubi++)
                        {
                            OPCSubscription wos = opcSvr.WriteSubscriptions[subsubi];
                            if (wos.Name == odt.ConnectedSubscriptionName)
                            {
                                odt.TagValues = odt.DataTable.GetAllValues() as double[];

                                if (wos.ItemValues == null || wos.ItemValues.Length != odt.TagValues.Length)
                                    wos.ItemValues = new ItemValueResult[odt.TagValues.Length];

                                List<Opc.Da.ItemValue> values = new List<ItemValue>();
                                for (int i = 0; i < odt.TagValues.Length; i++)
                                {
                                    Opc.Da.Item item = wos.Subscription.Items[i];
                                    System.Type wt = wos.ItemTypes[i];
                                    object ConvertedValue = this.ConvertOTSToOPC(odt.TagValues[i], wt);

                                    values.Add(new ItemValue(item) { Value = ConvertedValue });
                                    wos.ItemValues[i] = new ItemValueResult(item) { Value = ConvertedValue };
                                }

                                wos.Subscription.Write(values.ToArray());
                                break;
                            }
                        }
                    }// end if
                }
            }

            doevent.Set();
        }

        private void DataExchangeOPC(object obj)
        {

            for (int maini = 0; maini < this.myOPCServers.Count; maini++)
            {
                List<object> args = new List<object>();
                args.Add(this.myOPCServers[maini]);

                doEventsOPCToOTS[maini].Reset();
                args.Add(doEventsOPCToOTS[maini]);

                ThreadPool.QueueUserWorkItem(this.DataExchangeOPCProc, args);
            }
        }

        private void DataExchangeOPCProc(object obj)
        {
            List<object> args = (List<object>)obj;
            OPCServer opcSvr = (OPCServer)args[0];
            ManualResetEvent doevent = (ManualResetEvent)args[1];

            opcSvr.SelfDataExchange();


            try
            {
                if (opcSvr.Server.IsConnected)
                {

                    for (int subi = 0; subi < opcSvr.ReadSubscriptions.Count; subi++)
                    {
                        OPCSubscription opcSub = opcSvr.ReadSubscriptions[subi];


                        ////////////////////////////ots
                        if (opcSub.Type == 2) continue;
                        else if (opcSub.Type == 0)
                        {
                            opcSub.ItemValues = opcSub.Subscription.Read(opcSub.Subscription.Items);

                            //ots write
                            if (opcSub.ItemValues.Length > 0)
                            {
                                OTSDataTable odt = CommonController.Instance.OTSWriteDataTableList[opcSub.ConnectedDataTableIndex];

                                if (odt.TagValues == null)
                                    odt.TagValues = new double[odt.DataTable.VarDefinitions.Count];

                                for (int j = 0; j < opcSub.ItemValues.Length; j++)
                                {
                                    System.Type rt = opcSub.ItemTypes[j];
                                    double FromValue = this.ConvertOPCToOTS(opcSub.ItemValues[j]);
                                    odt.TagValues[j] = FromValue;
                                }
                                odt.DataTable.SetAllValues(odt.TagValues);
                            }
                        }
                        /////////////////////////////////// opc
                        else if (opcSub.Type == 1)
                        {
                            for (int subsubi = 0; subsubi < this.myOPCServers.Count; subsubi++)
                            {
                                OPCServer subOpcSvr = this.myOPCServers[subsubi];

                                if (subOpcSvr.Name == opcSub.ConnectedServerName)
                                {
                                    for (int j = 0; j < subOpcSvr.WriteSubscriptions.Count; j++)
                                    {
                                        OPCSubscription wos = subOpcSvr.WriteSubscriptions[j];
                                        if (wos.Name == opcSub.ConnectedSubscriptionName)
                                        {
                                            opcSub.ItemValues = opcSub.Subscription.Read(opcSub.Subscription.Items);
                                            List<Opc.Da.ItemValue> values = new List<ItemValue>();

                                            for (int i = 0; i < opcSub.ItemValues.Length; i++)
                                            {
                                                Opc.Da.Item item = wos.Subscription.Items[i];
                                                Opc.Da.ItemValueResult ivr = opcSub.ItemValues[i];

                                                System.Type wt = wos.ItemTypes[i];
                                                System.Type rt = opcSub.ItemTypes[i];

                                                object value = ivr.Value;
                                                if (wt != rt)
                                                {
                                                    value = System.Convert.ChangeType(ivr.Value, wt);
                                                }
                                                values.Add(new ItemValue(item) { Value = value });
                                            }
                                            wos.ItemValues = opcSub.ItemValues;
                                            wos.Subscription.Write(values.ToArray());

                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            doevent.Set();
        }

        private void DataExchangeStep(object obj)
        {
            try
            {
                for (int maini = 0; maini < this.myOPCServers.Count; maini++)
                {
                    OPCServer opcSvr = this.myOPCServers[maini];
                    if (opcSvr.Name.StartsWith("SPT."))
                    {
                        opcSvr.CommandSubscription.ItemValues = opcSvr.CommandSubscription.Subscription.Read(opcSvr.CommandSubscription.Subscription.Items);

                        extime = (DateTime)opcSvr.CommandSubscription.ItemValues[0].Value;
                        newextime = extime.AddMilliseconds(this.StepSize * this.RunInterval);

                        Opc.Da.ItemValue value = new Opc.Da.ItemValue(opcSvr.CommandSubscription.Subscription.Items[0]);
                        value.Value = newextime;
                        opcSvr.CommandSubscription.Subscription.Write(new Opc.Da.ItemValue[] { value });
                    }
                }
            }
            catch { }
        }

        private ManualResetEvent[] doEventsOPCToOTS;
        private ManualResetEvent[] doEventsOTSToOPC;
        private void DataExchange()
        {
            while (CommonController.Instance.integrator.StepsToExecute != 0)
            {
                Thread.Sleep(1);
            }


            for (int maini = 0; maini < myOPCServers.Count; maini++)
            {
                OPCServer opcSvr = myOPCServers[maini];
                if (opcSvr.Name.StartsWith("SPT."))
                {
                    OPCSubscription cg = opcSvr.CommandSubscription;
                    cg.ItemValues = cg.Subscription.Read(cg.Subscription.Items);

                    string state = (string)cg.ItemValues[2].Value;
                    while (state == "STATE_RUNNING") // 러닝중이면 기다린다.
                    {
                        Thread.Sleep(1);
                        cg.ItemValues = cg.Subscription.Read(cg.Subscription.Items);
                        state = (string)cg.ItemValues[2].Value;
                    }
                }
            }

            ManualResetEvent[] doEvent = new ManualResetEvent[] { new ManualResetEvent(false) };
            ThreadPool.QueueUserWorkItem(this.DataExchangeOTSSelf, doEvent[0]);
            ThreadPool.QueueUserWorkItem(this.DataExchangeOPC);
            ThreadPool.QueueUserWorkItem(this.DataExchangeOTS);

            WaitHandle.WaitAll(doEvent);
            if (doEventsOPCToOTS.Length > 0) WaitHandle.WaitAll(doEventsOPCToOTS);
            if (doEventsOTSToOPC.Length > 0) WaitHandle.WaitAll(doEventsOTSToOPC);

            ThreadPool.QueueUserWorkItem(this.DataExchangeStep);
        }
    }
}