using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;

namespace Unir.TFG.MeterMAX.MDM.Logic
{
    public class SessionEndedEventArgs : EventArgs
    {
        public MeterSession MeterSession { get; set; }
        public bool DataSetExecutionSuccess { get; set; }
        public int DataSetExecutionPercent { get; set; }
        public DataSetExecutionQuality DataSetExecutionQuality { get; set; }
        public long TotalBytesSent { get; set; }
        public long TotalBytesReceived { get; set; }
    }
}
