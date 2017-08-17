using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UniSimDesign;

namespace ARAUniSimSIMBridge.Data
{
    /// <summary>
    /// OTS Tag data 정보
    /// </summary>
    [Serializable]
    public class OTSTagData
    {
        /// <summary>
        /// fbcontrolop, digitalop...
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// tag name
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// parameter
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// 속해있는 sheet
        /// </summary>
        public string Sheet { get; set; }
        /// <summary>
        /// tag unit
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// tag value
        /// </summary>
        public double value { get; set; }

        /// <summary>
        /// 실제 operation
        /// </summary>
        [Browsable(false)]
        public _IOperation op { get; set; }

        /// <summary>
        /// tagname + parameter
        /// </summary>
        /// <returns></returns>
        public string GetFullTagName()
        {
            return string.Format("{0}.{1}", this.TagName, this.Parameter);
        }
    }
}
