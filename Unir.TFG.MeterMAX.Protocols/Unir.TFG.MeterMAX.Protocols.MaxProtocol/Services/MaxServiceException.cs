using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services
{
    public enum MaxServiceExceptionType
    {
        PacketIntegrity,
        PacketLenght,
        ArgumentError,
        FatalError
    }

    public class MaxServiceException : Exception
    {
        public MaxServiceExceptionType Type { get; set; }

        public MaxServiceException() : base() { }

        public MaxServiceException(MaxServiceExceptionType type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }

        public MaxServiceException(MaxServiceExceptionType type, string message)
            : this(type, message, null)
        {

        }
    }
}
