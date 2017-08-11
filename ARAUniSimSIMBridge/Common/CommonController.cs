using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniSimDesign;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ARAUniSimSIMBridge.Data;
using Aga.Controls.Tree;
using OpcCom;
using Opc;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Xml.Serialization;
using Opc.Da;

namespace ARAUniSimSIMBridge
{
    public class CommonController
    {

        private static CommonController _instance = null;
        public static CommonController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommonController();
                }
                return _instance;
            }
        }

        public ExtnUnitOperationContainer hyContainer;
        public Flowsheet flowsheet;
        public _SimulationCase simCase;
        public Solver solver;
        public Integrator integrator;
        public FixedDataTables hypdb;

        public List<PrivateController> Controllers { get; set; }
        public TreeModel TreemodelOPC { get; set; }
        public TreeModel TreemodelTag { get; set; }
        public List<OTSTagData> OTSTagList { get; set; }

        public double StepsOfOTS { get; set; }
        public string TimesOfReal { get; set; }
        public string TimesOfOTS { get; set; }

        public List<string> OPCLocalServerNames { get; set; }

        public List<OPCServer> OPCServerList { get; set; }
        public List<OTSDataTable> OTSReadDataTableList { get; set; }
        public List<OTSDataTable> OTSWriteDataTableList { get; set; }

        private bool IsTerminated = false;
        private bool IsRunningOTS = false;//init
        private Thread threadCheckSystem; //DRTF, Factor, modified

        public CommonController()
        {
            this.TreemodelOPC = new TreeModel();
            this.TreemodelTag = new TreeModel();
            this.OTSTagList = new List<OTSTagData>();
            this.OPCServerList = new List<OPCServer>();
            this.OTSWriteDataTableList = new List<OTSDataTable>();
            this.OTSReadDataTableList = new List<OTSDataTable>();
            this.OPCLocalServerNames = new List<string>();
            this.Controllers = new List<PrivateController>();
        }

        public void SetContainer(ExtnUnitOperationContainer Container)
        {
            if (this.hyContainer == null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                this.hyContainer = Container;


                this.flowsheet = this.hyContainer.Flowsheet;
                this.hypdb = this.hyContainer.SimulationCase.DataTables;
                this.simCase = this.hyContainer.SimulationCase;
                this.solver = this.hyContainer.SimulationCase.Solver;
                this.integrator = this.hyContainer.SimulationCase.Solver.Integrator;



                //opc 목록 가져오기.
                this.GetOPCServerList(); //처음 갱신.


                this.GetTagList(this.flowsheet, this.TreemodelTag.Nodes);


                this.SetBaseDocument();

                //opc서버 저장 상태
                this.IsTerminated = false; //start
                this.IsRunningOTS = false; //start

                /*
                this.threadCheckSystem = new Thread(this.procCheckSystem);
                this.threadCheckSystem.IsBackground = true;
                this.threadCheckSystem.Name = "CheckSystem";
                this.threadCheckSystem.Start();*/

                sw.Stop();
                //this.PrintLog(sw.ElapsedMilliseconds + ", SetContainer");
            }

        }

        /// <summary>
        /// unisim 모델별 스냅샷 경로
        /// </summary>
        public string PathModelSnapshotDirectory { get; set; }

        /// <summary>
        /// unisim 모델별 경로
        /// </summary>
        public string PathModelWorkDirectory { get; set; }

        /// <summary>
        /// arasimbridge 경로
        /// </summary>
        public string PathSIMBridgeDirectory { get; set; }

        /// <summary>
        /// extension 설정 파일 경로
        /// </summary>
        public string PathOLGAEXEConfig { get; set; }

        /// <summary>
        /// 올가 실행파일 경로 ( program file )
        /// </summary>
        public string PathInstalledOLGAEXE { get; set; }

        private void SetBaseDocument()
        {
            this.PathSIMBridgeDirectory = string.Format("{0}\\ARASIMBridge", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (Directory.Exists(PathSIMBridgeDirectory) == false)
            {
                Directory.CreateDirectory(PathSIMBridgeDirectory);
            }



            this.PathOLGAEXEConfig = string.Format("{0}\\OLGAExePath.xml", this.PathSIMBridgeDirectory);

            this.PathModelWorkDirectory = string.Format("{0}\\{1}", this.PathSIMBridgeDirectory, Path.GetFileNameWithoutExtension(this.simCase.FullName));
            if (Directory.Exists(PathModelWorkDirectory) == false)
            {
                Directory.CreateDirectory(PathModelWorkDirectory);
            }


            this.PathModelSnapshotDirectory = string.Format("{0}\\Snapshot", this.PathModelWorkDirectory);
            if (Directory.Exists(PathModelSnapshotDirectory) == false)
            {
                Directory.CreateDirectory(PathModelSnapshotDirectory);
            }



            bool checkOlgaPath = true;
            if (File.Exists(this.PathOLGAEXEConfig))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(OLGAConfig));
                using (StreamReader reader = new StreamReader(this.PathOLGAEXEConfig))
                {
                    OLGAConfig cf = (OLGAConfig)xmlSerializer.Deserialize(reader);
                    if (File.Exists(cf.PathOLGAExecutable))
                    {
                        this.PathInstalledOLGAEXE = cf.PathOLGAExecutable;
                    }
                    else //파일은 있지만 경로가 존재 하지 않을경우.
                    {
                        checkOlgaPath = false;
                    }
                }

            }
            else
            {
                checkOlgaPath = false;
            }

            if (checkOlgaPath == false) //파일이 없다면.
            {
                this.PathInstalledOLGAEXE = this.FindInstalledOLGA();
                this.SaveOLGAExecutablePath();
            }
        }

        private void CheckARAExtensionSIMBridge()
        {
            Process[] pss = Process.GetProcesses();

            bool check = false;
            for (int i = 0; i < pss.Length; i++)
            {
                if (pss[i].ProcessName.Contains("ARAExtensionSIMBridge"))
                {
                    check = true;
                }
            }
            if (check == false)
            {
                Process.Start("ARAExtensionSIMBridge\\ARAExtensionSIMBridge.exe", null);
            }
        }

        public void RegisterController(PrivateController controller)
        {
            if (this.Controllers.Count > 0)
            {
                if (this.Controllers.Where(x => x.UniqueID == controller.UniqueID).Count() > 0)
                {
                    controller.UniqueID = string.Empty;
                }
            }
            this.Controllers.Add(controller);

        }

        public void UnregisterController(PrivateController controller)
        {
            this.Controllers.Remove(controller);

            if (this.Controllers.Count == 0)
            {
                this.hyContainer = null;
 
                if (this.threadCheckSystem != null)
                {
                    this.threadCheckSystem.Abort();
                    this.threadCheckSystem = null;
                }

                this.IsTerminated = true; //end
                this.IsRunningOTS = false; //end

                this.OPCServerList.Clear();
                this.OTSTagList.Clear();
                this.OTSReadDataTableList.Clear();
                this.OTSWriteDataTableList.Clear();
                this.Controllers.Clear();
                this.OPCLocalServerNames.Clear();

                this.TreemodelOPC.Nodes.Clear();
                this.TreemodelTag.Nodes.Clear();

                FormMonitor.Instance.Dispose();
                FormMapping.Instance.Dispose();
            }

        }

        public int GetControllerIndex(PrivateController controller)
        {
            int result = -1;

            result = this.Controllers.IndexOf(controller);

            return result;
        }

        public int GetMinimumIntervalControllerIndex()
        {
            int result = -1;
            int min = 99999;
            for (int i = 0; i < this.Controllers.Count; i++)
            {
                PrivateController pc = this.Controllers[i];
                if (min > pc.RunInterval)
                {
                    min = pc.RunInterval;
                    result = i;
                }
            }

            return result;
        }

        private string FindInstalledOLGA()
        {
            RegistryKey SoftwareKey = Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Microsoft", true).OpenSubKey("Windows", true).OpenSubKey("CurrentVersion", true).OpenSubKey("Uninstall", true);
            string[] arrStrKeyName = SoftwareKey.GetSubKeyNames();

            for (int i = 0; i < arrStrKeyName.Length; i++)
            {
                string name = SoftwareKey.OpenSubKey(arrStrKeyName[i]).GetValue("DisplayName", "N").ToString();

                if (name.Contains("OLGA"))
                {
                    string installlocation = SoftwareKey.OpenSubKey(arrStrKeyName[i]).GetValue("InstallLocation", "N").ToString();

                    if (installlocation != "N")
                    {
                        name = name.Replace(" ", "-");
                        string path = FindInstalledOLGA(installlocation, name + ".exe");

                        if (string.IsNullOrEmpty(path) == false)
                        {
                            //this.txtOLGAEXE.Value = path;
                            //this.txtOLGAModel.Value = @"C:\Program Files (x86)\Schlumberger\OLGA 2015.1.1\Data\OPG Files\OPC-Server\Server-demo-with-opc_T\server-demo-with-opc.genkey";
                            //this.PrintLog(path);
                            return path;
                        }
                    }
                }
            }

            return string.Empty;
        }

        private string FindInstalledOLGA(string path, string OLGAfilename)
        {
            //this.PrintLog(path);
            string result = string.Empty;

            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            for (int i = 0; i < files.Length; i++)
            {
                string filename = Path.GetFileName(files[i]);
                string extension = Path.GetExtension(files[i]);

                if (extension.Equals(".exe", StringComparison.InvariantCulture))
                {
                    //this.PrintLog(filename);
                    if (filename == OLGAfilename)
                    {
                        result = files[i];
                        return result;
                    }
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    result = this.FindInstalledOLGA(dirs[i], OLGAfilename);

                    if (string.IsNullOrEmpty(result) == false)
                    {
                        return result;
                    }
                }
            }


            return result;
        }

        public void SaveOLGAExecutablePath()
        {
            //string path = this.FindInstalledOLGA();

            OLGAConfig cf = new OLGAConfig()
            {
                PathOLGAExecutable = this.PathInstalledOLGAEXE
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OLGAConfig));
            using (StreamWriter reader = new StreamWriter(this.PathOLGAEXEConfig))
            {
                xmlSerializer.Serialize(reader, cf);
            }
        }

        public void PrintLog(float msg)
        {
            PrintLog(msg.ToString());
        }
        public void PrintLog(double msg)
        {
            PrintLog(msg.ToString());
        }
        public void PrintLog(int msg)
        {
            PrintLog(msg.ToString());
        }
        public void PrintLog(string msg)
        {
            if (hyContainer != null)
                hyContainer.Trace(msg, false);
        }

        public bool CheckDuplicatedOLGAOPCServer(PrivateController pc)
        {
            bool result = false;
            for (int i = 0; i < this.Controllers.Count; i++)
            {
                PrivateController sub = this.Controllers[i];

                if (sub == pc) continue;

                if (sub.OPCServerName == pc.OPCServerName)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private DateTime OTSModelFileModifiedTIme;
        private double DTRF = -999999;
        private void procCheckSystem()
        {
            try
            {
                BackDoor bd = (UniSimDesign.BackDoor)this.integrator;
                this.OTSModelFileModifiedTIme = File.GetLastAccessTime(this.simCase.FullName);
                while (IsTerminated == false)
                {

                    //check desired real time factor 
                    RealVariable rv = (RealVariable)bd.get_BackDoorVariable(":ExtraData.107").Variable;
                    if (rv.Value != DTRF)
                    {
                        DTRF = rv.Value;
                        this.SetRealtimeFactor(DTRF);
                    }

                    //realtime 껏을경우 1로 초기화
                    if (this.integrator.RunAtRealTime == false)
                    {
                        bool flag = false;
                        for (int i = 0; i < this.Controllers.Count; i++)
                        {
                            PrivateController pc = this.Controllers[i];
                            if (pc.dblRealTimeFactor.Value != 1)
                            {
                                pc.dblRealTimeFactor.Value = 1;
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            rv.Value = 1;
                        }
                    }



                    //if (IsTerminated) break;
                    //for (int i = 0; i < this.Controllers.Count; i++)
                    //{
                    //    PrivateController pc = this.Controllers[i];
                    //    if (pc.preRealTimeFactor != pc.dblRealTimeFactor.Value)
                    //    {
                    //        this.SetRealtimeFactor(pc.dblRealTimeFactor.Value);
                    //        break;
                    //    }
                    //}




                    if (IsTerminated) break;
                    //check data exchange step 
                    for (int i = 0; i < this.Controllers.Count; i++)
                    {
                        if (IsTerminated) break;

                        PrivateController pc = this.Controllers[i];
                        double dbl = pc.dblOLGARunInterval.GetValue();
                        if (dbl < 1)
                        {
                            pc.dblOLGARunInterval.SetValue(1);
                        }
                    }


                    if (IsTerminated) break;
                    // chek model file change 
                    DateTime newTime = File.GetLastAccessTime(this.simCase.FullName);
                    if (this.OTSModelFileModifiedTIme.CompareTo(newTime) != 0)
                    {
                        this.OTSModelFileModifiedTIme = newTime;

                        for (int i = 0; i < this.Controllers.Count; i++)
                        {
                            if (IsTerminated) break;

                            PrivateController pc = this.Controllers[i];

                            string olgasnapfile = string.Format("{0}\\{1}", this.PathModelSnapshotDirectory, pc.UniqueID);
                            if (pc.SaveOLGASnapshot(olgasnapfile))
                            {
                                pc.txtOLGASnapshot.Value = olgasnapfile + ".rsw";
                            }

                            pc.SaveMappingList(true);
                            pc.SaveConfiguration();
                        }
                        this.DeleteNotUsingController();
                    }


                    if (IsTerminated) break;
                    // chek opc status
                    for (int i = 0; i < this.Controllers.Count; i++)
                    {
                        if (IsTerminated) break;

                        PrivateController pc = this.Controllers[i];
                        if (pc.dblConnectedOPC.Value == 1)
                        {
                            Opc.Server existOlga = this.FindOLGAOPCServer(pc.OPCServerName);
                            if (existOlga != null) //서버 있음
                            {
                            }
                            else //서버 없음.
                            {
                                if (pc.status != extensionStatus.Disconnected)
                                {
                                    pc.SetStatus(extensionStatus.Disconnected);
                                    pc.dblConnectedOPC.SetValue(0);

                                    this.RemoveOPCServers(pc); //서버 꺼짐
                                    this.GetOPCServerList(); //서버 꺼짐 

                                    pc.StopSimulation();//서버 꺼짐
                                }
                            }
                        }
                    }



                    if (IsTerminated) break;
                    // chek ots start
                    if (this.integrator.IsRunning) //시작했다면
                    {
                        if (this.IsRunningOTS == false) //  시작전 아직 시작 체크가 안됐다면,
                        {
                            this.StartSimulation();
                        }
                        this.IsRunningOTS = true;// 처음 한번만 하기위해 체크용도 bool
                    }
                    else //정지라면
                    {
                        if (this.IsRunningOTS == true) // 시작이었다면.
                        {
                            for (int i = 0; i < this.Controllers.Count; i++)
                            {
                                if (IsTerminated) break;

                                PrivateController pc = this.Controllers[i];
                                pc.StopSimulation();
                                pc.dblRunningSim.SetValue(0);//중지 표시.
                            }
                        }
                        this.IsRunningOTS = false; //중지 표시
                    }


                    if (this.integrator.Mode != IntegratorMode_enum.imManual) this.integrator.Mode = IntegratorMode_enum.imManual;

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                // this.PrintLog(ex.StackTrace);
            }
        }

        private void StartSimulation()
        {
            bool isprepare = true;
            this.InitDataTable();

            for (int i = 0; i < this.Controllers.Count; i++)
            {
                PrivateController pc = this.Controllers[i];
                if (pc.PrepareStartSimulation() == false) isprepare = false;
            }
            if (isprepare && IsTerminated == false)
            {
                for (int i = 0; i < this.Controllers.Count; i++)
                {
                    PrivateController pc = this.Controllers[i];
                    pc.StartSimulation();
                    pc.SaveConfiguration();
                    pc.dblRunningSim.SetValue(1); //시작 표시.
                }
            }
        }

        private void SetRealtimeFactor(double rtf)
        {
            for (int i = 0; i < this.Controllers.Count; i++)
            {
                PrivateController pc = this.Controllers[i];
                pc.preRealTimeFactor = rtf;
                pc.dblRealTimeFactor.Value = rtf;
            }
        }

        private void DeleteNotUsingController()
        {
            List<string> ids = this.Controllers.Select(x => x.UniqueID).ToList();
            string[] files = Directory.GetFiles(this.PathModelWorkDirectory);
            for (int i = files.Length - 1; i >= 0; i--)
            {
                string path = files[i];

                string filename = Path.GetFileNameWithoutExtension(path);

                bool exists = false;
                for (int j = 0; j < ids.Count; j++)
                {
                    if (filename.Contains(ids[j]))
                    {
                        exists = true;
                        break;
                    }
                }

                //this.PrintLog(filename + ", " + exists);
                if (exists == false)
                {
                    File.Delete(path);
                }
            }


            files = Directory.GetFiles(this.PathModelSnapshotDirectory);
            for (int i = files.Length - 1; i >= 0; i--)
            {
                string path = files[i];
                string filename = Path.GetFileNameWithoutExtension(path);

                bool exists = false;
                for (int j = 0; j < ids.Count; j++)
                {
                    if (filename.Contains(ids[j]))
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists == false)
                {
                    File.Delete(path);
                }
            }
        }

        private void GetTagList(UniSimDesign.Flowsheet fs, System.Collections.ObjectModel.Collection<Node> nodes)
        {
            Node fNode = new Node(fs.name);
            nodes.Add(fNode);

            /*
            Streams sts = fs.Streams;
            for (int i = 0; i < sts.Count; i++)
            {
                ProcessStream obj = (ProcessStream)sts[i];
            }*/

            Operations ops = fs.Operations;
            for (int i = 0; i < ops.Count; i++)
            {
                UniSimDesign._IOperation op = (UniSimDesign._IOperation)ops[i];

                string name = op.name;
                string type = op.TypeName;
                string sheet = op.TaggedName;
                sheet = sheet.Split('@')[1];

                HYSYSTagData htag = new HYSYSTagData() { type = type, name = name, sheet = sheet, op = op };

                Node tNode = new Node(name);
                fNode.Nodes.Add(tNode);
                //tNode.Image = FormBrowser.Instance.imagelist.Images[4];
                tNode.Image = Properties.Resources.tag_blue;


                if (type == "araunisimsimbridge.arasimbridge")
                {

                }
                else if (type == "valveop")
                {
                    UniSimDesign.Valve valve = op as UniSimDesign.Valve;
                    if (valve == null) continue;
                    int valvetype = (int)valve.ValveType;

                    OTSTagData ResistanceValue = new OTSTagData()
                    {
                        TagName = name,
                        Type = type,
                        Sheet = sheet,
                        op = op,
                        value = valve.ResistanceValue,
                        Parameter = "ResistanceValue",
                    };

                    OTSTagData PercentOpenValue = new OTSTagData()
                    {
                        TagName = name,
                        Type = type,
                        Sheet = sheet,
                        op = op,
                        value = valve.PercentOpenValue,
                        Parameter = "PercentOpenValue",
                    };

                    this.OTSTagList.Add(ResistanceValue);
                    this.OTSTagList.Add(PercentOpenValue);

                    //htag.ResistanceValue = valve.ResistanceValue;
                    //htag.PercentOpenValue = valve.PercentOpenValue;
                }
                else if (type == "selectionop")
                {
                    UniSimDesign.SelectorBlock sb = op as UniSimDesign.SelectorBlock;
                    if (sb == null) continue;
                    if (sb.OutputVal == null) continue;


                    OTSTagData OutputVal = new OTSTagData()
                    {
                        TagName = name,
                        Type = type,
                        Sheet = sheet,
                        op = op,
                        value = sb.OutputVal.Value,
                        Parameter = "OutputVal",
                    };
                    this.OTSTagList.Add(OutputVal);

                    // htag.OutputVal = sb.OutputVal.Value;
                }
                else if (type == "fbcontrolop")
                {
                    UniSimDesign.Controller fn = op as UniSimDesign.Controller;
                    if (fn == null) continue;

                    UniSimDesign.UnitConversionSet ucS1;
                    UniSimDesign.UnitConversionType_enum ucType = fn.PV.UnitConversionType;
                    ucS1 = ((UniSimDesign.Application)hyContainer.Flowsheet.Application).UnitConversionSetManager.GetUnitConversionSet(ucType);

                    //htag.unit = ucS1.CurrentDisplayUnit.name;
                    //htag.PV = fn.PV.GetValue(ucS1.CurrentDisplayUnit.name);

                    OTSTagData PV = new OTSTagData()
                    {
                        TagName = name,
                        Type = type,
                        Sheet = sheet,
                        op = op,
                        value = fn.PV.GetValue(ucS1.CurrentDisplayUnit.name),
                        Unit = ucS1.CurrentDisplayUnit.name,
                        Parameter = "PV",
                    };
                    this.OTSTagList.Add(PV);

                }

                else if (type == "digitalop")
                {
                    UniSimDesign.BackDoor bd;
                    UniSimDesign.RealVariable hyrv;
                    bd = (UniSimDesign.BackDoor)op;
                    hyrv = (UniSimDesign.RealVariable)bd.get_BackDoorVariable(":selection.501").Variable; //Output Stat
                    //hyrv = bd.BackDoorVariable(":selection.501").Variable; //Output Stat
                    //htag.OPState = hyrv.Value;


                    OTSTagData OPState = new OTSTagData()
                    {
                        TagName = name,
                        Type = type,
                        Sheet = sheet,
                        op = op,
                        value = hyrv.Value,
                        Parameter = "OPState",
                    };
                    this.OTSTagList.Add(OPState);
                }
                else if (type == "booleanlatchop")
                {
                    UniSimDesign.BooleanLatch bl = op as UniSimDesign.BooleanLatch;
                    if (bl == null) continue;
                    //htag.OutputVal = bl.OutputValue;


                    OTSTagData OutputVal = new OTSTagData()
                    {
                        TagName = name,
                        Type = type,
                        Sheet = sheet,
                        op = op,
                        value = bl.OutputValue,
                        Parameter = "OutputVal",
                    };
                    this.OTSTagList.Add(OutputVal);
                }
                else if (type == "booleanandop")
                {
                    UniSimDesign.BooleanAnd ba = op as UniSimDesign.BooleanAnd;
                    if (ba == null) continue;

                    //htag.OutputVal = ba.OutputValue;

                    OTSTagData OutputVal = new OTSTagData()
                    {
                        TagName = name,
                        Type = type,
                        Sheet = sheet,
                        op = op,
                        value = ba.OutputValue,
                        Parameter = "OutputVal",
                    };
                    this.OTSTagList.Add(OutputVal);

                }
                else if (type == "reliefvalveop")
                {
                }
                else if (type == "flashtank")
                {
                }
                else if (type == "sep3op")
                {
                }
                else if (type == "heatexop")
                {
                }
                else if (type == "aircollerop")
                {
                }
                else if (type == "pumpop")
                {
                }
                else if (type == "booleanlatchop")
                {
                }
                else if (type == "booleanandop")
                {
                }
            }

            if (fs.Flowsheets.Count > 0)
            {
                for (int i = 0; i < fs.Flowsheets.Count; i++)
                {
                    this.GetTagList(fs.Flowsheets[i] as UniSimDesign.Flowsheet, fNode.Nodes);
                }
            }
        }

        /// <summary>
        /// loacal, network opc server list 불러오기.
        /// </summary>
        public void GetOPCServerList()
        {
            for (int i = this.TreemodelOPC.Nodes.Count - 1; i >= 0; i--)
            {
                this.TreemodelOPC.Nodes.RemoveAt(i);
            }
            this.TreemodelOPC.Nodes.Clear();


            Node rootNode = new Node("Local");
            //rootNode.Image = FormBrowser.Instance.imagelist.Images[0];
            rootNode.Image = Properties.Resources.monitor;

            if (this.TreemodelOPC.Nodes.Where(x => x.Text.Equals("Local")).Count() == 0)
            {
                this.OPCLocalServerNames.Clear();
                this.OPCLocalServerNames.Add("OLGA OPCServer");
                this.TreemodelOPC.Nodes.Add(rootNode);
                try
                {
                    ServerEnumerator discovery = new OpcCom.ServerEnumerator();
                    Opc.Server[] servers2 = discovery.GetAvailableServers(Specification.COM_DA_20);
                    if (servers2 != null)
                    {
                        foreach (Opc.Server server in servers2)
                        {
                            Node sub = new Node(server.Name);
                            sub.Tag = server;
                            //sub.Image = FormBrowser.Instance.imagelist.Images[2];
                            sub.Image = Properties.Resources.server;
                            rootNode.Nodes.Add(sub);

                            if (server.Name.StartsWith("SPT."))
                                continue;

                            if (server.Name.StartsWith("AspenTech.OTS"))
                                continue;


                            this.OPCLocalServerNames.Add(server.Name);
                            //this.PrintLog(server.Url.ToString());
                        }
                    }
                }
                catch { }
            }
            //new Thread(procGetNetwork).Start(); 
        }

        private void procGetNetwork()
        {
            List<string> opcNetworkServers = new List<string>();

            ServerEnumerator discovery = new OpcCom.ServerEnumerator();

            Node NetworkNode = new Node("Network");
            NetworkNode.Image = Properties.Resources.globe_network;
            //NetworkNode.Image = FormBrowser.Instance.imagelist.Images[1];

            lock (this.TreemodelOPC)
            {
                if (this.TreemodelOPC.Nodes.Where(x => x.Text.Equals("Network")).Count() == 0)
                {
                    this.TreemodelOPC.Nodes.Add(NetworkNode);

                    string[] hosts = null;
                    try
                    {
                        hosts = discovery.EnumerateHosts();
                    }
                    catch
                    {

                    }

                    if (hosts != null && hosts.Length > 0)
                    {
                        foreach (string host in hosts)
                        {
                            Node sub = new Node(host);
                            NetworkNode.Nodes.Add(sub);

                            sub.Image = Properties.Resources.monitor;
                            //sub.Image = FormBrowser.Instance.imagelist.Images[0];

                            opcNetworkServers.Add(host);

                        }

                        for (int i = 0; i < this.Controllers.Count; i++)
                        {
                            PrivateController pc = this.Controllers[i];
                            pc.txtNetworkServers.SetBounds(opcNetworkServers.Count);
                            pc.txtNetworkServers.Values = opcNetworkServers.ToArray();
                        }
                    }
                }
            }
        }

        public void SetViewOLGAList()
        {
            for (int i = 0; i < this.Controllers.Count; i++)
            {
                PrivateController pc = this.Controllers[i];
                pc.txtLocalServers.SetBounds(this.OPCLocalServerNames.Count);
                pc.txtLocalServers.Values = this.OPCLocalServerNames.ToArray();
            }
        }

        public bool AddMappingOPCOTS(List<string> serverNames, List<MappingData> MappingList, PrivateController pc)
        {
            bool noError = true;

            this.ResetControllerData(pc);

            for (int maini = 0; maini < serverNames.Count; maini++)
            {
                string serverName = serverNames[maini];

                /*********************************************************************************/
                // hysys self
                /*if (this.GetControllerIndex(pc) == 0 && serverName.Equals("hysys", StringComparison.CurrentCultureIgnoreCase))
                {
                    List<MappingData> selfMapping = (from query in MappingList.AsEnumerable()
                                                     where query.FromType.Equals(serverName, StringComparison.CurrentCultureIgnoreCase)
                                                     && query.ToType.Equals(serverName, StringComparison.CurrentCultureIgnoreCase)
                                                     select query).ToList();

                    if (selfMapping.Count > 0)
                    {
                        DataTable dt = (DataTable)this.hypdb.Add(string.Format("ara_{0}_{1}_r", pc.UniqueID, serverName));
                        OTSDataTable odt = new OTSDataTable(pc)
                        {
                            DataTable = dt,
                            Type = 2,
                            ConnectedServerIndex = -1,
                            TagNames = new string[selfMapping.Count],
                            TagUnits = new string[selfMapping.Count],
                            TagValues = new double[selfMapping.Count],
                        };
                        this.OTSReadDataTableList.Add(odt);

                        for (int i = 0; i < selfMapping.Count; i++)
                        {
                            MappingData md = selfMapping[i];
                            string[] strarr = md.FromName.Split('.');
                            _IOperation obj = this.FindObject(strarr[0], strarr[1]);

                            if (obj != null)
                            {
                                this.InsertDataInTable(obj, strarr[0], strarr[1], dt.VarDefinitions, DataTableAccessMode_enum.dtam_Read);

                                odt.TagNames[i] = strarr[0];
                                odt.TagUnits[i] = string.Empty;
                                VarDefinition var = obj as VarDefinition;
                                if (var != null) odt.TagUnits[i] = var.Units;
                            }
                        }


                        dt = (DataTable)this.hypdb.Add(string.Format("ara_{0}_{1}_w", pc.UniqueID, serverName));
                        odt = new OTSDataTable(pc)
                        {
                            DataTable = dt,
                            Type = 2,
                            ConnectedServerIndex = -1,
                            TagNames = new string[selfMapping.Count],
                            TagUnits = new string[selfMapping.Count],
                            TagValues = new double[selfMapping.Count],
                        };
                        this.OTSWriteDataTableList.Add(odt);

                        for (int i = 0; i < selfMapping.Count; i++)
                        {
                            MappingData md = selfMapping[i];
                            string[] strarr = md.ToName.Split('.');
                            _IOperation obj = this.FindObject(strarr[0], strarr[1]);

                            if (obj != null)
                            {
                                this.InsertDataInTable(obj, strarr[0], strarr[1], dt.VarDefinitions, DataTableAccessMode_enum.dtam_ReadWrite);

                                odt.TagNames[i] = strarr[0];
                                odt.TagUnits[i] = string.Empty;
                                VarDefinition var = obj as VarDefinition;
                                if (var != null) odt.TagUnits[i] = var.Units;
                            }
                        }
                    }
                    continue;
                }*/


                ////////////////////////////////////////////////////////////////////////////////
                //tree에서 노드 찾기
                Node svrNode = this.FindServerNode(serverName);

                if (svrNode == null) { continue; }
                if ((svrNode.Tag is Opc.Server) == false) { continue; }

                Opc.Server svr = (Opc.Server)svrNode.Tag;
                Opc.Da.Server dasvr = (Opc.Da.Server)svr;


                List<MappingData> readMapping = (from query in MappingList.AsEnumerable()
                                                 where query.FromType.Equals(serverName, StringComparison.CurrentCultureIgnoreCase)
                                                 select query).ToList();
                List<string> readT = readMapping.Select(x => x.ToType).Distinct().ToList();


                List<MappingData> writeMapping = (from query in MappingList.AsEnumerable()
                                                  where query.ToType.Equals(serverName, StringComparison.CurrentCultureIgnoreCase)
                                                  select query).ToList();
                List<string> writeF = writeMapping.Select(x => x.FromType).Distinct().ToList();



                try
                {
                    OPCServer ns = null;
                    ns = this.FindOPCServerByProgID(serverName);
                    if (ns == null) //없으면 새로 추가.
                    {
                        this.PrintLog(serverName + " new created");
                        ns = new OPCServer(pc);
                        ns.SetServer(dasvr);
                        this.OPCServerList.Add(ns);
                    }
                    else
                    {
                        dasvr = ns.Server;
                    }

                    //opc서버 연결 상태 아니라면.
                    if (dasvr.IsConnected == false)
                    {
                        //opc server 연결 시도 
                        if (ns.Connect()) //AddMappingOPCOTS 
                        {
                            //연결 성공시 모든 opc item 찾기.
                            this.BrowseChildren(ns, dasvr, svrNode.Nodes, null);
                            //svrNode.Image = FormBrowser.Instance.imagelist.Images[3];
                            svrNode.Image = Properties.Resources.monitor_link;
                        }
                        else
                        {
                            pc.SetStatus(extensionStatus.Disconnected);
                            return false;
                        }
                    }




                    Opc.Da.BrowseFilters filters = new Opc.Da.BrowseFilters()
                    {
                        BrowseFilter = Opc.Da.browseFilter.all,
                        //Fliters의 모든 속성과 속성값을 리턴하도록 설정
                        ReturnAllProperties = true,
                        ReturnPropertyValues = true,
                    };


                    /*********************************************************************************/
                    // opc -> ots
                    for (int subi = 0; subi < readT.Count; subi++)
                    {
                        string subServerName = readT[subi];

                        List<MappingData> subMapping = (from query in readMapping.AsEnumerable()
                                                        where query.ToType == subServerName
                                                        select query).ToList();


                        //opc 그룹 추가.
                        OPCSubscription opcGroup = ns.AddGroup(string.Format("r_{0}", subServerName), 0);
                        opcGroup.ConnectedServerName = subServerName;
                        opcGroup.ConnectedSubscriptionName = string.Format("w_{0}", serverName);

                        //셀프 태그
                        if (serverName == subServerName) opcGroup.Type = 2;

                        //ots 로가는 태그
                        if (subServerName.Equals("hysys", StringComparison.CurrentCultureIgnoreCase))
                        {
                            opcGroup.ConnectedServerName = string.Empty;
                            opcGroup.ConnectedSubscriptionName = string.Empty;
                            opcGroup.Type = 0;

                            //ots datatable 추가.
                            DataTable dt = (DataTable)this.hypdb.Add(string.Format("ara_{0}_{1}_w", pc.UniqueID, serverName));
                            OTSDataTable odt = new OTSDataTable(pc)
                            {
                                DataTable = dt,
                                Type = 1,
                                ConnectedServerName = serverName,
                                ConnectedSubscriptionName = opcGroup.Name,
                                TagNames = new string[subMapping.Count],
                                TagUnits = new string[subMapping.Count],
                                TagValues = new double[subMapping.Count],
                            };
                            this.OTSWriteDataTableList.Add(odt);

                            //opc 그룹과 연결 테이블 정보
                            opcGroup.ConnectedDataTableIndex = this.OTSWriteDataTableList.IndexOf(odt);
                            opcGroup.ConnectedDataTableName = odt.Name;

                            //data table에 태그 추가.
                            for (int i = 0; i < subMapping.Count; i++)
                            {
                                MappingData md = subMapping[i];
                                string[] strarr = md.ToName.Split('.');
                                _IOperation obj = this.FindObject(strarr[0], strarr[1]);

                                if (obj != null)
                                {
                                    this.InsertDataInTable(obj, strarr[0], strarr[1], dt.VarDefinitions, DataTableAccessMode_enum.dtam_ReadWrite);
                                    odt.TagNames[i] = strarr[0];
                                    odt.TagUnits[i] = string.Empty;
                                    VarDefinition var = obj as VarDefinition;
                                    if (var != null) odt.TagUnits[i] = var.Units;
                                }
                            }
                        }

                        //opc item 추가 준비.
                        List<Opc.Da.Item> itemList = new List<Opc.Da.Item>();
                        for (int j = 0; j < subMapping.Count; j++)
                        {
                            MappingData md = subMapping[j];
                            itemList.Add(OPCServer.CreateOPCItem(md.FromName));
                        }

                        //read opc item 추가.
                        Opc.Da.Subscription group = opcGroup.Subscription;
                        Opc.Da.ItemResult[] itemResults = group.AddItems(itemList.ToArray());
                        for (int i = itemResults.Length - 1; i >= 0; i--)
                        {
                            Opc.Da.ItemResult result = itemResults[i];
                            if (result.ResultID.Failed())
                            {
                                this.PrintLog(string.Format("Error adding read items: {0}", result.ItemName));
                            }
                        }

                        //item 추가가 되었다면.
                        if (group.Items.Length > 0)
                        {
                            //서버의 Item중 식별된 Itemid를 찾는다.
                            Opc.Da.ItemPropertyCollection[] propertyLists = dasvr.GetProperties(group.Items, filters.PropertyIDs, filters.ReturnPropertyValues);
                            opcGroup.ItemTypes = new System.Type[propertyLists.Length];

                            //opc item count == propertyLists.length
                            for (int i = 0; i < propertyLists.Length; i++)
                            {
                                bool hasType = false;
                                for (int j = 0; j < propertyLists[i].Count; j++)
                                {
                                    if (propertyLists[i][j].Description.Equals("Item Value", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        opcGroup.ItemTypes[i] = propertyLists[i][j].DataType;
                                        hasType = true;
                                        break;
                                    }
                                }

                                if (hasType == false)
                                {
                                    opcGroup.ItemTypes[i] = typeof(double);
                                }
                            }
                        }
                    }



                    /*********************************************************************************/
                    // ots -> opc  
                    for (int subi = 0; subi < writeF.Count; subi++)
                    {
                        string subServerName = writeF[subi];

                        List<MappingData> subMapping = (from query in writeMapping.AsEnumerable()
                                                        where query.FromType == subServerName
                                                        select query).ToList();

                        OPCSubscription opcGroup = ns.AddGroup(string.Format("w_{0}", subServerName), 1);
                        opcGroup.ConnectedServerName = subServerName;
                        opcGroup.ConnectedSubscriptionName = string.Format("r_{0}", serverName);

                        //셀프 태그
                        if (serverName == subServerName) opcGroup.Type = 2;

                        //ots에서 받는 태그라면 data table 만들기.
                        if (subServerName.Equals("hysys", StringComparison.CurrentCultureIgnoreCase))
                        {
                            opcGroup.ConnectedServerName = string.Empty;
                            opcGroup.ConnectedSubscriptionName = string.Empty;
                            opcGroup.Type = 0;

                            DataTable dt = (DataTable)this.hypdb.Add(string.Format("ara_{0}_{1}_r", pc.UniqueID, serverName));
                            OTSDataTable odt = new OTSDataTable(pc)
                            {
                                DataTable = dt,
                                Type = 1,
                                ConnectedServerName = serverName,
                                ConnectedSubscriptionName = opcGroup.Name,
                                TagNames = new string[subMapping.Count],
                                TagUnits = new string[subMapping.Count],
                                TagValues = new double[subMapping.Count],
                            };
                            this.OTSReadDataTableList.Add(odt);

                            //index 넣기.
                            opcGroup.ConnectedDataTableIndex = this.OTSReadDataTableList.IndexOf(odt);
                            opcGroup.ConnectedDataTableName = odt.Name;

                            //태그추가
                            for (int i = 0; i < subMapping.Count; i++)
                            {
                                MappingData md = subMapping[i];
                                string[] strarr = md.FromName.Split('.');
                                UniSimDesign._IOperation obj = this.FindObject(strarr[0], strarr[1]);

                                if (obj != null)
                                {
                                    this.InsertDataInTable(obj, strarr[0], strarr[1], dt.VarDefinitions, DataTableAccessMode_enum.dtam_Read);

                                    odt.TagNames[i] = strarr[0];
                                    odt.TagUnits[i] = string.Empty;
                                    VarDefinition var = obj as VarDefinition;
                                    if (var != null) odt.TagUnits[i] = var.Units;
                                }
                            }
                        }


                        List<Opc.Da.Item> itemList = new List<Opc.Da.Item>();
                        for (int j = 0; j < subMapping.Count; j++)
                        {
                            MappingData md = subMapping[j];
                            itemList.Add(OPCServer.CreateOPCItem(md.ToName));
                        }

                        //read item 추가.
                        Opc.Da.Subscription group = opcGroup.Subscription;
                        Opc.Da.ItemResult[] itemResults = group.AddItems(itemList.ToArray());
                        for (int i = itemResults.Length - 1; i >= 0; i--)
                        {
                            Opc.Da.ItemResult result = itemResults[i];
                            if (result.ResultID.Failed())
                            {
                                this.PrintLog(string.Format("Error adding write items: {0}", result.ItemName));
                            }
                        }


                        if (group.Items.Length > 0)
                        {
                            //서버의 Item중 식별된 Itemid를 찾는다.
                            Opc.Da.ItemPropertyCollection[] propertyLists = dasvr.GetProperties(group.Items, filters.PropertyIDs, filters.ReturnPropertyValues);
                            opcGroup.ItemTypes = new System.Type[propertyLists.Length];

                            for (int i = 0; i < propertyLists.Length; i++)
                            {
                                bool hasType = false;
                                for (int j = 0; j < propertyLists[i].Count; j++)
                                {
                                    if (propertyLists[i][j].Description.Equals("Item Value", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        opcGroup.ItemTypes[i] = propertyLists[i][j].DataType;
                                        hasType = true;
                                        break;
                                    }
                                }
                                if (hasType == false)
                                {
                                    opcGroup.ItemTypes[i] = typeof(double);
                                }
                            }
                        }
                    }


                    for (int i = 0; i < this.OTSReadDataTableList.Count; i++)
                    {
                        this.OTSReadDataTableList[i].DataTable.StartTransfer();
                    }
                    for (int i = 0; i < this.OTSWriteDataTableList.Count; i++)
                    {
                        this.OTSWriteDataTableList[i].DataTable.StartTransfer();
                    }
                }
                catch (Exception ex)
                {
                    this.PrintLog(ex.StackTrace + "\n" + serverNames[maini] + ", " + dasvr.Name);
                    //svrNode.Image = FormBrowser.Instance.imagelist.Images[10];
                    svrNode.Image = Properties.Resources.monitor_error;
                    noError = false;
                    break;
                }
            }

            return noError;
        }

        public bool CheckMappingOTS(List<string> serverNames, List<MappingData> MappingList)
        {


            bool noError = true;
            int cnt = 0;
            StringBuilder strb = new StringBuilder();
            for (int maini = 0; maini < serverNames.Count; maini++)
            {
                //hysys->opc
                List<MappingData> readt = (from query in MappingList.AsEnumerable()
                                           where query.FromType.Equals("hysys", StringComparison.CurrentCultureIgnoreCase) &&
                                            query.ToType == serverNames[maini]
                                           select query).ToList();

                for (int i = 0; i < readt.Count; i++)
                {
                    OTSTagData b = this.OTSTagList.Find(x => x.GetFullTagName() == readt[i].FromName);
                    if (b == null)
                    {
                        noError = false;
                        strb.AppendFormat("-{0}\n", readt[i].FromName);
                        cnt++;
                    }
                }


                //opc->hysys
                List<MappingData> writet = (from query in MappingList.AsEnumerable()
                                            where query.ToType.Equals("hysys", StringComparison.CurrentCultureIgnoreCase) &&
                                                 query.FromType == serverNames[maini]
                                            select query).ToList();

                for (int i = 0; i < writet.Count; i++)
                {
                    OTSTagData b = this.OTSTagList.Find(x => x.GetFullTagName() == writet[i].ToName);
                    if (b == null)
                    {
                        noError = false;
                        strb.AppendFormat("-{0}\n", writet[i].FromName);
                        cnt++;
                    }
                }
            }

            if (noError == false)
            {
                if (cnt == 1)
                {
                    strb.Insert(0, "There is a wrong OTS Tagr\n");
                    this.PrintLog(strb.ToString());
                }
                else if (cnt > 1)
                {
                    strb.Insert(0, "There are some wrong OTS Tags\n");
                    this.PrintLog(strb.ToString());
                }
            }

            return noError;
        }

        public bool CheckMappingOPC(List<string> serverNames, List<MappingData> MappingList)
        {
            bool noError = true;
            int cnt = 0;
            StringBuilder strb = new StringBuilder();
            for (int maini = 0; maini < serverNames.Count; maini++)
            {
                OPCServer ns = null;
                ns = FindOPCServerByProgID(serverNames[maini]);
                if (ns == null) continue;

                List<MappingData> readt = (from query in MappingList.AsEnumerable()
                                           where query.FromType.Equals(serverNames[maini], StringComparison.CurrentCultureIgnoreCase)
                                           select query).ToList();




                for (int i = 0; i < readt.Count; i++)
                {
                    Opc.Da.BrowseElement b = ns.OPCItemlist.Find(x => x.ItemName == readt[i].FromName);
                    if (b == null)
                    {
                        noError = false;
                        strb.AppendFormat("-{0}\n", readt[i].FromName);
                        cnt++;
                    }
                }

                List<MappingData> writet = (from query in MappingList.AsEnumerable()
                                            where query.ToType.Equals(serverNames[maini], StringComparison.CurrentCultureIgnoreCase)
                                            select query).ToList();
                for (int i = 0; i < writet.Count; i++)
                {
                    Opc.Da.BrowseElement b = ns.OPCItemlist.Find(x => x.ItemName == writet[i].ToName);
                    if (b == null)
                    {
                        noError = false;
                        strb.AppendFormat("-{0}\n", writet[i].FromName);
                        cnt++;
                    }
                }
            }


            if (noError == false)
            {
                if (cnt == 1)
                {
                    strb.Insert(0, "There is a wrong OPC Tagr\n");
                    this.PrintLog(strb.ToString());
                }
                else if (cnt > 1)
                {
                    strb.Insert(0, "There are some wrong OPC Tags\n");
                    this.PrintLog(strb.ToString());
                }
            }
            return noError;
        }

        public bool CheckDatatable(string name)
        {
            if (this.hypdb.Count == 0) return false;

            bool exsist = false;
            for (int i = 0; i < this.hypdb.Count; i++)
            {
                DataTable dt = (DataTable)this.hypdb[i];

                if (dt.name == name)
                {
                    exsist = true;
                    break;
                }
            }

            return exsist;
        }

        public int CountOPCServer(PrivateController pc)
        {
            return GetOPCServers(pc).Count;
        }

        public List<OPCServer> GetOPCServers(PrivateController pc)
        {
            List<OPCServer> result = new List<OPCServer>();
            for (int i = 0; i < this.OPCServerList.Count; i++)
            {
                if (this.OPCServerList[i].Controller == pc)
                    result.Add(this.OPCServerList[i]);
            }
            return result;
        }

        public int CountOTSReadDatatable(PrivateController pc)
        {
            return GetOTSReadDatatables(pc).Count;
        }

        public List<OTSDataTable> GetOTSReadDatatables(PrivateController pc)
        {
            List<OTSDataTable> result = new List<OTSDataTable>();
            for (int i = 0; i < this.OTSReadDataTableList.Count; i++)
            {
                if (this.OTSReadDataTableList[i].Controller == pc)
                    result.Add(this.OTSReadDataTableList[i]);
            }
            return result;
        }

        public int CountOTSWriteDatatable(PrivateController pc)
        {
            return GetOTSWriteDatatables(pc).Count;
        }

        public List<OTSDataTable> GetOTSWriteDatatables(PrivateController pc)
        {
            List<OTSDataTable> result = new List<OTSDataTable>();
            for (int i = 0; i < this.OTSWriteDataTableList.Count; i++)
            {
                if (this.OTSWriteDataTableList[i].Controller == pc)
                    result.Add(this.OTSWriteDataTableList[i]);
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////
        public void BrowseChildren(OPCServer osg, Opc.Da.Server daserver, System.Collections.ObjectModel.Collection<Node> nodes, string itemId = null)
        {
            osg.OPCItemlist.Clear();
            ItemIdentifier itemID = new ItemIdentifier(itemId);
            Opc.Da.BrowsePosition position = null;
            Opc.Da.BrowseFilters _Filters = new Opc.Da.BrowseFilters()
            {
                BrowseFilter = browseFilter.all,
                ReturnAllProperties = true,
                ReturnPropertyValues = true,
            };
            Opc.Da.BrowseElement[] elements = daserver.Browse(itemID, _Filters, out position);

            foreach (Opc.Da.BrowseElement el in elements)
            {
                if (el.IsItem)
                {
                    Node subnode = new Node(el.Name) { Tag = el };
                    nodes.Add(subnode);
                    //subnode.Image = this.imageList1.Images[4];
                    subnode.Image = Properties.Resources.tag_blue;
                    osg.OPCItemlist.Add(el);
                }
                else
                {
                    //Node blanknode = new Node("blank") { Tag = 123 };
                    //subnode.Nodes.Add(blanknode);
                    FindFlatItem(osg, el, daserver, nodes);
                }
            }

        }
        private void FindFlatItem(OPCServer osg, BrowseElement el, Opc.Da.Server Server, System.Collections.ObjectModel.Collection<Node> nodes)
        {
            Node subnode = new Node(el.Name) { Tag = el };
            nodes.Add(subnode);
            if (el.IsItem)
            {
                //subnode.Image = this.imageList1.Images[4];
                subnode.Image = Properties.Resources.tag_blue;
                osg.OPCItemlist.Add(el);
            }
            else
            {
                Opc.Da.BrowsePosition position = null;
                ItemIdentifier identifier = new ItemIdentifier(el.ItemPath, el.ItemName);
                BrowseFilters _Filters = new Opc.Da.BrowseFilters()
                {
                    BrowseFilter = browseFilter.all,
                    ReturnAllProperties = true,
                    ReturnPropertyValues = true,
                };

                BrowseElement[] elements = Server.Browse(identifier, _Filters, out position);

                if (elements == null) return;

                foreach (BrowseElement item in elements)
                {
                    FindFlatItem(osg, item, Server, subnode.Nodes);
                }
            }
        }

        ////////////////////////////////////////////////////////////////
        //flowsheet 에서 이름으로 operation찾기
        public _IOperation FindObject(string name, string param)
        {
            _IOperation result = null;

            List<OTSTagData> list =
               (from query in this.OTSTagList.AsEnumerable()
                where query.TagName == name && query.Parameter == param
                select query).ToList();


            if (list.Count > 0)
            {
                result = list[0].op;
            }

            return result;
        }

        //datatable에 아이템 추가하기
        public void InsertDataInTable(_IOperation obj, string tagName, string param, VarDefinitions vars, DataTableAccessMode_enum acctype)
        {
            string type = obj.TypeName;

            if (type == "valveop")
            {
                Valve valve = (UniSimDesign.Valve)obj;
                int valvetype = (int)valve.ValveType;
                UnitConversionSet ucS1;
                UnitConversionType_enum ucType = valve.PercentOpen.UnitConversionType;
                ucS1 = ((UniSimDesign.Application)hyContainer.Flowsheet.Application).UnitConversionSetManager.GetUnitConversionSet(ucType);
                vars.Add(valve.PercentOpen, "Valve Current Position", tagName, ucS1.CurrentDisplayUnit.name, acctype);
            }
            else if (type == "fbcontrolop")
            {
                Controller fn = (Controller)obj;
                UnitConversionSet ucS1;
                UnitConversionType_enum ucType = fn.PV.UnitConversionType;
                ucS1 = ((UniSimDesign.Application)hyContainer.Flowsheet.Application).UnitConversionSetManager.GetUnitConversionSet(ucType);

                vars.Add(fn.PV, "PV", tagName, ucS1.CurrentDisplayUnit.name, acctype);
            }
            else if (type == "digitalop")
            {
                BackDoor bd;
                RealVariable hyrv;
                bd = (BackDoor)obj;
                hyrv = (RealVariable)bd.get_BackDoorVariable(":selection.501").Variable; //Output Stat

                vars.Add(hyrv, "OP State", tagName, System.Type.Missing, acctype);
            }
            else if (type == "selectionop")
            {
                SelectorBlock sb = (SelectorBlock)obj;
                vars.Add(sb.OutputVal, "Output Value", tagName, System.Type.Missing, acctype);
            }
            else
            {
                //this.readTable.VarDefinitions.Add(obj, "OP State", "test", "", 1);
            }
        }

        //ara datatable 초기화 
        public void InitDataTable()
        {
            for (int i = this.hypdb.Count - 1; i >= 0; i--)
            {
                UniSimDesign.DataTable tbl = this.hypdb[i] as UniSimDesign.DataTable;
                if (tbl.name.StartsWith("ara_"))
                {
                    tbl.EndTransfer();
                    this.hypdb.Remove(i);
                }
            }

            this.OTSReadDataTableList.Clear();
            this.OTSWriteDataTableList.Clear();
            for (int i = 0; i < OPCServerList.Count; i++)
            {
                this.OPCServerList[i].RemoveAllGroup(false);
            }
        }

        public void ResetControllerData(PrivateController pc)
        {

            for (int i = this.hypdb.Count - 1; i >= 0; i--)
            {
                UniSimDesign.DataTable tbl = this.hypdb[i] as UniSimDesign.DataTable;
                if (tbl.name.StartsWith(string.Format("ara_{0}", pc.UniqueID)))
                {
                    tbl.EndTransfer();
                    this.hypdb.Remove(i);
                }
            }

            for (int i = this.OTSReadDataTableList.Count - 1; i >= 0; i--)
            {
                OTSDataTable odt = this.OTSReadDataTableList[i];
                if (odt.Controller == pc)
                {
                    this.OTSReadDataTableList.RemoveAt(i);
                }
            }
            for (int i = this.OTSWriteDataTableList.Count - 1; i >= 0; i--)
            {
                OTSDataTable odt = this.OTSWriteDataTableList[i];
                if (odt.Controller == pc)
                {
                    this.OTSWriteDataTableList.RemoveAt(i);
                }
            }


            List<OPCServer> myServers = this.GetOPCServers(pc);
            for (int i = 0; i < myServers.Count; i++)
            {
                myServers[i].RemoveAllGroup(false);
            }

        }

        //opc tree 에서 node 찾기
        public Node FindServerNode(string nodename)
        {
            Node result = null;
            for (int i = 0; i < this.TreemodelOPC.Nodes.Count; i++)
            {
                Node child = this.TreemodelOPC.Nodes[i];

                if (child.Text == nodename)
                {
                    result = child;
                    break;
                }
                else
                {
                    if (child.Nodes.Count > 0)
                    {
                        Node subnode = FindServerNode(child.Nodes, nodename);
                        if (subnode != null)
                        {
                            result = subnode;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        private Node FindServerNode(System.Collections.ObjectModel.Collection<Node> nodes, string nodename)
        {
            Node result = null;

            for (int i = 0; i < nodes.Count; i++)
            {
                Node child = nodes[i];

                if (child.Text == nodename)
                {
                    result = child;
                    break;
                }
                else
                {
                    if (child.Nodes.Count > 0)
                    {
                        Node subnode = FindServerNode(child.Nodes, nodename);
                        if (subnode != null)
                        {
                            result = subnode;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        //opclist에서 server 찾기
        public OPCServer FindOPCServerByProgID(string progid)
        {
            OPCServer result = null;
            for (int i = 0; i < this.OPCServerList.Count; i++)
            {
                if (this.OPCServerList[i].Server.Name == progid)
                {
                    result = this.OPCServerList[i];
                    break;
                }
            }
            return result;
        }


        //현재 연결된 모든 opc server 아이템 지우기.
        public void RemoveOPCServers(PrivateController pc)
        {
            for (int i = OPCServerList.Count - 1; i >= 0; i--)
            {
                OPCServer osg = OPCServerList[i];

                if (osg.Controller == pc)
                {

                    osg.RemoveAllGroup(true);
                    osg.Server.Disconnect();
                    osg.Server.Dispose();

                    this.OPCServerList.RemoveAt(i);
                }
            }


            pc.IsApplyMapping = false; //RemoveOPCServers
        }

        //사용하지 않는 opc서버 그룹, 아이템 모두 지우기.
        public void DisconnectNotusingServer()
        {
            List<MappingData> MappingList = new List<MappingData>();
            for (int i = 0; i < this.Controllers.Count; i++)
            {
                MappingList.AddRange(this.Controllers[i].MappingList);
            }

            List<string> serverNames = (from query in MappingList.AsEnumerable()
                                        select query.FromType).ToList();

            List<string> serverNames2 = (from query in MappingList.AsEnumerable()
                                         select query.ToType).ToList();

            //서버이름 추리기.
            serverNames.AddRange(serverNames2);
            serverNames = serverNames.Distinct().ToList();
            serverNames.Sort();


            //ServerEnumerator discovery = new OpcCom.ServerEnumerator();
            //Opc.Server[] servers2 = discovery.GetAvailableServers(Specification.COM_DA_20);
            for (int i = OPCServerList.Count - 1; i >= 0; i--)
            {
                OPCServer osg = OPCServerList[i];

                if (serverNames.Contains(osg.Server.Name) == false)
                {

                    Node svrNode = this.FindServerNode(osg.Name);
                    svrNode.Nodes.Clear();
                    //svrNode.Image = FormBrowser.Instance.imagelist.Images[2];
                    svrNode.Image = Properties.Resources.server;
                    osg.RemoveAllGroup(false);

                }
            }
        }


        public bool ConnectOPCServer(string serverName, PrivateController pc)
        {
            bool result = false;
            //tree에서 노드 찾기.
            Node svrNode = this.FindServerNode(serverName);

            if (svrNode == null) return false;
            if ((svrNode.Tag is Opc.Server) == false) return false;

            Opc.Server svr = (Opc.Server)svrNode.Tag;
            Opc.Da.Server dasvr = (Opc.Da.Server)svr;

            OPCServer ns = null;
            ns = FindOPCServerByProgID(serverName);
            if (ns == null) //없으면 새로 추가.
            {
                ns = new OPCServer(pc);
                ns.SetServer(dasvr);
                this.OPCServerList.Add(ns);
            }
            else
            {
                dasvr = ns.Server;
            }


            if (dasvr.IsConnected == false)
            {
                if (ns.Connect())
                {
                    this.BrowseChildren(ns, dasvr, svrNode.Nodes, null);
                    //svrNode.Image = FormBrowser.Instance.imagelist.Images[3];
                    svrNode.Image = Properties.Resources.server_link;

                    pc.SetStatus(extensionStatus.Connected);
                }
                else
                {
                    this.PrintLog("can not connect");
                    result = false;
                }
            }
            result = true;



            return result;
        }

        //현재 pc의 opc서버 목록에서 OLGA OPC Server 찾기
        public Opc.Server FindOLGAOPCServer(string OPCServerName)
        {
            Opc.Server result = null;

            using (ServerEnumerator discovery = new OpcCom.ServerEnumerator())
            {
                Opc.Server[] servers2 = discovery.GetAvailableServers(Specification.COM_DA_20, null, null);
                if (servers2 != null)
                {
                    foreach (Opc.Server server in servers2)
                    {
                        if (server.Name.StartsWith(OPCServerName))
                        {
                            result = server;
                            break;
                        }
                    }
                }
            }

            return result;
        }








    }
}
