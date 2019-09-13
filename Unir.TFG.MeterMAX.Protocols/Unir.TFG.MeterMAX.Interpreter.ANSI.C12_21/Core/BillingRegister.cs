using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class BillingRegister
    {
        public decimal Value { get; set; }
        public string UOM { get; set; }
        public string Direction { get; set; }

        public BillingRegister()
        { }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Value, string.IsNullOrEmpty(UOM) ? "n/a" : UOM, string.IsNullOrEmpty(Direction) ? "n/a" : Direction);
        }
    }
}
