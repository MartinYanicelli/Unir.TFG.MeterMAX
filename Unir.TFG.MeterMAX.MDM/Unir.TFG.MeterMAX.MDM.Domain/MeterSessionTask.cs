using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class MeterSessionTask : Noanet.XamArch.Domain.Entity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Error { get; set; }
        public bool Completed { get; set; }
        public DataSetComponent DataSetComponent { get; set; }
        public IDictionary<object, MeterSessionItemTask> Items { get; set; }
    }
}
