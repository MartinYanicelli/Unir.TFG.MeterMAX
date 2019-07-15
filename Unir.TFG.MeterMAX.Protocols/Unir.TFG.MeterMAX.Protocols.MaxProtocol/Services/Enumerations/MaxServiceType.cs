using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services.Enumerations
{
    public enum MaxServiceType : byte
    {
        ShortService,
        ServiceWithData = 0x18,
    }
}
