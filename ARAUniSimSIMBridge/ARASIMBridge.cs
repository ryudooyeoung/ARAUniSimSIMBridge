using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UniSimDesign;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using ARAUniSimSIMBridge.Data;
using System.Diagnostics;
using System.Threading;


namespace ARAUniSimSIMBridge
{
    /// <summary>
    /// Extension 기본 구성 class
    /// </summary>
    [ComVisible(true)]
    [ProgId("ARAUniSimSIMBridge.ARASIMBridge")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class ARASIMBridge
    {
        /// <summary>
        /// 기본 OTS Container
        /// </summary>
        private ExtnUnitOperationContainer hyContainer = null;
        private ExtnDynUnitOpContainer dynContainer = null;

        /// <summary>
        /// 
        /// </summary>
        private PrivateController controller = null;
        private bool isLoaded = false;

        /// <summary>
        /// 맨처음 초기화
        /// </summary> 
        public int Initialize(ExtnUnitOperationContainer Container, bool IsRecalling)
        {
            this.hyContainer = Container;

            this.controller = new PrivateController();
            this.controller.SetContainer(this.hyContainer);

            if (IsRecalling) { }
            return (int)CurrentExtensionVersion_enum.extnCurrentVersion;
        }

        /// <summary>
        /// extension 필수 함수
        /// </summary> 
        public void Execute(bool isForgetpass)
        {
            if (isForgetpass) { return; }
        }

        /// <summary>
        /// extension 필수 함수
        /// </summary> 
        public void StatusQuery(ObjectStatus Status)
        {
            try
            {
                if (hyContainer.IsIgnored) { return; }

                if (this.isLoaded == false)
                {
                    //((_IOperation)Status.Parent).name = "ARA SIMBridge";
                    this.controller.SetBaseDocument();
                    this.controller.LoadConfiguration();
                    this.isLoaded = true;
                }
            }
            catch { }
        }

        /// <summary>
        /// EDF 파일(디자인폼)에서 버튼이나 내용변경에 따른 이벤트를
        /// message(string)로 구분하여 처리한다.
        /// </summary>
        /// <param name="Variable"></param>
        public void VariableChanged(InternalVariableWrapper Variable)
        {
            if (Variable.Tag == "msgLoadOLGAEXE") //올가 실행경로 불러오기
            {
                this.controller.LoadOLGAExecutable();
            }
            else if (Variable.Tag == "msgLoadOLGASnapshot")//올가 스냅샷
            {
                this.controller.LoadOLGASnapshot();
            }
            else if (Variable.Tag == "msgLoadOLGAModel")//올가 genkey 경로
            {
                this.controller.LoadOLGAModel();
            }
            else if (Variable.Tag == "msgDisconnectOPC") //opc 서버 연글 끊기
            {
                this.controller.DisconnectOPCServer();
            }
            else if (Variable.Tag == "msgConnectOPC") //opc 서버 실행
            {
                this.controller.ConnectOPCServer();
            }

            else if (Variable.Tag == "msgLoadMapping") //태그 매핑 불러오기
            {
                this.controller.LoadMappingList(string.Empty);
            }
            else if (Variable.Tag == "msgSaveMapping") //태그 매핑 저장
            {
                this.controller.SaveMappingList();
            }
            else if (Variable.Tag == "msgResetMapping") //태그 매핑 clear
            {
                this.controller.ResetMappingList();
            }

            else if (Variable.Tag == "msgMonitor")
            {
                this.controller.ShowMonitor(); //속도,값 확인하기.
            }
            else if (Variable.Tag == "msgAddMapping") //태그 매핑 만들기.
            {
                this.controller.ShowMappingEditor();
            }
            else if (Variable.Tag == "msgSnapshot") //스탭샷 저장.
            {
                this.controller.TakeSnapshot(); //olga snapshot 저장.
            }
            else if (Variable.Tag == "msgLocalServers") { this.controller.SelectLocalServer(); }
        }


        /// <summary>
        /// Extension 기본 함수
        /// </summary> 
        public bool VariableChanging(InternalVariableWrapper variable)
        {
            bool isOK = true;
            //CommonController.Instance.PrintLog(variable.Tag);
            if (variable.Tag == "txtLocalServerSelected")
            {
                //this.controller.SelectLocalServer(); //server
                 
            }
            return isOK;
        }


        #region functions for dynamic extension
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public void DynInitialize(ExtnDynUnitOpContainer pContainer, bool IsRecalling, long MyVersion, bool HoldupExist)
        {
            this.dynContainer = pContainer;
            HoldupExist = false;
            MyVersion = (int)CurrentExtensionVersion_enum.extnCurrentVersion;
        }

        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public bool InitializeSystem(bool ForceInit)
        {
            return true;
        }

        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public long NumberOfFlowEquations()
        {
            return 0;
        }

        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public long NumberOfPressBalEquations()
        {
            return 0;
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public long NumberOfFlowBalEquations()
        {
            return 0;
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public long NumberOfGeneralEquations()
        {
            return 0;
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public bool PreProcessStates(double Dtime)
        {
            return true;
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public bool PostProcessStates(double Dtime)
        {
            return true;
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public bool StepEnergyExplicitly(double Dtime)
        {
            return true;
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public bool StepCompositionExplicitly(double Dtime)
        {
            return true;
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public void CoefficientsOfFlowEquations(out object k1, out object k2)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public object FlowInFlowEquations()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public void ReferencePressureInFlowEquations(out object p1, out object p2)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public void UpdateCoefficientsOfFlowEquations(double Dtime, out object k1, out object k2)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// dynamic extension 필수 함수
        /// </summary> 
        public object UpdateDensities(double Dtime)
        {
            throw new Exception("The method or operation is not implemented.");
        }




        /// <summary>
        /// Extension 기본 함수
        /// </summary> 
        public void VariableQuery(InternalVariableWrapper variable)
        {
            //Not implemented
        }

        /// <summary>
        /// Extension 기본 함수
        /// </summary> 
        public bool OnHelp(ref string helpPanel)
        {
            return true;
        }

        /// <summary>
        /// edf 화면 호출시 자동 호출, 여기에 다른 작업을 선행 할 수 있다.
        /// false를 반환하면 화면 뜨지않음.
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public bool OnView(ref string viewName)
        {
            return true;
        }
        #endregion


        /// <summary>
        /// Extension 기본 함수, 모델 저장시 자동 호출
        /// </summary>
        public void Save()
        {
            this.controller.Save();
        }


        /// <summary>
        /// extension 종료시 호출 되는 함수.
        /// 1. extension 삭제시
        /// 2. unisim 종료시
        /// </summary>
        public void Terminate()
        {
            this.controller.RemoveData();

            Marshal.FinalReleaseComObject(hyContainer);
            hyContainer = null;
            Marshal.FinalReleaseComObject(dynContainer);
            dynContainer = null;
        }
    }
}