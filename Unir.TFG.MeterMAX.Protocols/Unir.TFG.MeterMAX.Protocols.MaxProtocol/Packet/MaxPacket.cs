using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.MaxProtocol.Packets
{
    public class MaxPacket
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

        public MaxPacket()
        {
            STX = 0x02;
            DATA = new List<byte>();
        }
    }
}
