using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum ReadingType : byte
    {
        FullRead = 0x30,
        PartialReadWithIndex = 0x31,
        PartialReadWithOffset = 0x3F,
        PartialReadDefault = 0x3E
    }
}
