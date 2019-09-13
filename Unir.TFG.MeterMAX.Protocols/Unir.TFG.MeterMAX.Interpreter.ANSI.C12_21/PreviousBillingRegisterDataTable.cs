using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public abstract class PreviousBillingRegisterDataTable : BillingRegisterDataTable
    {
        public DateTime? TimeWhenPreviousEventOccurred { get; protected set; }
        public abstract string PreviousEventName { get; }

        public PreviousBillingRegisterDataTable() : base()
        {
 
        }

        public PreviousBillingRegisterDataTable(DataSelectionTable dataSelectionTable) 
            : base(dataSelectionTable)
        {

        }

        protected override void OnParse(byte[] table)
        {
            int initial = 0; //Empieza del segundo byte

            byte[] timeArray = new byte[5];
            Array.Copy(table, timeArray, 5);
            TimeWhenPreviousEventOccurred = Helper.ConvertToDateTime(timeArray);

            initial = initial + 5;
            initial = initial + 1;//Season
            initial = initial + 1;//Number of Demand Resets

            OnParse(table, initial);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("DateTime When {0} Occurred: {1}", PreviousEventName, TimeWhenPreviousEventOccurred);
            result.AppendLine();
            result.Append(base.ToString());
            return result.ToString();
        }


    }
}
