using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21
{
    #region Custome Delegate Declarations
    public delegate void BaudRateChangedEventHandler(object sender, BaudRateEventArgs e);
    public delegate void SessionTimeoutEventHandler(object sender, SessionTimeoutEventArgs e);
    public delegate void ServiceExecutionEventHandler(object sender, ServiceExecutionEventArgs e);
    public delegate void HandshakeCompletedEventHandler(object sender, BaudRateEventArgs e);
    public delegate void HandshakeErrorEventHandler(object sender, HandshakeErrorEventArgs e);
    public delegate void SetMessageEventHandler(object sender, SetMessageEventArgs e);
    public delegate void RawDataEventHandler(object sender, RawDataEventArgs e);
    #endregion

    public class SessionException : Exception
    {
        public SessionException(string message) : base(message)
        { }

        public SessionException(string message, Exception innerExepction) : base(message, innerExepction)
        { }
    }

    #region Custome EventArgs Declarations
    public class BaudRateEventArgs : EventArgs
    {
        public int BaudRate { get; set; }

        public BaudRateEventArgs()
            : base()
        {

        }

        public BaudRateEventArgs(int baudRate)
            : base()
        {
            BaudRate = baudRate;
        }
    }

    public class SessionTimeoutEventArgs : EventArgs
    {
        public TimeSpan ElapsedTime { get; set; }

        public SessionTimeoutEventArgs() : base() { }

        public SessionTimeoutEventArgs(TimeSpan elapsedTime)
            : base()
        {
            ElapsedTime = elapsedTime;
        }
    }

    public class SetMessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public SetMessageEventArgs()
            : base()
        {

        }

        public SetMessageEventArgs(string message)
            : base()
        {
            Message = message;
        }
    }

    public class HandshakeErrorEventArgs : SetMessageEventArgs
    {
        public HandshakeState State { get; set; }

        public HandshakeErrorEventArgs() : base() { }

        public HandshakeErrorEventArgs(HandshakeState state, string message)
            : base(message)
        {
            State = state;
        }
    }

    public class ServiceExecutionEventArgs : EventArgs
    {
        public IService Service {get; set;}
        public TimeSpan ElapsedTime { get; set; }
        public int ProgressPercent { get; set; }

        public ServiceExecutionEventArgs(IService service, TimeSpan elapsedTime, int progressPercent)
        {
            Service = service;
            ElapsedTime = elapsedTime;
            ProgressPercent = progressPercent;
        }
        
        public ServiceExecutionEventArgs(IService service) : this(service, TimeSpan.Zero, 0)
        {
            
        }
    }

    public class RawDataEventArgs : EventArgs
    {
        public SessionPhase Phase { get; set; }
        public byte[] Data { get; set; }

        public RawDataEventArgs()
            : base()
        { }

        public RawDataEventArgs(SessionPhase phase, byte[] data)
        {
            Phase = phase;
            Data = data;
        }
    }

    #endregion    
}