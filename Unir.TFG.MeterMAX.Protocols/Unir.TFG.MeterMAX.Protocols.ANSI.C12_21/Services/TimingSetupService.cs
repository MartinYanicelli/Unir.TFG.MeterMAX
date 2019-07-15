using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

/*
 * The Timing Setup Service provides the mechanism for reconfiguring time-outs, delays and retry attempts from the default values specified in this Standard.
 * The Timing Setup Service is an optional service.
 * The Timing Setup Service is initiated by the C12.21 Client.
 * 
 * Request:
 *          <timing-setup> ::= 71H <traffic><inter-char><resp-to><nbr-retries>
 *                  <traffic> ::= <byte> {Channel traffic time-out in seconds}
 *                  <inter-char> ::= <byte> {Inter-character time-out in seconds}
 *                  <resp-to> ::= <byte> {Response time-out in seconds}
 *                  <nbr-retries> ::= <byte> {Maximum number of retry attempts}
 *                  
 * Response:
 *          The responses <sns>, <isss>, <bsy>, and <err> indicate a problem with the received Timing Setup Service request and the timer parameters will maintain their current settings.
 *          The response <ok> indicates the Timing Setup Service request was accepted and the new settings now apply. 
 *          The data following the <ok> indicates the setting that will apply upon positive acknowledgment.
 *                  <timing-setup-r> ::= <sns> | <isss> | <bsy> | <err> | <ok><traffic><inter-char><resp-to><nbr-retries>
 *                  
 * */
namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class TimingSetupService : Service
    {
        public readonly TimingSetting Setting;

        #region Constructor
        public TimingSetupService(TimingSetting setting) 
            : base(ServiceRequestCode.TimingSetup)
        {
            Setting = setting;
        }
        #endregion
        
        protected override void OnFillRequestDataPacket(Packets.Packet requestPacket)
        {
            base.OnFillRequestDataPacket(requestPacket);
            requestPacket.DATA.Add(Setting.ChannelTrafficTimeout);
            requestPacket.DATA.Add(Setting.InterCharacterTimeout);
            requestPacket.DATA.Add(Setting.ResponseTimeout);
            requestPacket.DATA.Add(Setting.NumberOfRetries);
        }

        protected override void OnProcessResponseData()
        {
            Setting.ChannelTrafficTimeout = responsePacket.DATA[1];
            Setting.InterCharacterTimeout = responsePacket.DATA[2];
            Setting.ResponseTimeout = responsePacket.DATA[3];
            Setting.NumberOfRetries = responsePacket.DATA[4];
        }
    }
}
