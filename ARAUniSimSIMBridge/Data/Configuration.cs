using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARAUniSimSIMBridge.Data
{
    [Serializable]
    public class Configuration
    {
        public string PathOLGAGenkey { get; set; }
        public string PathOLGASnapshot { get; set; }

        public double Foctor { get; set; }
        public int RunInterval { get; set; }
        public string PathMapping { get; set; }

    }
}
