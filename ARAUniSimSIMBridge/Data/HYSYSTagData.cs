using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UniSimDesign;

namespace ARAUniSimSIMBridge.Data
{
    public class HYSYSTagData
    {
        public string type { get; set; }
        public string name { get; set; }
        public string sheet { get; set; }
        public string unit { get; set; }

        public double ResistanceValue { get; set; }
        public double PercentOpenValue { get; set; }
        public double OutputVal { get; set; }
        public double SP { get; set; }
        public double PV { get; set; }
        public double OPState { get; set; }

        [Browsable(false)]
        public _IOperation op { get; set; }
    }
}
