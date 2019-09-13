using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class OnDemandMeterSession : Noanet.XamArch.Domain.Entity
    {
        public Guid Guid { get; set; }
        public DataSet DataSet { get; set; }
        public Meter Meter { get; set; }
        public MeterSessionTypeCode SessionType { get; set; }
        public MeterSession MeterSession { get; set; }
        public UserSession UserSession { get; set; }
    }
}
