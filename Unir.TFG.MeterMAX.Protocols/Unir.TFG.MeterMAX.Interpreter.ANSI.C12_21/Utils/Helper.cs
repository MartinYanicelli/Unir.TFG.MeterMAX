using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils
{
    internal class Helper
    {
        internal static short GetShort(byte[] arr)
        {
            short res = 0;
            JIBitArray jb = new JIBitArray(arr);
            res = 0;
            if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
            {
                jb = jb.Not();
                res = jb.GetShorts()[0];
                res = (short)(res + 1);
                res = (short)(-1 * res);
            }
            else
            {
                res = jb.GetShorts()[0];
            }
            return res;
        }

        internal static int GetInt(byte[] arr)
        {
            int res = 0;
            JIBitArray jb = new JIBitArray(arr);
            res = 0;
            if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
            {
                jb = jb.Not();
                res = jb.GetInt()[0];
                res = res + 1;
                res = -1 * res;
            }
            else
            {
                res = jb.GetInt()[0];
            }
            return res;
        }

        internal static long GetLong(byte[] arr)
        {
            long res = 0;
            JIBitArray jb = new JIBitArray(arr);
            res = 0;
            if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
            {
                jb = jb.Not();
                res = jb.GetLong()[0];
                res = res + 1;
                res = -1 * res;
            }
            else
            {
                res = jb.GetLong()[0];
            }
            return res;
        }

        internal static bool IsBitSet(byte data, int bitPosition)
        {
            return (data & (1 << bitPosition)) != 0;
        }

        internal static DateTime? ConvertToDateTime(byte[] dateArray)
        {
            int year = CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(dateArray[0]); 
            int month = dateArray[1];
            int day = dateArray[2];
            int hour = (dateArray.Length > 3) ? dateArray[3] : 0;
            int minutes = (dateArray.Length > 4) ? dateArray[4] : 0;
            int seconds = (dateArray.Length == 6) ? dateArray[5] : 0;
            return ((year != 0) && (month != 0) && (day != 0)) ? new DateTime(year, month, day, hour, minutes, seconds) : (DateTime?)null;
        }

        internal static string ConvertUOMCodeToString(UOMCode code)
        {
            return (code == UOMCode.ActivePower_W) ? "kW"
                : (code == UOMCode.ReactivePower_VAR) ? "kVAR"
                : (code == UOMCode.ApparentPower_VA) ? "kVA"
                : code.ToString();
        }
    }
}
