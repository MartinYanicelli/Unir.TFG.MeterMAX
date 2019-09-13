using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class PreviousDemandResetDataTable : PreviousBillingRegisterDataTable
    {
        public override string PreviousEventName
        {
            get { return "Demand Reset"; }
        }

        public PreviousDemandResetDataTable() : base()
        {
 
        }

        public PreviousDemandResetDataTable(DataSelectionTable dataSelectionTable) 
            : base(dataSelectionTable)
        {

        }
    }
}
