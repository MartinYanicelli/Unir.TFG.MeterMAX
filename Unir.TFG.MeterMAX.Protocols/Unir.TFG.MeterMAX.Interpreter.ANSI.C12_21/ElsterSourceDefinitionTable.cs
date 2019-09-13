using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class ElsterSourceDefinitionTable : TableParser
    {
        public readonly IList<Source> Sources;
        
        private const int EntryLen = 7;

        public ElsterSourceDefinitionTable()
        {
            Sources = new List<Source>();
        }

        protected override void OnParse(byte[] table)
        {
            int initial = 0;
            int offset = 0;
            for (int i = 0; i < (table.Length / EntryLen); i++)
            {
                Source source = new Source();
                offset = initial;
                // Get UOM Code
                source.UOMCode = (UOMCode) table[offset];
                // Get Flow 
                offset++;
                byte flow = table[offset];
                if (Helper.IsBitSet(flow, 0))
                {
                    source.Flow |= SourceFlow.Q1;
                }
                if (Helper.IsBitSet(flow, 1))
                {
                    source.Flow |= SourceFlow.Q2;
                }
                if (Helper.IsBitSet(flow, 2))
                {
                    source.Flow |= SourceFlow.Q3;
                }
                if (Helper.IsBitSet(flow, 3))
                {
                    source.Flow |= SourceFlow.Q4;
                }
                if (Helper.IsBitSet(flow, 4))
                {
                    source.Flow |= SourceFlow.Delivered;
                }
                
                //Get Segmentation
                JIBitArray segmentationJibArray = new JIBitArray(new byte[] { flow }).SubJIBitArray(0,3);
                source.Segmentation = (SourceSegmentation)segmentationJibArray.GetShorts()[0];
                // Get Usage
                offset++;
                byte usage = table[offset];
                if (Helper.IsBitSet(usage, 0))
                {
                    source.Usage |= SourceUsage.BillingSummation;
                }
                if (Helper.IsBitSet(usage, 1))
                {
                    source.Usage |= SourceUsage.BillingDemand;
                }
                if (Helper.IsBitSet(usage, 2))
                {
                    source.Usage |= SourceUsage.Profile;
                }
                if (Helper.IsBitSet(usage, 3))
                {
                    source.Usage |= SourceUsage.PowerQuality;
                }
                if (Helper.IsBitSet(usage, 4))
                {
                    source.Usage |= SourceUsage.WaveForm;
                }
                // Get Harmonic
                offset++;
                JIBitArray harmonicJibArray = new JIBitArray(new byte[] { table[offset] }).SubJIBitArray(2, 6);
                source.Harmonic = (SourceHarmonic) harmonicJibArray.GetShorts()[0];
                // Get Scale Factor
                offset++;
                JIBitArray scaleFactorJibArray = new JIBitArray(new byte[] { table[offset] }).SubJIBitArray(3, 5);
                short scaleFactor = 0;
                if (scaleFactorJibArray.Get(0) == true)
                {
                    scaleFactorJibArray = scaleFactorJibArray.Not();
                    scaleFactor = scaleFactorJibArray.GetShorts()[0];
                    scaleFactor = (short)(scaleFactor + 1);
                    scaleFactor = (short)(-1 * scaleFactor);
                }
                source.ScaleFactor = scaleFactor;
                offset += 2;
                source.MultiplierSelect = (int)table[offset];
                Sources.Add(source);
                initial = offset + 1;
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            for (int index = 0; index < Sources.Count; index++)
            {
                Source source = (Source)Sources[index];
                result.AppendFormat(" Source {0}: Usage: {1} - ScaleFactor: {2} - MultiplierSelect: {3}", index, source.Usage, source.ScaleFactor, source.MultiplierSelect);
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}
