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
using Opc;
using UniSimDesign;

using Opc.Da;
using System.Text.RegularExpressions;

namespace ARAUniSimSIMBridge
{
    /// <summary>
    /// Extension 상태 구분.
    /// </summary>
    public enum extensionStatus
    {
        /// <summary>
        /// 연결 해제, 에러, 처음시작
        /// </summary>
        Disconnected,
        /// <summary>
        /// 연결 성공
        /// </summary>
        Connected,
    }
    /// <summary>
    /// Extension 각각을 컨트롤.
    /// </summary>
    public class PrivateController
    {
        /// <summary>
        /// 데이터 교환 mapping list.
        /// </summary>
        public List<MappingData> MappingList { get; set; }

        /// <summary>
        /// local 존재하는 opcserver 목록
        /// </summary>
        public InternalTextFlexVariable txtLocalServers { get; set; }
        /// <summary>
        /// newtwork 상에 존재하는 opcserver 목록
        /// </summary>
        public InternalTextFlexVariable txtNetworkServers { get; set; }
        /// <summary>
        /// extension의 고유 ID
        /// </summary>
        private InternalTextVariable txtUniqueID;
        /// <summary>
        /// extension의 고유 ID
        /// </summary>
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



        /// <summary>
        /// extension 구분 하기 위한 생성 날짜.
        /// </summary>
        private InternalTextVariable txtCreateDate;


        /// <summary>
        /// opc server 선택 구분
        /// </summary>
        public InternalRealVariable dblOPCServerSelected;

        /// <summary>
        /// 현재 선택한 network opc server
        /// </summary>
        private InternalTextVariable txtNetworkServerSelected;
        /// <summary>
        /// 현재 선택한 local opc server
        /// </summary>
        public InternalTextVariable txtLocalServerSelected;



        /// <summary>
        /// olga opc인 경우 불러올 snapshot 파일 경로
        /// </summary>
        public InternalTextVariable txtOLGASnapshot { get; set; }
        /// <summary>
        /// olga opc인 경우 실행 model genkey 파일 경로
        /// </summary>
        public InternalTextVariable txtOLGAModel { get; set; }
        /// <summary>
        /// olga opc인 경우 olga 실행 파일 경로
        /// </summary>
        private InternalTextVariable txtOLGAExecutable;

        /// <summary>
        /// mapping list 에서 보내는쪽 type
        /// </summary>
        private InternalTextFlexVariable txtFromTypes;
        /// <summary>
        /// mapping list 에서 보내는쪽 tagname
        /// </summary>
        private InternalTextFlexVariable txtFromNames;
        /// <summary>
        /// mapping list 에서 받는쪽 type
        /// </summary>
        private InternalTextFlexVariable txtToTypes;
        /// <summary>
        /// mapping list 에서 받는쪽 tagname
        /// </summary>
        private InternalTextFlexVariable txtToNames;

        /// <summary>
        /// opc서버 연결상태 구분
        /// </summary>
        public InternalRealVariable dblConnectedOPC { get; set; }
        /// <summary>
        /// 현재 시뮬레이션 동작중인지 구분
        /// </summary>
        public InternalRealVariable dblRunningSim { get; set; }



        /// <summary>
        /// data 교환 주기
        /// </summary>
        public InternalRealVariable dblOLGARunInterval { get; set; }
        /// <summary>
        /// ots 실행 배속.
        /// </summary>
        public InternalRealVariable dblRealTimeFactor { get; set; }
        /// <summary>
        /// 이전에 사용된 ots 실행 배속
        /// </summary>
        public double preRealTimeFactor { get; set; }


        /// <summary>
        /// olga opc server 인경우 olga에서 표시되는 message
        /// </summary>
        public InternalTextVariable txtOLGAMessage { get; set; }
        /// <summary>
        /// 현재 extension의 상태를 문자로 표시하기위한 변수
        /// </summary>
        public InternalTextVariable txtSimStatus { get; set; }
        /// <summary>
        /// 현재 extension의 상태를 색상으로 표시하기위한 변수
        ///-1 dark green
        ///0 red
        ///1 green
        ///2 blue
        ///3 pink
        ///4 sky
        ///5 gray
        ///6 black
        ///7 yellow
        ///8 white 
        /// </summary>
        public InternalRealVariable dblSimStatus { get; set; }



        /// <summary>
        /// 컴퓨터 시간 표시
        /// </summary>
        public InternalTextFlexVariable txtCPUClockTimes { get; set; }
        /// <summary>
        /// ots 시간 표시
        /// </summary>
        public InternalTextFlexVariable txtUniSimTimes { get; set; }
        /// <summary>
        /// olga opc server 시간 표시.
        /// </summary>
        public InternalTextFlexVariable txtOLGATimes { get; set; }



        /// <summary>
        /// data교환에 사용되는 thread
        /// </summary>
        public Thread threadDataExchange { get; set; }


        /// <summary>
        /// 현재 extension 상태 표시.
        /// </summary>
        public extensionStatus status = extensionStatus.Disconnected;

        /// <summary>
        /// mppaing list xml 저장 경로
        /// </summary>
        private string MappingPath;

        /// <summary>
        /// 실제로 데이터 ots-opc 데이터 교환중인지
        /// </summary>
        public bool IsStartDataExchange { get; set; }

        /// <summary>
        /// mapping list가 제대로 datatable, opc group에 적용 됐는지
        /// </summary>
        public bool IsApplyMapping { get; set; }

        /// <summary>
        /// unisimDesign을 제어하기위한 변수
        /// </summary>
        private ExtnUnitOperationContainer hyContainer;

        /// <summary>
        /// 생산자.
        /// </summary>
        public PrivateController()
        {
            this.MappingList = new List<MappingData>();
        }

        /// <summary>
        /// 기본 설정
        /// </summary>
        /// <param name="hyContainer">ots container</param>
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

            this.txtOLGAMessage = (InternalTextVariable)hyContainer.FindVariable("txtOLGAMessage").Variable;



            this.SetStatus(extensionStatus.Disconnected); //초기설정

            CommonController.Instance.RegisterController(this);
            CommonController.Instance.SetContainer(hyContainer);

            this.txtLocalServers.SetBounds(CommonController.Instance.OPCLocalServerNames.Count);
            this.txtLocalServers.Values = CommonController.Instance.OPCLocalServerNames.ToArray();
        }


        /// <summary>
        /// local opc server 목록에서 선택했을 경우 이벤트 처리.
        /// </summary>
        public void SelectLocalServer()
        {
            //CommonController.Instance.PrintLog(txtLocalServerSelected.Value + ", " + this.GetOPCServerName());
            if (this.txtLocalServerSelected.Value.Equals("OLGA OPCServer"))
            {
                this.dblOPCServerSelected.Value = 20; //olga 서버 선택
            }
            else
            {
                this.dblOPCServerSelected.Value = 22; //olga 이외 다른서버 선택
            }
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

        /// <summary>
        /// Olga model 저장하기.
        /// </summary>
        /// <param name="path">저장경로</param>
        /// <returns>true 성공, false 실패</returns>
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
                        Thread.Sleep(10);
                        os.ItemValues = cg.Read(cg.Items);
                    }

                    result = true;
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// Olga model 불러오기
        /// </summary>
        /// <param name="path">경로</param>
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
        /// <summary>
        /// 내문서에 위치한 기본 작업 경로 설정.
        /// </summary>
        public void SetBaseDocument()
        {
            //새로 추가 된 extension인지 파악하기.
            if (string.IsNullOrEmpty(this.txtCreateDate.Value))
            {
                //새로 추가된 extension.
                this.txtCreateDate.Value = DateTime.Now.ToString("yyyyMMddhhmmssff");//새로만들어짐

                //기존에 있던 operation의 자료는 삭제한다.
                CommonController.Instance.DeleteOldFiles(this.hyContainer.name);
            }
            else
            {
                //이미 만들어진 id 
            }



            if (string.IsNullOrEmpty(this.txtUniqueID.Value))
            {
                this.txtUniqueID.Value = this.hyContainer.name;
            }

            this.PathModelConfigXML = string.Format("{0}\\{1}.xml", CommonController.Instance.PathModelWorkDirectory, this.txtUniqueID.Value);
            this.PathLeastmappingXML = string.Format("{0}\\{1}_Mapping.xml", CommonController.Instance.PathModelWorkDirectory, this.txtUniqueID.Value);
        }


        /// <summary>
        /// Olga, ots 저장하기.
        /// </summary>
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

        /// <summary>
        /// mapping 편집하기.
        /// </summary>
        public void ShowMappingEditor()
        {
            FormMapping.Instance.SetController(this);
            FormMapping.Instance.UpdateTable();

            FormMapping.Instance.ShowDialog();

            if (FormMapping.Instance.ResultOK)
            {
                this.IsApplyMapping = false; //ShowBrowser 내용변경
                this.SetMappingTable(); //show browser
            }
        }


        /// <summary>
        /// 모니터 화면 열기.
        /// </summary>
        public void ShowMonitor()
        {
            FormMonitor.Instance.SetMonitor(this);
            FormMonitor.Instance.Show();
        }



        /// <summary>
        /// extension 삭제시 데이터 정리하기.
        /// </summary>
        public void RemoveData()
        {
            //common controll에서 현재 extension 등록 해제함.
            CommonController.Instance.UnregisterController(this);

            //연결중인 opc
            this.KillMyOLGA();

            try
            {
                this.IsStartDataExchange = false; //RemoveData
                this.threadDataExchange.Abort();
                this.threadDataExchange = null;
            }
            catch { }
        }



        /// <summary>
        /// mapping list 불러오기.
        /// </summary>
        /// <param name="path">경로</param>
        public void LoadMappingList(string path)
        {
            bool isLoad = false;
            if (string.IsNullOrEmpty(path)) //config 파일에서 호출한경우 OpenFileDialog로 경로 찾기.
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
            this.IsApplyMapping = false; //load 내용변경
        }


        /// <summary>
        /// mapping list 모두 지우기.
        /// </summary>
        public void ResetMappingList()
        {
            this.IsApplyMapping = false; // reset 내용 지우기
            this.MappingList.Clear();
            this.SetMappingTable();
        }


        /// <summary>
        /// Extension Save
        /// </summary>
        public void Save()
        {
            string olgasnapfile = string.Format("{0}\\{1}", CommonController.Instance.PathModelSnapshotDirectory, this.UniqueID);
            if (this.SaveOLGASnapshot(olgasnapfile))
            {
                this.txtOLGASnapshot.Value = olgasnapfile + ".rsw";
            }

            this.SaveMappingList(true);
            this.SaveConfiguration();

            if (CommonController.Instance.GetControllerIndex(this) == 0)
                CommonController.Instance.DeleteNotUsingController(); //stop일때
        }

        /// <summary>
        /// 현재 mapping list 저장하기.
        /// </summary>
        /// <param name="autoSave"></param>
        public void SaveMappingList(bool autoSave = false)
        {
            if (this.MappingList.Count == 0) return;

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
                        this.dblOPCServerSelected.Value = 20; //olga 선택
                    }
                    else
                    {
                        this.dblOPCServerSelected.Value = 22; //olga 이외 서버 선택
                    }

                    this.PreType = this.dblOPCServerSelected.Value;

                    this.dblRealTimeFactor.Value = cf.Foctor;
                    this.preRealTimeFactor = cf.Foctor;

                    this.dblOLGARunInterval.Value = cf.RunInterval;
                }
            }
        }



        /// <summary>
        /// OLGA OPC Server에서 쓰이는 Model name 
        /// </summary>
        public string OLGAModelName { get; set; }
        /// <summary>
        /// OLGA OPC Server name.
        /// </summary>
        public string OPCServerName { get; set; }

        /// <summary>
        /// genkey file에서 OLGA opc server 정보 추출하기.
        /// </summary>
        /// <param name="path">경로</param>
        private void GetModelINfoFromGenkey(string path)
        {
            //기본 내용으로 초기화
            this.OPCServerName = "OLGAOPCServer";
            this.OLGAModelName = "ServerDemo";

            //파일읽기 시작
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

                if (check) //server에 관련된 내용만 추려내기.
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

            //같은 이름의 모델을 사용하지는 체크
            if (CommonController.Instance.CheckDuplicatedOLGAOPCServer(this))
            {
                CommonController.Instance.PrintLog(string.Format("OLGA OPC Server {0} is duplicated", this.OPCServerName));
                this.OPCServerName = string.Empty;
                this.OLGAModelName = string.Empty;
                this.txtOLGAModel.Value = string.Empty;
            }

        }

        /// <summary>
        /// extension 설정 환경 xml로 저장하기.
        /// </summary>
        public void SaveConfiguration()
        {

            if (string.IsNullOrEmpty(this.OPCServerName))
            {
                return;
            }

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


        /// <summary>
        /// mapping list를 datatable, opc group에 적용하기.
        /// </summary>
        /// <returns></returns>
        private bool ApplyMappingTable()
        {
            bool noError = true;

            //현재 연결된 OPC서버와 관련없는 Mapping data 삭제
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
                this.IsApplyMapping = false; //체크 에러
                this.SetMappingTable();
            }

            //else
            {
                if (CommonController.Instance.AddMappingOPCOTS(serverNames, this.MappingList, this) == false)
                {
                    noError = false;
                    this.IsApplyMapping = false; //적용중 에러
                }
                else
                {
                    noError = true;
                    this.IsApplyMapping = true; // 적용 완료
                }
            }

            return noError;
        }

        /// <summary>
        /// 현재 연결 된 opc server에 없는 mapping 정보가 있는지 파악하며 없으면 삭제한다.
        /// </summary>
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

        /// <summary>
        /// edf matrix에 mapping list 내용 추가.
        /// </summary>
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
        /// OLGA 실행파일 경로 설정.
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

        /// <summary>
        /// OLGA 스냅샷 파일 경로 설정.
        /// </summary>
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


        /// <summary>
        /// 서버가 변경 됐는지 체크하기 위한 이전 연결내역.
        /// </summary>
        public double PreType = -99;

        /// <summary>
        /// OPC Server연결.
        /// </summary>
        /// <returns>true 성공, false 실패</returns>
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
                this.IsApplyMapping = false; // 재연결
                //CommonController.Instance.PrintLog("server is changed");
            }

            PreType = type;
            return result;
        }


        /// <summary>
        /// OLGA 이외 다른 OPC Server 연결 하기.
        /// </summary>
        /// <returns>true 성공, false 실패</returns>
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

        /// <summary>
        /// OLGA OPC Server 연결
        /// </summary>
        /// <returns>true 성공, false 실패</returns>
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

            //olga opc server 실행
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
                if (loadSnapshot)
                    this.LoadOLGASanpshot(this.txtOLGASnapshot.Value);

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

        /// <summary>
        /// OPC Server 연결 끊기.
        /// </summary>
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

        /// <summary>
        /// 현재 extension 상태 표시.
        /// </summary>
        /// <param name="status"></param>
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


        /// <summary>
        /// 연결된 olga opc server process
        /// </summary>
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

        /// <summary>
        /// extension과 연결중인 olga opc server 프로세스 종료하기.
        /// </summary>
        /// <returns>true 성공, false 실패</returns>
        public bool KillMyOLGA()
        {
            bool result = true;
            if (this.OLGAProcess != null)
            {
                try
                {
                    //olga opc server 종료
                    this.OLGAProcess.Kill();

                    //olga opc server 등록 해제하기
                    Process process = new Process();
                    ProcessStartInfo cmd = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = this.txtOLGAExecutable.Value,
                        CreateNoWindow = true,
                        UseShellExecute = true,
                    };

                    if (string.IsNullOrEmpty(this.OPCServerName) == false)
                    {
                        cmd.Arguments = string.Format("-unreg {0}", this.OPCServerName);
                        process.StartInfo = cmd;
                        process.Start();
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

        /// <summary>
        /// 모든 olga opcserver 종료시키기
        /// </summary>
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



            Process process2 = new Process();
            ProcessStartInfo cmd2 = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = this.txtOLGAExecutable.Value,
                CreateNoWindow = true,
                UseShellExecute = true,
            };
            Opc.Server[] servers2 = new OpcCom.ServerEnumerator().GetAvailableServers(Specification.COM_DA_20);
            if (servers2 != null)
            {
                foreach (Opc.Server server in servers2)
                {
                    //if (server.Name.StartsWith("SPT."))
                    if (server.Name.StartsWith(this.OPCServerName))
                    {
                        //Process.Start(this.txtOLGAExecutable.Value, "-unreg " + server.Name);
                        cmd2.Arguments = string.Format("-unreg {0}", this.OPCServerName);
                        process2.StartInfo = cmd2;
                        process2.Start();
                    }
                }
            }
        }

        /// <summary>
        /// ots 시작 준비.
        /// </summary>
        /// <returns>ots시작 준비 완료 여부</returns>
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

            //BackDoor bd = (UniSimDesign.BackDoor)CommonController.Instance.integrator;
            //this.CalcRealtimeFactor(((RealVariable)bd.get_BackDoorVariable(":ExtraData.107").Variable).Value);
            this.CalcRealtimeFactor(dblRealTimeFactor.Value);

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

        /// <summary>
        /// 데이터 교환 시작
        /// </summary>
        public void StartSimulation()
        {
            //데이터 교환 시작.
            this.IsStartDataExchange = true; //start 
            this.threadDataExchange = new Thread(procDataExcahnge);
            this.threadDataExchange.IsBackground = true;
            this.threadDataExchange.Name = "DataExchange";
            this.threadDataExchange.Start();
        }

        /// <summary>
        /// 데이터 교환 중지
        /// </summary>
        /// <returns></returns>
        public void StopSimulation()
        {
            this.IsStartDataExchange = false; //stop 
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

        /// <summary>
        /// Data 교환 주기
        /// </summary>
        public int RunInterval = 1;
        private int baseAccessTime = 500;
        private int accessTime = 1;
        private bool isControlMain = false;


        private List<OPCServer> myOPCServers = null;
        private List<OTSDataTable> myReadDTs = null;
        private List<OTSDataTable> myWriteDTs = null;
        private TimeSpan cycleElapsed;
        /// <summary>
        /// 데이터 교환.
        /// </summary>
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
                while (IsStartDataExchange)
                {
                    cycleStart = DateTime.Now;

                    this.DataExchange();
                    this.TotalExecuteSteps++;

                    this.cycleElapsed = DateTime.Now - cycleStart;

                    this.ElapsedTimes.Add((float)this.cycleElapsed.TotalMilliseconds);
                    if (this.ElapsedTimes.Count > 600)
                        ElapsedTimes.RemoveAt(0);

                    this.CalcAccesstime();
                    Thread.Sleep(accessTime);

                    //스텝 표시 
                    //new Thread(() => this.UpdateTImes()).Start();
                    this.UpdateTImes();
                }
            }
            catch { }
            //try catch 중요함!!!
            //error 발생시 ots가 강제 종료 되므로 반드시 try catch로 묶에 예외 처리해함.
        }


        /// <summary>
        /// cpu, ots, opcserver 시간 표시하기.
        /// </summary>
        /// <param name="isFirst">맨처음 시간 표시여부.</param>
        private void UpdateTImes(bool isFirst = false)
        {
            try
            {
                if (isFirst)
                {
                    this.CPUStartTime = DateTime.Now;
                    this.OTSStartTime = CommonController.Instance.integrator.CurrentTime.Value;
                }

                //olga opc srever 시간 가져오기.
                for (int i = 0; i < this.myOPCServers.Count; i++)
                {
                    OPCServer osg = this.myOPCServers[i];
                    if (osg.Server.Name.StartsWith("SPT.")) //olga 서버 가져오기.
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


                if (CommonController.Instance.IsDebug)
                {
                    //step info 
                    double expectedTime = (TotalExecuteSteps * this.StepSize * this.RunInterval) / 1000.0f;

                    //this.txtConnectedServer.Value = string.Format("{0}", this.OPCServerName);
                    this.txtOLGAMessage.Value = string.Format("[{0}] {1} {2}={3}  {4}ms, {5}, {6}, {7}, {8}, {9}",
                       CommonController.Instance.GetControllerIndex(this), //0
                       this.UniqueID, //1
                       TotalExecuteSteps * this.RunInterval, //2
                       this.ConvertTime(expectedTime), //3
                       this.StepSize, //4
                       this.RunInterval, //5
                       this.dblRealTimeFactor.Value,//6
                       this.baseAccessTime,//7
                       this.StepSize * this.RunInterval, //8
                       cycleElapsed.TotalMilliseconds //9
                       );
                }


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

                //다른 extension 시간 update
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

        /// <summary>
        /// hh:mm:ss 형식으로 변환
        /// </summary>
        /// <param name="dsec">초</param>
        /// <returns>변환된 hh:mm:ss</returns>
        public string ConvertTime(double dsec)
        {
            int sec = (int)dsec;

            int hour = sec / (60 * 60);
            sec %= (60 * 60);

            int min = sec / 60;
            sec %= 60;

            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, min, sec);
        }

        /// <summary>
        /// data교환에만 사용된 시간.
        /// </summary>
        public List<float> ElapsedTimes = new List<float>();
        /// <summary>
        /// 수정된 정지 시간
        /// </summary>
        public List<int> AccessTimes = new List<int>();
        /// <summary>
        /// 1 cycle을 수행하는데 사용된 시간.
        /// </summary>
        public List<float> GapTimes = new List<float>();
        private double StepSize = 0;
        /// <summary>
        /// 코드 실행시간 측정하고 계산하여 최대한 오차를 줄임.
        /// </summary>
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

        /// <summary>
        /// 배속 적용
        /// </summary>
        /// <param name="RTF"></param>
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

            try
            {
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
            }
            catch { }

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
            while (CommonController.Instance.integrator.StepsToExecute > 0)
            {
                Thread.Sleep(5);
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


            /*
            ManualResetEvent[] doEvent = new ManualResetEvent[] { new ManualResetEvent(false) };
            this.DataExchangeOTSSelf(doEvent[0]);
            this.DataExchangeOPC(null);
            this.DataExchangeOTS(null);*/



            //ThreadPool.QueueUserWorkItem(this.DataExchangeStep);
            this.DataExchangeStep(null);

            //ots excution.
            if (this.isControlMain)
            {
                CommonController.Instance.integrator.StepsToExecute += RunInterval;
            }
        }
    }
}