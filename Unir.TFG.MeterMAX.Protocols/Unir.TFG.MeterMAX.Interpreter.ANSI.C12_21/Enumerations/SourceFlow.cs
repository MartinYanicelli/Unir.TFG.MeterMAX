using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations
{
    [Flags]
    public enum SourceFlow
    {
        Delivered,
        Q1,
        Q2,
        Q3,
        Q4
    }
}
