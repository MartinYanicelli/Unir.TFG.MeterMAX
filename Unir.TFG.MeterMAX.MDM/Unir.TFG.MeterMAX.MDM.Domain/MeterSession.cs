using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class MeterSession : Noanet.XamArch.Domain.Entity
    {
        public Guid Guid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan ElapsedTime => EndDate - StartDate;
        public string SessionTrace { get; set; }
        public MeterSessionStatus SessionStatus { get; set; }
        public MeterSessionSetting SessionSetting { get; set; }
        public Meter Meter { get; }
        public IDictionary<int, MeterSessionTask> SessionTasks { get; }

        public MeterSession(Meter meter, DataSet dataSet)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));

            Meter = meter ?? throw new ArgumentNullException(nameof(meter));

            SessionTasks = dataSet.DataSetComponents.ToDictionary(k => k.Id, v => new MeterSessionTask() {
                Name = v.Name,
                DataSetComponent = v,
                Items = new Dictionary<object, MeterSessionItemTask>() });
        }
    }
}
