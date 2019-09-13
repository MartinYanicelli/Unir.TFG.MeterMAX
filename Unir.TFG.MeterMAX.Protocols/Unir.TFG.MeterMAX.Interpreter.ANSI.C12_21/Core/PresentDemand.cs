using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class PresentDemand
    {
        public TimeSpan TimeRemaining { get; set; }
        public BillingRegister Value { get; set; }
    }
}
