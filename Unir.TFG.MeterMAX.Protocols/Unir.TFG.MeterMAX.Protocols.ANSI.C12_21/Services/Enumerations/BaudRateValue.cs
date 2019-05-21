using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum BaudRateValue : byte
    {
        ExternallyDefined = 0x00,
        BaudRate_300 = 0x01,
        BaudRate_600,
        BaudRate_1200,
        BaudRate_2400,
        BaudRate_4800,
        BaudRate_9600,
        BaudRate_14400,
        BaudRate_19200,
        BaudRate_28800,
        BaudRate_57600,
        BaudRate_38400,
        BaudRate_115200,
        BaudRate_128000,
        BaudRate_256000
    }
}
