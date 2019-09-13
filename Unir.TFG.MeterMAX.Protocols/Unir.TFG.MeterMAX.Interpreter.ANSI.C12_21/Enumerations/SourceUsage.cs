using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations
{
    [Flags]
    public enum SourceUsage
    {
        BillingSummation,
        BillingDemand,
        Profile,
        PowerQuality,
        WaveForm,
        NotUsedByTheMeter
    }
}
