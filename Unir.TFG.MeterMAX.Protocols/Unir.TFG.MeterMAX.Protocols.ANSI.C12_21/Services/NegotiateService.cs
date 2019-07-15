using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    /*
     * The Negotiate Service provides the mechanism for reconfiguring the communication channel for communication parameters differing from 
     * the default values specified in this Standard. The Negotiate Service is an optional service. 
     * The Negotiate Service shall be issued only after the Identification Service and before the Logon Service. 
     * The negotiated parameters shall remain in effect until re-negotiated or the communication channel is closed.
     * 
     * Request:
     *      <negotiate> ::= <baud-rate-selector><packet-size><nbr-packets><baud-rate>*
     *          <baud-rate-selector>::= 60H |                   {No <baud rate> included in request. Stay at default baud rate}
     *                                  61H |                   {1 <baud rate> included in request} 
     *                                  62H |                   {2 <baud rate> included in request}
     *                                  63H |                   {3 <baud rate> included in request}
     *                                  64H |                   {4 <baud rate> included in request}
     *                                  65H |                   {5 <baud rate> included in request} 
     *                                  66H |                   {6 <baud rate> included in request}
     *                                  67H |                   {7 <baud rate> included in request}
     *                                  68H |                   {8 <baud rate> included in request}
     *                                  69H |                   {9 <baud rate> included in request}
     *                                  6AH |                   {10 <baud rate> included in request}
     *                                  6BH                     {11 <baud rate> included in request}
     *          <packet-size> ::= <word16>                      {Maximum packet size supported, in bytes. This value shall be in the range of 64 - 8192 bytes, inclusive.}
     *          <nbr-packets> ::= <byte>                        {Maximum number of packets this layer is able to reassemble into an upper layer data structure at one time. 
     *                                                          The value zero (0) is reserved for future standard extension.}
     *          <baud-rate> ::=         00H |                   {Externally defined}
     *                                  01H |                   {300 baud}
     *                                  02H |                   {600 baud}
     *                                  03H |                   {1200 baud}
     *                                  04H |                   {2400 baud}
     *                                  05H |                   {4800 baud}
     *                                  06H |                   {9600 baud}
     *                                  07H |                   {14400 baud}
     *                                  08H |                   {19200 baud}
     *                                  09H |                   {28800 baud}
     *                                  0AH |                   {57600 baud}
     *                                  0BH |                   {38400 baud}
     *                                  0CH |                   {115200 baud}
     *                                  0DH |                   {128000 baud}
     *                                  0EH                     {256000 baud}
     *                                  0FH – FFH               {reserved}
     * 
     * 
     * Response:
     *      <negotiate-r> ::=   <sns> | <isss> | <bsy> | <err> |
     *                          <ok><packet-size><nbr-packets><baud-rate>
     * 
     * */
    public class NegotiateService : Service
    {
        public NegotiationSetting Setting { private set; get; }
        
        public NegotiateService(NegotiationSetting setting)
            : base(ServiceRequestCode.Negotiate) 
        {
            Setting = setting;
        }

        protected override Packet OnCreatePacket()
        {
            Packet packet = base.OnCreatePacket();
            packet.CTRL = 0x20;
            return packet;
        }

        protected override void OnProcessResponseData()
        {
            Setting.PacketSize = BitConverter.ToInt16(new byte[] { responsePacket.DATA[2], responsePacket.DATA[1] }, 0);
            Setting.NumberOfPackets = responsePacket.DATA[3];
            Setting.BaudRate = (BaudRateValue)responsePacket.DATA[4];
        }

        protected override void OnFillRequestDataPacket(Packet requestPacket)
        {
            requestPacket.DATA.Add((byte)Setting.BaudRateSelector);
            requestPacket.DATA.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes(Setting.PacketSize).Reverse() : BitConverter.GetBytes(Setting.PacketSize));
            requestPacket.DATA.Add(Setting.NumberOfPackets);
            requestPacket.DATA.Add((byte)Setting.BaudRate);
        }
    }
}
