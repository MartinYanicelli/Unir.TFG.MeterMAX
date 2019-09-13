using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class PresentRegisterDataTable : TableParser
    {
        ActualRegisterTable ActualRegisterTable { get; set; }
        
        public IList<PresentDemand> Demands { get; private set; }
        public IList<BillingRegister> Values { get; private set; }

        public PresentRegisterDataTable()
        {
            Demands = new List<PresentDemand>();
            Values = new List<BillingRegister>();
        }

        public PresentRegisterDataTable(ActualRegisterTable actualRegisterTable)
            : this()
        {
            ActualRegisterTable = actualRegisterTable;
        }

        protected override void OnParse(byte[] table)
        {
            if (ActualRegisterTable == null)
                throw new InvalidOperationException("La propiedad ActualRegisterTable no puede ser nula.");
            
            /*if (BitConverter.IsLittleEndian)
                Array.Reverse(cant_arr);
            short cant = BitConverter.(cant_arr, 0);*/

            int offset = 0;
            for (int i = 0; i < ActualRegisterTable.NumberOfPresentDemands; i++)
            {
                int hours = (int)table[offset];
                offset++;
                int minutes = (int)table[offset];
                offset++;
                int seconds = (int)table[offset];
                offset++;
                
                byte[] demandArray = new byte[5];
                Array.Copy(table, offset, demandArray, 0, 5);
                Array.Reverse(demandArray);
                PresentDemand demand = new PresentDemand
                {
                    TimeRemaining = new TimeSpan(hours, minutes, seconds),
                    Value = new BillingRegister() { Value = Helper.GetLong(demandArray) }
                };
                Demands.Add(demand);
                offset = offset + 5;
            }

            for (int i = 0; i < ActualRegisterTable.NumberOfPresentValues; i++)
            {
                byte[] valueArray = new byte[6];
                Array.Copy(table, offset, valueArray, 0, 6);
                Array.Reverse(valueArray);
                long value = Helper.GetLong(valueArray);
                Values.Add(new BillingRegister() { Value = value });
                offset = offset + 6;
            }
        }
        
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            for (int index = 0; index < Demands.Count; index++)
            {
                result.AppendFormat("Demand{0} Time Remaining: {1} Value: {2} ", index, Demands[index].TimeRemaining, Demands[index].Value);
                result.AppendLine();
            }
            result.AppendLine();
            for (int index = 0; index < Values.Count; index++)
            {
                result.AppendFormat("Value{0}: {1}", index, Values[index]);
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}
