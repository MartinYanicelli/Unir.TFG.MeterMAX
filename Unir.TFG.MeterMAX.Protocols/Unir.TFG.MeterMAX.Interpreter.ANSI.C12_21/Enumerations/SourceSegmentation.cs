using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations
{
    public enum SourceSegmentation
    {
        TotalOrNotPhaseRelated = 0,
        PhaseA,
        PhaseC,
        NotValidForInstrumentationRequests,
        Neutral,
        A_Neutral,
        B_Neutral,
        C_Neutral
    }
}
