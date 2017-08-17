using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARAUniSimSIMBridge.Data
{
    /// <summary>
    /// Extension 설정 내용
    /// </summary>
    [Serializable]
    public class Configuration
    {
        /// <summary>
        /// OLGA genkey 경로
        /// </summary>
        public string PathOLGAGenkey { get; set; }
        /// <summary>
        /// OLGA Snapshot 경로
        /// </summary>
        public string PathOLGASnapshot { get; set; }

        /// <summary>
        /// OTS 배속
        /// </summary>
        public double Foctor { get; set; }
        /// <summary>
        /// Data 교환 주기
        /// </summary>
        public int RunInterval { get; set; }

        /// <summary>
        /// Mapping list 경로
        /// </summary>
        public string PathMapping { get; set; }
        /// <summary>
        /// 연결 했던 OPC Server name
        /// </summary>
        public string OPCServerName { get; set; }
    }
}
