using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class Demand
    {
        public DateTime? Date { get; set; }
        public BillingRegister Max { get; set; }
        public BillingRegister Cumulative { get; set; }
    }
}
