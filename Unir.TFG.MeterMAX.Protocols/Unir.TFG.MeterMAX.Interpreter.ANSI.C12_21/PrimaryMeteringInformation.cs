using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class PrimaryMeteringInformation : TableParser
    {
        public short ExternalMultiplier_ScaleFactor { get; private set; }
        public int ExternalMultiplier { get; private set; }
        public short Adjusted_Ke_Scale_Factor { get; private set; }
        public short Adjusted_Kh_Scale_Factor { get; private set; }
        public long Adjusted_Kh { get; private set; }
        public short Adjusted_p_r { get; private set; }
        public long Calculated_Transformer_Factor { get; private set; }

        protected override void OnParse(byte[] table)
        {
            int offset = 0;
            byte[] exMultiplierScaleFactorArray = new byte[1];
            exMultiplierScaleFactorArray[0] = table[offset];
            ExternalMultiplier_ScaleFactor = Helper.GetShort(exMultiplierScaleFactorArray);
            offset++;

            byte[] exMultiplierArray = new byte[4];
            Array.Copy(table, offset, exMultiplierArray, 0, 4);
            Array.Reverse(exMultiplierArray);
            ExternalMultiplier = Helper.GetInt(exMultiplierArray);
            offset = offset + 4;

            byte[] adjustedKeScaleFactorArray = new byte[1];
            adjustedKeScaleFactorArray[0] = table[offset];
            Adjusted_Ke_Scale_Factor = Helper.GetShort(adjustedKeScaleFactorArray);
            offset++;

            byte[] adjustedKhScaleFactorArray = new byte[1];
            adjustedKhScaleFactorArray[0] = table[offset];
            Adjusted_Kh_Scale_Factor = Helper.GetShort(adjustedKhScaleFactorArray);
            offset++;

            byte[] adjustedKhArray = new byte[6];
            Array.Copy(table, offset, adjustedKhArray, 0, 6);
            Array.Reverse(adjustedKhArray);
            Adjusted_Kh = Helper.GetLong(adjustedKhArray);
            offset = offset + 6;

            byte[] adjustedPrArray = new byte[1];
            adjustedPrArray[0] = table[offset];
            Adjusted_p_r = Helper.GetShort(adjustedPrArray);
            offset++;

            byte[] calculatedTransformerFactorArray = new byte[6];
            Array.Copy(table, offset, calculatedTransformerFactorArray, 0, 6);
            Array.Reverse(calculatedTransformerFactorArray);
            Calculated_Transformer_Factor = Helper.GetLong(calculatedTransformerFactorArray);
            offset = offset + 6;
            
        }
        
        public override string ToString()
        {
            return string.Format("External Multiplier Scale Factor: {0} - External Multiplier: {1} - Adjusted Ke Scale Factor: {2} - Adjusted Kh Scale Factor: {3} -  Adjusted Kh: {4} - Adjusted_p_r: {5} - Calculated Transformer Factor: {6}", 
                ExternalMultiplier_ScaleFactor, ExternalMultiplier, Adjusted_Ke_Scale_Factor, Adjusted_Kh_Scale_Factor, Adjusted_Kh, Adjusted_p_r, Calculated_Transformer_Factor);
        }
    }
}
