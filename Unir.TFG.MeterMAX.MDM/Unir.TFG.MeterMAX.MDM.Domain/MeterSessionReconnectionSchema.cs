using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class MeterSessionReconnectionSchema : Noanet.XamArch.Domain.Entity
    {
        public int MaxReconnectionAttempts { get; set; }
        public IList<ReconnectionSchedule> ReconnectionSchedules { get; set; }

        public MeterSessionReconnectionSchema()
        {
            ReconnectionSchedules = new List<ReconnectionSchedule>();
        }
    }
}
