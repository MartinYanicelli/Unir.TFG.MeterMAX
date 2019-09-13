using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;

namespace Unir.TFG.MeterMAX.MDM.Logic
{
    public class SessionProgressEventArgs : EventArgs
    {
        public MeterSessionTask SessionTask { get; set; }
        public int PercentAdvance { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}
