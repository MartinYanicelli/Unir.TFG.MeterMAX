using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class CommunicationChannelSetting : Noanet.XamArch.Domain.Entity
    {
        public int? SendAckResponseThershold { get; set; }
        public BaudRate BaudRate { get; set; }
        public PacketSize PacketSize { get; set; }
        public byte NumberOfPackets { get; set; }

        public byte ChannelTrafficTimeout { get; set; }
        public byte InterCharacterTimeout { get; set; }
        public byte ResponseTimeout { get; set; }
        public byte NumberOfRetries { get; set; }
    }
}
