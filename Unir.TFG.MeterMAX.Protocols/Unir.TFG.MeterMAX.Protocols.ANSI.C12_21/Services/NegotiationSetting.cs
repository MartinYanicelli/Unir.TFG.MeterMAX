using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class NegotiationSetting
    {
        public BaudRateSelector BaudRateSelector { get; set; }
        public short PacketSize { get; set; }
        public byte NumberOfPackets { get; set; }
        public BaudRateValue BaudRate { get; set; }

    }
}
