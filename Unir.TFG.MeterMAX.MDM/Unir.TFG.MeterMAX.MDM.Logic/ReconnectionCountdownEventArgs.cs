using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Logic
{
    public class ReconnectionCountdownEventArgs : EventArgs
    {
        public TimeSpan RemainingTime { get; set; }
    }
}
