using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

/*
 * PSEM requests always include a one-byte request code. Code numbers are represented in hexadecimal format as follows
 *  20H-7FH Codes in this range signify request codes
 * 
*/
namespace Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services.Enumerations
{
    public enum MaxServiceRequestCode : byte
    {
        StartRemoteSession = 0xF8,
        
    }
}
