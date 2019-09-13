using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class InsituMeterSessionSetting : MeterSessionSetting
    {
        public string PortName { get; set; }
        public bool? DtrEnabled { get; set; }

    }
}
