using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

/*
 * The Disconnect Service is used for immediate disconnection of the communication channel.
 * The Disconnect Service is an optional service.
 *  Request:
 *      The Disconnect Service should be used for reasons such as excessive errors, security issues, internal error conditions, end of session, or other reasons.
 *          <disconnect> ::= 22H
 
 * Response:
 *      The responses <sns> and <err> indicate a problem with the received Disconnect Service request and the channel remains open.
 *      The response <ok> indicates the Disconnect Service request was accepted and the channel will be disconnected.
 *          <disconnect-r> ::= <sns> | <err> | <ok>
 * */
namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class DisconnectService : Service
    {
        #region Constructor
        public DisconnectService()
            : base(ServiceRequestCode.Disconnect)
        {
 
        }
        #endregion

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
