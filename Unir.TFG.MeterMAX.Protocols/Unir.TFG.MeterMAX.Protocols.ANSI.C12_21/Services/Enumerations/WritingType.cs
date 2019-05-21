using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum WritingType
    {
        FullWrite = 0x40,
        PartialWriteWithIndex = 0x41,
        PartialWriteWithOffset = 0x4F
    }
}
