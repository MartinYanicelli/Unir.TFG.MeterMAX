using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class MeterSessionSetting : Noanet.XamArch.Domain.Entity
    {
        public string Name { get; set; }
        public int? InternalReconnectionAttempts { get; set; }
        public MeterProtocolSetting ProtocolSetting { get; set; }
        public MeterSessionReconnectionSchema ReconnectionSchema { get; set; }
        public CommunicationChannelSetting CommunicationChannelSetting { get; set; }
    }
}
