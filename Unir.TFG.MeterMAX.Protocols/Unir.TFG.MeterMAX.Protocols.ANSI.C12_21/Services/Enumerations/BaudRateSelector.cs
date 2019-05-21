using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum  BaudRateSelector : byte
    {
        NoneBaudRateRequested = 0x60,
        OneBaudRateRequested,
        TwoBuadRateRequested,
        ThreeBuadRateRequested,
        FourBuadRateRequested,
        FiveBuadRateRequested,
        SixBuadRateRequested,
        SevenBuadRateRequested,
        EightBuadRateRequested,
        NineBuadRateRequested,
        TenBuadRateRequested,
        ElevenBuadRateRequested
    }
}
