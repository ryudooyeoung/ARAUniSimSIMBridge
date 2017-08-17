using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc;
using Opc.Da;


namespace ARAUniSimSIMBridge.Data
{
    /// <summary>
    /// OPC Group 정보
    /// </summary>
    public class OPCSubscription
    {
        /// <summary>
        /// OPC Group name
        /// </summary>
        public string Name { get { return this.Subscription.Name; } }

        /// <summary>
        /// 0 ots , 1 opc, 2 self
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// opc group
        /// </summary>
        public Opc.Da.Subscription Subscription { get; set; }

        /// <summary>
        /// item type list
        /// </summary>
        public System.Type[] ItemTypes { get; set; }

        /// <summary>
        /// 데이터 교환에 쓰일 data list
        /// </summary>
        public Opc.Da.ItemValueResult[] ItemValues { get; set; }

        /// <summary>
        /// 다른 opc와 연결 됐을경우 연결된 opc server index
        /// </summary>
        public int ConnectedServerIndex { get; set; }
        /// <summary>
        /// 연결된 opc에서 교환 할 자료가 들어있는 group index
        /// </summary>
        public int ConnectedSubscriptionIndex { get; set; }
        /// <summary>
        /// 다른 opc와 연결 됐을경우 연결된 opc server name
        /// </summary>
        public string ConnectedServerName { get; set; }
        /// <summary>
        /// 연결된 opc에서 교환 할 자료가 들어있는 group name
        /// </summary>
        public string ConnectedSubscriptionName { get; set; }


        /// <summary>
        /// 교환 할 자료가 들어있는 ots data table index
        /// </summary>
        public int ConnectedDataTableIndex { get; set; }
        /// <summary>
        /// 교환 할 자료가 들어있는 ots data table name
        /// </summary>
        public string ConnectedDataTableName { get; set; }


        /// <summary>
        /// 생성자
        /// </summary>
        public OPCSubscription()
        {
            this.ItemTypes = new System.Type[0];
            this.ItemValues = new Opc.Da.ItemValueResult[0];

            this.Type = 1;
            this.ConnectedDataTableIndex = -1;
            this.ConnectedServerIndex = -1;
            this.ConnectedSubscriptionIndex = -1;
        }

        /// <summary>
        /// 요약 정보
        /// </summary>
        /// <returns>정보</returns>
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
