using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class DimensionRegisterTable : TableParser
    {
        public int NumberOfSelfReads { get; protected set; }
        public int NumberOfSummations { get; protected set; }
        public int NumberOfDemmands { get; protected set; }
        public int NumberOfCoincidentValues { get; protected set; }
        public int NumberOfMaxDemandOccurrence { get; protected set; }
        public int NumberOfTiers { get; protected set; }
        public int NumberOfPresentDemands { get; protected set; }
        public int NumberOfPresentValues { get; protected set; }

        public DimensionRegisterTable()
        {
           
        }

        protected override void OnParse(byte[] table)
        {
            NumberOfSelfReads = (int)table[2];
            NumberOfSummations = (int)table[3];
            NumberOfDemmands = (int)table[4];
            NumberOfCoincidentValues = (int)table[5];
            NumberOfMaxDemandOccurrence = (int)table[6];
            NumberOfTiers = (int)table[7];
            NumberOfPresentDemands = (int)table[8];
            NumberOfPresentValues = (int)table[9];
        }

        public override string ToString()
        {
            return string.Format("Number of Selft Reads: {0} - Number of Summations: {1} - Number of Demmands: {2} - Number of Coincident Values: {3} - Number of Max Demand Occurrence: {4} - Number of Tiers: {5} - Number of Present Demands: {6} - Number of Present Values: {7}",
                NumberOfSelfReads, NumberOfSummations, NumberOfDemmands, NumberOfCoincidentValues, NumberOfMaxDemandOccurrence, NumberOfTiers, NumberOfPresentDemands, NumberOfPresentValues);
        }

    }
}
