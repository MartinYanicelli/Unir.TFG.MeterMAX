using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class PreviousSeasonDataTable : PreviousBillingRegisterDataTable
    {

        public override string PreviousEventName
        {
            get { return "Season Changed"; }
        }

        public PreviousSeasonDataTable() : base() { }

        public PreviousSeasonDataTable(DataSelectionTable dataSelectionTable) 
            : base(dataSelectionTable) 
        { }
       
    }
}
