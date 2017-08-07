using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc;
using Opc.Da;

namespace ARAUniSimSIMBridge.Data
{
    public class OPCServer
    {
        //0 local , 1 network
        public int ServerType { get; set; }

        public string Name { get { return Server.Name; } }


        public Opc.Da.Server Server { get; set; }

        public OPCSubscription CommandSubscription { get; set; }
        public List<OPCSubscription> ReadSubscriptions { get; set; }
        public List<OPCSubscription> WriteSubscriptions { get; set; }

        public List<Opc.Da.BrowseElement> OPCItemlist { get; set; }


        public PrivateController Controller { get; set; }
        public OPCServer(PrivateController Controller)
        {
            this.Controller = Controller;
            this.ReadSubscriptions = new List<OPCSubscription>();
            this.WriteSubscriptions = new List<OPCSubscription>();

            this.OPCItemlist = new List<BrowseElement>();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",
                Server.Name, Server.IsConnected, Server.Url, Server.Subscriptions.Count);
        }

        public void SelfDataExchange()
        {

            try
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


                if (selfRead != null && selfWrite != null)
                {
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
            catch (Exception ex)
            {
                CommonController.Instance.PrintLog("dddddddddddddddd" + ex.StackTrace);
            }
        }



        public void PrintGroup()
        {
            for (int i = 0; i < this.Server.Subscriptions.Count; i++)
            {
                //Controller.Instance.PrintLog(this.Server.Subscriptions[i].Name);
            }
        }



        public void SetServer(Opc.Da.Server server)
        {
            this.Server = server;
        }

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
                Opc.Da.Subscription cg = (Opc.Da.Subscription)this.Server.CreateSubscription(subscriptionstate);
                this.CommandSubscription = new OPCSubscription() { Subscription = cg };

                if (this.Server.Name.StartsWith("SPT."))
                {
                    if (this.SetOLGACmd(ref cg) == false)
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
                CommonController.Instance.PrintLog(ex.StackTrace);
            }

        }

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


        private bool SetOLGACmd(ref Opc.Da.Subscription cmd)
        {
            bool result = true;
            try
            {
                cmd.RemoveItems(cmd.Items);
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
            }
            catch (Exception ex)
            {
                CommonController.Instance.PrintLog(ex.StackTrace);
                result = false;
            }

            return result;
        }

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
