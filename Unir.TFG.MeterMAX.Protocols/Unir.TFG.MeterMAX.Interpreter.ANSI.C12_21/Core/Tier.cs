using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class Tier : BillingDataBlock
    {
        public Tier(int tierNumber) 
            : base(string.Format("Tier{0}", (char) (65 + tierNumber)))
        {

        }
    }
}
