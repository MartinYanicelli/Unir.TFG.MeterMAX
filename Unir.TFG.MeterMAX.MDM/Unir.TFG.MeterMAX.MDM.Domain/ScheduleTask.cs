using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class ScheduleTask : Noanet.XamArch.Domain.Entity
    {
        public Guid Name { get; set; }
        public string Description { get; set; }
        public ScheduleTaskStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? NextExecution { get; set; }
        public DataSet DataSet { get; set; }
        public IList<MeterSession> MeterSessions { get; set; }
    }
}
