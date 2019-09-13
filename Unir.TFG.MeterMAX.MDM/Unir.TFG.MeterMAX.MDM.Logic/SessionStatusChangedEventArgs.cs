using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;

namespace Unir.TFG.MeterMAX.MDM.Logic
{
    public class SessionStatusChangedEventArgs : EventArgs
    {
        public MeterSessionStatus SessionStatus { get; set; }
        public string CurrentSessionTrace { get; set; }
    }
}
