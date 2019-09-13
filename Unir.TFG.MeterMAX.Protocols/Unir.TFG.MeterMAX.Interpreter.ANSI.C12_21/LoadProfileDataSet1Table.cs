using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class LoadProfileDataSet1Table: TableParser
    {
        public ActualLoadProfileTable ActualLoadProfileTable { get; set; }
        public IList<LpData> LpData { get; private set; }

        public LoadProfileDataSet1Table()
        {
            LpData = new List<LpData>();
        }
        
        public LoadProfileDataSet1Table(ActualLoadProfileTable actualLoadProfileTable)
        {
            ActualLoadProfileTable = actualLoadProfileTable;
        }

        protected override void OnParse(byte[] table)
        {
            if (ActualLoadProfileTable == null)
                throw new InvalidOperationException("La Propiedad ActualLoadProfileTable no puede ser nula.");

            int offset = 0;
            for (int i = 0; i < ActualLoadProfileTable.NumberOfDataBlocks; i++)
            {
                try
                {
                    LpData data = new LpData
                    {
                        DateTime = Helper.ConvertToDateTime(new byte[] { table[offset], table[++offset], table[++offset], table[++offset], table[++offset], 0 }).Value
                    };
                    offset++;
                    offset = offset + ((ActualLoadProfileTable.NumberOfIntervalsPerBlock + 7) / 8);//Simple Interval Status

                    /*string cad = "";
                    for (int j = 0; j < Nbr_Blk_Ints_Set1 * (1 + Nbr_Chns_Set1 * 5 / 2); j++)
                    {
                        byte[] b = new byte[1];
                        b[0] = table[pos];
                        pos++;
                        cad = cad + HexEncoding.ToString(b);
                    }
                    dat.datos_bloque = cad;*/
                    for (int j = 0; j < ActualLoadProfileTable.NumberOfIntervalsPerBlock; j++)
                    {
                        offset = offset + 1 + (ActualLoadProfileTable.NumberOfChannels / 2); //Extended Interval Status

                        //Interval Data
                        IList<short> lpIntervalData = new List<short>();
                        for (int k = 0; k < ActualLoadProfileTable.NumberOfChannels; k++)
                        {
                            byte[] valueArray = new byte[2];
                            Array.Copy(table, offset, valueArray, 0, 2);
                            Array.Reverse(valueArray);
                            short value = Helper.GetShort(valueArray);
                            offset = offset + 2;

                            lpIntervalData.Add(value);

                        }
                        data.Blocks.Add(lpIntervalData);
                    }

                    LpData.Add(data);
                }
                catch (Exception)
                {
                    // comsumimos el error por intervalo incompleto...
                }
            }
        }
        
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            for (int index = 0; index < LpData.Count; index++)
            {
                LpData data = (LpData)LpData[index];
                result.AppendFormat("IntervalDate: {0}; ", data.DateTime);
                result.AppendLine();
                for (int j = 0; j < data.Blocks.Count; j++)
                {
                    IList<short> lpBlock = data.Blocks[j];
                    for (int k = 0; k < lpBlock.Count; k++)
                    {
                        result.AppendFormat("Block{0} Channel {1} Value: {2}; \t", j, k, lpBlock[k]);
                    }
                }
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}
