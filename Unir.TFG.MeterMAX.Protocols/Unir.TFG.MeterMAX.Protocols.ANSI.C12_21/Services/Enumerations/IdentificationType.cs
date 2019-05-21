using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    /*
     * <feature> ::= <auth-ser> | <auth-ser-ticket> | <device-class> | <device-identity>
     * */
    public enum IdentificationType : byte
    {
        AuthenticationService = 0x01,
        AuthenticationServiceWithTicket = 0x02,
        DeviceClass = 0x06,
        DeviceIdentity = 0x07
    }
}
