using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniSimDesign;

namespace ARAUniSimSIMBridge.Data
{
    /// <summary>
    /// OTS Datatable 정보
    /// </summary>
    public class OTSDataTable
    {
        /// <summary>
        /// ots data table
        /// </summary>
        public DataTable DataTable { get; set; }
        /// <summary>
        /// data value list
        /// </summary>
        public double[] TagValues { get; set; }
        /// <summary>
        /// tag name list
        /// </summary>
        public string[] TagNames { get; set; }
        /// <summary>
        /// tag unit list
        /// </summary>
        public string[] TagUnits { get; set; }

        /// <summary>
        /// data table name
        /// </summary>
        public string Name { get { return this.DataTable.name; } }

        /// <summary>
        /// 0 ots , 1 opc , 2 self\
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 데이터 교환 할 opc server index
        /// </summary>
        public int ConnectedServerIndex { get; set; }
        /// <summary>
        /// 데이터 교환 할 opc group index
        /// </summary>
        public int ConnectedSubscriptionIndex { get; set; }

        /// <summary>
        /// 데이터 교환 할 opc server name
        /// </summary>
        public string ConnectedServerName { get; set; }
        /// <summary>
        /// 데이터 교환 할 opc group name
        /// </summary>
        public string ConnectedSubscriptionName { get; set; }

        /// <summary>
        /// 해당 extension
        /// </summary>
        public PrivateController Controller { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="Controller">해당 extension</param>
        public OTSDataTable(PrivateController Controller)
        {
            this.Controller = Controller;
            this.Type = 1;
        }

        /// <summary>
        /// 정보출력
        /// </summary>
        /// <returns>정보출력</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Controller.OPCServerName, Name, TagNames.Length);
        }
    }
}
