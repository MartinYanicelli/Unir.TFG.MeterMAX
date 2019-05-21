using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public enum ServiceExceptionType
    {
        PacketIntegrity,
        PacketLenght,
        ArgumentError,
        FatalError
    }

    public class ServiceException : Exception
    {
        public ServiceExceptionType Type { get; set; }

        public ServiceException() : base() { }

        public ServiceException(ServiceExceptionType type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }

        public ServiceException(ServiceExceptionType type, string message)
            : this(type, message, null)
        {

        }
    }
}
