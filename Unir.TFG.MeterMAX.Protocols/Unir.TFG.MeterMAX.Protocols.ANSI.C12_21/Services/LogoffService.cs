using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    /*
     * The Logoff Service provides for an orderly shutdown of the session established by the Logon Service.
     * The Logoff Service is a required service.
     * 
     * Request:
     * Following a Logoff Service request, the communication channel will retain the parameters previously established.
     *  <logoff> ::= 52H
     * 
     * Response:
     * The responses <isss>, <bsy>, and <err> indicate a problem with the received Logoff Service request.
     * The response <ok> indicates the acceptance of the Logoff Service and the cessation of the session established by the Logon Service.
     *  <logoff-r> ::= <isss> | <bsy> | <err> | <ok>
     * 
     * */
    public class LogoffService : Service
    {
        
        #region Constructor
        public LogoffService()
            : base(ServiceRequestCode.Logoff)
        {
            
        }
        #endregion

        protected override Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets.Packet OnCreateRequestPacket()
        {
            Packet packet = base.OnCreateRequestPacket();
            packet.CTRL = 0x20;
            return packet;
        }

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
