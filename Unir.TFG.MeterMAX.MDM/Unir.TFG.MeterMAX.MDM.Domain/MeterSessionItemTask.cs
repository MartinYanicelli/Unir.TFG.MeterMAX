using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class MeterSessionItemTask : Noanet.XamArch.Domain.Entity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Error { get; set; }
        public bool Success { get; set; }
        public object TaskResult { get; set; }
        public MeterSessionTask SessionTask { get; set; }
    }
}
