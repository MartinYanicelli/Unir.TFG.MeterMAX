using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum RemoteServiceType : byte
    {
        ShortService,
        ServiceWithData = 0x18,
    }
}
