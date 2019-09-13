using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class DimensionLoadProfileTable : TableParser
    {
        public int LpMemoryLen { get; protected set; }
        public short LpFlags { get; protected set; }
        public int LpFormats { get; protected set; }
        public int NumberOfDataBlocks { get; protected set; }
        public int NumberOfIntervalsPerBlock { get; protected set; }
        public int NumberOfChannels { get; protected set; }
        public int IntervalTime { get; protected set; }

        public DimensionLoadProfileTable()
        {

        }

        protected override void OnParse(byte[] table)
        {
            int offset = 0;

            byte[] memoryLenArray = new byte[4];
            Array.Copy(table, offset, memoryLenArray, 0, 4);
            Array.Reverse(memoryLenArray);
            LpMemoryLen = Helper.GetInt(memoryLenArray);
            offset = offset + 4;

            byte[] flagsArray = new byte[2];
            Array.Copy(table, offset, flagsArray, 0, 2);
            Array.Reverse(flagsArray);
            LpFlags = Helper.GetShort(flagsArray);
            offset = offset + 2;

            LpFormats = table[offset];
            offset++;

            byte[] blockArray = new byte[2];
            Array.Copy(table, offset, blockArray, 0, 2);
            Array.Reverse(blockArray);
            NumberOfDataBlocks = Helper.GetShort(blockArray);
            offset = offset + 2;
         
            byte[] intervalsPerBlockArray = new byte[2];
            Array.Copy(table, offset, intervalsPerBlockArray, 0, 2);
            Array.Reverse(intervalsPerBlockArray);
            NumberOfIntervalsPerBlock = Helper.GetShort(intervalsPerBlockArray);
            offset = offset + 2;

            NumberOfChannels = table[offset];
            offset++;

            IntervalTime = table[offset];
        }

        public override string ToString()
        {
            return string.Format("LP Memory LEN: {0} - LP Flags: {1} - LP Formats: {2} - Number of DataBlocks: {3} - Number of Intervals p/Blocks: {4} - Number of Channels. {5} - IntervalTime: {6}", 
                LpMemoryLen, LpFlags, LpFormats, NumberOfDataBlocks, NumberOfIntervalsPerBlock, NumberOfChannels, IntervalTime);
        }
    }
}
