using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ARAUniSimSIMBridge.Data
{
    [Serializable]
    public class MappingData : ICloneable
    {
        public string FromType { get; set; }
        public string FromName { get; set; }
        [XmlIgnore]
        [Browsable(false)]
        public double FromValue { get; set; }


        public string ToType { get; set; }
        public string ToName { get; set; }
        [XmlIgnore]
        [Browsable(false)]
        public double ToValue { get; set; }


        public object Clone()
        {
            MappingData result = new MappingData();

            result.FromType = this.FromType;
            result.FromName = this.FromName;

            result.ToType = this.ToType;
            result.ToName = this.ToName;

            result.FromValue = this.FromValue;
            result.ToValue = this.ToValue;


            return result;

        }

        public override string ToString()
        {
            return string.Format("{0}[{1}] - {2}[{3}]", FromType, FromName, ToType, ToName);
        }
    }
}
