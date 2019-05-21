using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum RemoteServiceResponseCode : byte
    {
        Ok = 0x0,
        BadCRC = 0x1,
        CommunicationLockout = 0x2,
        IllegalComandSyntaxLenght = 0x3,
        FramingError = 0x4,
        TimeOutError = 0x5,
        InvalidPassword = 0x6,
        NakFromComputer = 0x7,
        FunctionIsNotAllowed = 0xB,
        RequestInProcess = 0xC,
        TooBusyToHonor = 0xD,
        RequestNotSupported = 0xF,
        MeterUnreachable = 0x40,
        ModemHardwareFailure = 0x41,
        FatalError = 0x42,
        Unknow = 0xFF
    }
}
