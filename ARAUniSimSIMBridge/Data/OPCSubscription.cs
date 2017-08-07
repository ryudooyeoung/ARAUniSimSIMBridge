using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc;
using Opc.Da;


namespace ARAUniSimSIMBridge.Data
{
    public class OPCSubscription
    {
        public string Name { get { return this.Subscription.Name; } }

        //0 ots , 1 opc, 2 self
        public int Type { get; set; }
        public Opc.Da.Subscription Subscription { get; set; }
        public System.Type[] ItemTypes { get; set; }
        public Opc.Da.ItemValueResult[] ItemValues { get; set; }


        //opc server index, group index.
        public int ConnectedServerIndex { get; set; }
        public int ConnectedSubscriptionIndex { get; set; }
        public string ConnectedServerName { get; set; }
        public string ConnectedSubscriptionName { get; set; }


        //if connected ots, save the index
        public int ConnectedDataTableIndex { get; set; }
        public string ConnectedDataTableName { get; set; }


        public OPCSubscription()
        {
            this.ItemTypes = new System.Type[0];
            this.ItemValues = new Opc.Da.ItemValueResult[0];

            this.Type = 1;
            this.ConnectedDataTableIndex = -1;
            this.ConnectedServerIndex = -1;
            this.ConnectedSubscriptionIndex = -1;
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (this.Type == 0)
            {
                result = string.Format("ots {0} {1} {2}", Name, ConnectedDataTableIndex, ConnectedDataTableName);
            }
            else if (this.Type == 1)
            {
                result = string.Format("opc {0} {1} {2} {3} {4}", Name, ConnectedServerIndex, ConnectedServerName, ConnectedSubscriptionIndex, ConnectedSubscriptionName);
            }
            else if (this.Type == 2)
            {
                result = string.Format("self {0}", Name);
            }

            return result;
        }
    }
}
