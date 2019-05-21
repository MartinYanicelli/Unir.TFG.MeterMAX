using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    /*
     * The Terminate Service provides for an immediate cessation of the communication channel and aborts an open session, 
     * and implicitly invokes the Logoff Service. This service is optional.
     * 
     * Request: 
     *  The Terminate Service may be used to terminate either partially or fully established 
     *  communication channels for reasons such a excessive errors, security issues, internal error conditions, end of session, 
     *  or other reasons as set by the device manufacturer. When the Terminate Service is invoked within an open session, 
     *  any or all session-oriented transactions may be lost or may be rolled back to values that existed at the start of session.
     *      <terminate> ::= 21H
     *      
     * Response:
     *  The responses <sns> and <err> indicates a problem with the received Terminate Service request and the channel remains open.
     *  The response <ok> indicates the service request was accepted and the channel will return to default settings as stated, 
     *  Default Settings, upon receipt of a positive acknowledgment.
     *      <terminate-r> ::= <sns> | <err> | <ok>
     *                        
     * 
     * */

    public class TerminateService : Service
    {
        #region Constructor
        public TerminateService()
            : base(ServiceRequestCode.Terminate)
        {
 
        }
        #endregion

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
