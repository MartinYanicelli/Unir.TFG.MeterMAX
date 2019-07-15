using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services.Enumerations
{
    public enum MaxBaudRate : byte
    {
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
