using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations
{
    public enum SessionPhase
    {
        Started,
        Handshake,
        ReadAndWriteRequest,
        Terminate,
        TerminateAndRestart
    }
}
