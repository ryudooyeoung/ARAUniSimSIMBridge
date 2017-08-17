using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ARAUniSimSIMBridge.Data
{
    /// <summary>
    /// Mapping list 정보
    /// </summary>
    [Serializable]
    public class MappingData : ICloneable
    {
        /// <summary>
        /// 데이터를 보낼 ots, opc 이름
        /// ots -> hysys
        /// opc -> progid를 사용함
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 데이터를 보낼 tag name
        /// </summary>
        public string FromName { get; set; }


        /// <summary>
        /// 데이터를 받을 tag name
        /// </summary>
        public string ToType { get; set; }
        /// <summary>
        /// 데이터를 받을 tag name
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// 아이템 복사
        /// </summary>
        /// <returns>복사된 item</returns>
        public object Clone()
        {
            MappingData result = new MappingData();

            result.FromType = this.FromType;
            result.FromName = this.FromName;

            result.ToType = this.ToType;
            result.ToName = this.ToName;

            return result;
        }

        /// <summary>
        /// 정보 출력
        /// </summary>
        /// <returns>정보</returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}] - {2}[{3}]", FromType, FromName, ToType, ToName);
        }
    }
}
