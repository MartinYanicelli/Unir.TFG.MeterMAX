using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets
{
    public class RemotePacket
    {
        public byte STX { get; set; }

        public byte STYPE { get; set; }

        public byte? PAD { get; set; } // ???
        public byte? STAT { get; set; }

        public byte? ACK { get; set; }
        
        public byte? SCODE { get; set; }
        public byte? LEN { get; set; }

        public List<byte> DATA { get; set; }

        public byte? CRCL { get; set; }
        public byte? CRCH { get; set; }

        public RemotePacket()
        {
            STX = 0x02;
            DATA = new List<byte>();
        }
    }
}
