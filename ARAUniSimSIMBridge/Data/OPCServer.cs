using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc;
using Opc.Da;

namespace ARAUniSimSIMBridge.Data
{
    /// <summary>
    /// OPC Server 정보
    /// </summary>
    public class OPCServer
    {
        /// <summary>
        /// 0 local , 1 network
        /// </summary>
        public int ServerType { get; set; }

        /// <summary>
        /// opc server 이름
        /// </summary>
        public string Name { get { return Server.Name; } }


        /// <summary>
        /// opc server
        /// </summary>
        public Opc.Da.Server Server { get; set; }

        /// <summary>
        /// commonad 그룹
        /// </summary>
        public OPCSubscription CommandSubscription { get; set; }
        /// <summary>
        /// read와 연관된 group
        /// </summary>
        public List<OPCSubscription> ReadSubscriptions { get; set; }
        /// <summary>
        /// write와 연관된 group
        /// </summary>
        public List<OPCSubscription> WriteSubscriptions { get; set; }

        /// <summary>
        /// opc server에 존재하는 item list
        /// </summary>
        public List<Opc.Da.BrowseElement> OPCItemlist { get; set; }


        /// <summary>
        /// 해당 extension
        /// </summary>
        public PrivateController Controller { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="Controller">해당 extension controller</param>
        public OPCServer(PrivateController Controller)
        {
            this.Controller = Controller;
            this.ReadSubscriptions = new List<OPCSubscription>();
            this.WriteSubscriptions = new List<OPCSubscription>();

            this.OPCItemlist = new List<BrowseElement>();
        }

        /// <summary>
        /// 정보 표현
        /// </summary>
        /// <returns>정보</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",
                Server.Name, Server.IsConnected, Server.Url, Server.Subscriptions.Count);
        }

        /// <summary>
        /// 자기 자신에게 쓰이는 데이터 교환
        /// </summary>
        public void SelfDataExchange()
        {
            OPCSubscription selfRead = null;
            for (int subi = 0; subi < this.ReadSubscriptions.Count; subi++)
            {
                if (this.ReadSubscriptions[subi].Type == 2)
                {
                    selfRead = this.ReadSubscriptions[subi];
                    break;
                }
            }

            OPCSubscription selfWrite = null;
            for (int subi = 0; subi < this.WriteSubscriptions.Count; subi++)
            {
                if (this.WriteSubscriptions[subi].Type == 2)
                {
                    selfWrite = this.WriteSubscriptions[subi];
                    break;
                }
            }

            //올바른 데이터 그룹이라면.
            if (selfRead != null && selfWrite != null)
            {
                //읽기
                selfRead.ItemValues = selfRead.Subscription.Read(selfRead.Subscription.Items);

                List<Opc.Da.ItemValue> values = new List<ItemValue>();

                for (int i = 0; i < selfRead.ItemValues.Length; i++)
                {
                    Opc.Da.Item item = selfWrite.Subscription.Items[i];
                    Opc.Da.ItemValueResult ivr = selfRead.ItemValues[i];

                    System.Type wt = selfWrite.ItemTypes[i];
                    System.Type rt = selfRead.ItemTypes[i];

                    object value = ivr.Value;
                    if (wt != rt)
                    {
                        value = Activator.CreateInstance(wt);
                    }

                    values.Add(new ItemValue(item) { Value = value });
                }
                selfWrite.ItemValues = selfRead.ItemValues;
                selfWrite.Subscription.Write(values.ToArray());
            }
        }


        /// <summary>
        /// opc server
        /// </summary>
        /// <param name="server">해당 서버</param>
        public void SetServer(Opc.Da.Server server)
        {
            this.Server = server;
        }

        /// <summary>
        /// server 연결
        /// </summary>
        /// <returns>true 성공, false 실패</returns>
        public bool Connect()
        {
            if (this.Server == null)
            {
                //Controller.Instance.PrintLog(" server nullllll");
                return false;
            }

            try
            {
                this.Server.Connect();

                //olga 용 command group 설정
                if (this.Server.Name.StartsWith("SPT."))
                {
                    if (this.SetOLGACmd() == false)
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                CommonController.Instance.PrintLog(this.Server.Url.ToString());
                CommonController.Instance.PrintLog(ex.StackTrace);
                return false;
            }

            return true;
        }
 


        /// <summary>
        /// opc 그룹 모두 지우기
        /// </summary>
        /// <param name="reset">command 그룹 삭제 여부</param>
        public void RemoveAllGroup(bool reset)
        {
            try
            {
                for (int j = this.Server.Subscriptions.Count - 1; j >= 0; j--)
                {
                    Opc.Da.Subscription sub = this.Server.Subscriptions[j];

                    if (reset == false)
                        if (sub.Name == "cmd") continue;


                    this.Server.CancelSubscription(sub);
                }

                if (reset)
                {
                    this.CommandSubscription = null;
                }

                this.WriteSubscriptions.Clear();
                this.ReadSubscriptions.Clear();

            }
            catch (Exception ex)
            {
                CommonController.Instance.PrintLog("RemoveAllGroup , " + ex.GetType().Name);
                CommonController.Instance.PrintLog(ex.StackTrace);
            }

        }

        /// <summary>
        /// 그룹추가.
        /// </summary>
        /// <param name="name">group name</param>
        /// <param name="type">read 0, write 1</param>
        /// <returns>추가 성공한 group</returns>
        public OPCSubscription AddGroup(string name, int type)
        {
            bool duplicated = false;
            OPCSubscription result = null;

            if (type == 0)
            {
                for (int i = 0; i < this.ReadSubscriptions.Count; i++)
                {
                    if (this.ReadSubscriptions[i].Name.Equals(name))
                    {
                        duplicated = true;
                        result = this.ReadSubscriptions[i];
                        break;
                    }
                }
            }
            else if (type == 1)
            {
                for (int i = 0; i < this.WriteSubscriptions.Count; i++)
                {
                    if (this.WriteSubscriptions[i].Name.Equals(name))
                    {
                        duplicated = true;
                        result = this.WriteSubscriptions[i];
                        break;
                    }
                }
            }

            //중복이 아니라면
            if (duplicated == false)
            {
                SubscriptionState subscriptionstate = new SubscriptionState()
                {
                    Active = true,
                    Deadband = 0,
                    KeepAlive = 0,
                    UpdateRate = 1000,
                    ClientHandle = Guid.NewGuid().ToString(),
                    Name = name
                };

                //현재 선택된 서버에 Group을 생성한다.
                Opc.Da.Subscription subscription = (Opc.Da.Subscription)Server.CreateSubscription(subscriptionstate);


                OPCSubscription newgroup = new OPCSubscription() { Subscription = subscription };

                if (type == 0) this.ReadSubscriptions.Add(newgroup);
                else if (type == 1) this.WriteSubscriptions.Add(newgroup);

                result = newgroup;
            }
            return result;
        }


        /// <summary>
        /// olga opc 서버용 command group 추가.
        /// </summary>
        /// <returns></returns>
        private bool SetOLGACmd()
        {
            bool result = true;

            SubscriptionState subscriptionstate = new SubscriptionState()
            {
                Active = true,
                Deadband = 0,
                KeepAlive = 0,
                UpdateRate = 1000,
                ClientHandle = Guid.NewGuid().ToString(),
                Name = "cmd"
            };
            //현재 선택된 서버에 Group을 생성한다.
            Opc.Da.Subscription cmd = (Opc.Da.Subscription)this.Server.CreateSubscription(subscriptionstate);
            this.CommandSubscription = new OPCSubscription() { Subscription = cmd };



            List<Opc.Da.Item> cmdlist = new List<Opc.Da.Item>();
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.ExternalClock", this.Controller.OLGAModelName))); //0
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.TIME", this.Controller.OLGAModelName))); //1
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.State", this.Controller.OLGAModelName))); //2
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.LoadSnap", this.Controller.OLGAModelName)));//3
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.LoadSnap.File", this.Controller.OLGAModelName)));//4
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.SaveSnap", this.Controller.OLGAModelName)));//5
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.SaveSnap.File", this.Controller.OLGAModelName)));//6
            cmdlist.Add(OPCServer.CreateOPCItem(string.Format("Sim.{0}.LastMessage", this.Controller.OLGAModelName)));//7

            ItemResult[] ir = cmd.AddItems(cmdlist.ToArray());

            StringBuilder strb = new StringBuilder();
            for (int i = 0; i < ir.Length; i++)
            {
                if (ir[i].ResultID.Failed())
                {
                    result = false;
                    strb.AppendFormat("{0}\n", ir[i].ItemName);
                }
            }
            if (result == false)
            {
                strb.Insert(0, "Check OLGA OPC Server configuration, can't find the tag\n");
                CommonController.Instance.PrintLog(strb.ToString());
            }

            return result;
        }

        /// <summary>
        /// OPC Item 생성
        /// </summary>
        /// <param name="name">Item name</param>
        /// <returns>Item</returns>
        public static Opc.Da.Item CreateOPCItem(string name)
        {
            Opc.Da.Item item = new Opc.Da.Item()
            {
                ItemName = name,
                Active = true,
                ActiveSpecified = true,
                ItemPath = "",
                ClientHandle = Guid.NewGuid().ToString()
            };
            return item;
        }

    }
}
