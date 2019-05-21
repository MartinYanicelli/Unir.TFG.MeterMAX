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
    public enum ServiceRequestCode : byte
    {
        Identification = 0x20,
        Logon = 0x50,
        Security = 0x51,
        Authenticate = 0x53,
        Read = 0x30, // Full Read
        Write = 0x40, // Full Write
        Logoff = 0x52,
        Negotiate = 0x00,
        Wait = 0x70,
        TimingSetup = 0x71,
        Terminate = 0x21,
        Disconnect = 0x22
    }
}
