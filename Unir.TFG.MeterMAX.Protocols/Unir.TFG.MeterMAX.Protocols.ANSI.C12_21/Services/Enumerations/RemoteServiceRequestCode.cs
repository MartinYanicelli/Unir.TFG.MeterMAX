using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

/*
 * PSEM requests always include a one-byte request code. Code numbers are represented in hexadecimal format as follows
 *  20H-7FH Codes in this range signify request codes
 * 
*/
namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum RemoteServiceRequestCode : byte
    {
        StartRemoteSession = 0xF8,
        
    }
}
