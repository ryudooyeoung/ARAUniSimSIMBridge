using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UniSimDesign;

namespace ARAUniSimSIMBridge.Data
{
    [Serializable]
    public class OTSTagData
    {
        public string Type { get; set; }
        public string TagName { get; set; }
        public string Parameter { get; set; }

        public string Sheet { get; set; }
        public string Unit { get; set; }
        public double value { get; set; }


        [Browsable(false)]
        public _IOperation op { get; set; }

        public string GetFullTagName()
        {
            return string.Format("{0}.{1}", this.TagName, this.Parameter);
        }
    }
}
