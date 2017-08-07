using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniSimDesign;

namespace ARAUniSimSIMBridge.Data
{
    public class OTSDataTable
    {
        public DataTable DataTable { get; set; }
        public double[] TagValues { get; set; }
        public string[] TagNames { get; set; }
        public string[] TagUnits { get; set; }


        public string Name { get { return this.DataTable.name; } }

        //0 ots , 1 opc , 2 self
        public int Type { get; set; }

        //opc server index, group index.
        public int ConnectedServerIndex { get; set; }
        public int ConnectedSubscriptionIndex { get; set; }

        public string ConnectedServerName { get; set; }
        public string ConnectedSubscriptionName { get; set; }


        public PrivateController Controller { get; set; }
        public OTSDataTable(PrivateController Controller)
        {
            this.Controller = Controller;
            this.Type = 1;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Controller.OPCServerName, Name, TagNames.Length);
        }
    }
}
