using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public interface IService
    {
        ServiceResponseCode? ResponseCode { get; }
        bool HasMorePackets { get; }
        bool IsFirstPacket { get; }
        bool IsMultiplePacket { get; }
        DataFormat DataFormat { get; }
        
        byte[] SendRequest();
        void ProcessResponse(byte[] rawResponse);
        void Reset();
    }
}
