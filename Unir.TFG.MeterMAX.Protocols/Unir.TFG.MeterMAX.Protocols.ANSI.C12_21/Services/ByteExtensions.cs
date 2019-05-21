using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public static class ByteExtensions
    {
        public static bool GetBit(this byte data, int index)
        {
            if ((index < 0) || (index > 7))
                throw new ArgumentOutOfRangeException("index");

            return  (1 == ((data >> index) & 1));
            
            //int shift = (7 - index);
            //byte bitMask = (byte)(1 << shift);
            //return (data & bitMask) != 0;
        }

        public static byte GetSection(this byte data, short section)
        {
            var bitVector = new System.Collections.Specialized.BitVector32(data);
            var bitSection = System.Collections.Specialized.BitVector32.CreateSection(section);
            return (byte)bitVector[bitSection];
        }
    }
}
