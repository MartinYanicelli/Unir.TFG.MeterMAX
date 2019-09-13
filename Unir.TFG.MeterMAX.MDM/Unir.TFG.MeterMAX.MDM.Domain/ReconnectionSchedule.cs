using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class ReconnectionSchedule : Noanet.XamArch.Domain.Entity
    {
        public TimeSpan Schedule { get; set; }
        public MeterSessionReconnectionSchema ReconnectionSchema { get; set; }
    }
}
