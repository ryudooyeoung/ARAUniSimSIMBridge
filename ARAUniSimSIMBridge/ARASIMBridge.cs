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
    [ComVisible(true)]
    [ProgId("ARAUniSimSIMBridge.ARASIMBridge")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]

    /*
    -1 dark green
    0 red
    1 green
    2 blue
    3 pink
    4 sky
    5 gray
    6 black
    7 yellow
    8 white
     */
    public class ARASIMBridge
    {
        private ExtnUnitOperationContainer hyContainer;
        private ExtnDynUnitOpContainer dynContainer;

        private PrivateController controller = null;
        bool isLoaded = false;
        public ARASIMBridge()
        {
        }

        ~ARASIMBridge()
        {
        }

        public int Initialize(ExtnUnitOperationContainer Container, bool IsRecalling)
        {
            this.hyContainer = Container;
            this.controller = new PrivateController();
            this.controller.SetContainer(this.hyContainer);

            if (IsRecalling) { }

            return (int)CurrentExtensionVersion_enum.extnCurrentVersion;
        }

        public void Execute(bool isForgetpass)
        {
            //hyContainer.Trace("Execute ", false); 
            if (isForgetpass == true) { return; }
        }

        ///////   Shared Methods
        public void StatusQuery(ObjectStatus Status)
        {
            try
            {

                bool ignore = ((_IOperation)hyContainer.ExtensionInterface).IsIgnored;
                if (ignore == true) { return; }
                //((_IOperation)Status.Parent).name = "ARA SIMBridge";

                if (this.isLoaded == false)
                {
                    this.controller.SetBaseDocument();
                    this.controller.LoadConfiguration();
                    this.isLoaded = true;
                }
            }
            catch { }
        }

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
                this.controller.ShowMonitor();
            }
            else if (Variable.Tag == "msgAddMapping") //태그 매핑 만들기.
            {
                this.controller.ShowMappingEditor();
            }
            else if (Variable.Tag == "msgSnapshot") //스탭샷 저장.
            {
                this.controller.TakeSnapshot();
            }

            else if (Variable.Tag == "msgLocalServers")
            {
                this.controller.SelectLocalServer();
            }
        }

        ///////  about dynamic method 
        public void DynInitialize(ExtnDynUnitOpContainer pContainer, bool IsRecalling, long MyVersion, bool HoldupExist)
        {
            dynContainer = pContainer;
            HoldupExist = false;
            MyVersion = (int)CurrentExtensionVersion_enum.extnCurrentVersion;


        }

        public bool InitializeSystem(bool ForceInit)
        {
            //this.PrintLog("InitializeSystem");
            return true;
        }

        public long NumberOfFlowEquations()
        {
            //this.PrintLog("NumberOfFlowEquations");
            return 0;
        }

        public long NumberOfPressBalEquations()
        {
            //this.PrintLog("NumberOfPressBalEquations");
            return 0;
        }

        public long NumberOfFlowBalEquations()
        {
            //this.PrintLog("NumberOfFlowBalEquations");
            return 0;
        }

        public long NumberOfGeneralEquations()
        {
            // this.PrintLog("NumberOfGeneralEquations");
            return 0;
        }

        public bool PreProcessStates(double Dtime)
        {
            //this.PrintLog("PreProcessStates");
            return true;
        }

        public bool PostProcessStates(double Dtime)
        {
            //this.PrintLog("PostProcessStates");
            return true;
        }

        public bool StepEnergyExplicitly(double Dtime)
        {
            //this.PrintLog("StepEnergyExplicitly");
            return true;
        }

        public bool StepCompositionExplicitly(double Dtime)
        {
            //this.PrintLog("StepCompositionExplicitly");
            return true;
        }

        ///////  about windows method  
        /*public void OnView(object msgvar)
        {}*/

        public void Terminate()
        {
            //Controller.Instance.PrintLog("Terminate");
            this.controller.RemoveData();
        }
    }
}