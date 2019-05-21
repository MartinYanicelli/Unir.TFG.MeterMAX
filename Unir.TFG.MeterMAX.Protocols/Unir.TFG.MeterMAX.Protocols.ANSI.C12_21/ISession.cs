using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21
{
    public interface ISession : IDisposable
    {
        #region Properties
        IList<IService> Services { get; set; }
        bool IsSessionReady { get; }
        SessionPhase Phase { get; }
        SessionStatus SessionStatus { get; }
        #endregion

        #region Events
        event EventHandler SessionStarting;
        event EventHandler SessionStarted;
        event SetMessageEventHandler SessionStartError;
        event SessionTimeoutEventHandler SessionTimeout;
        event EventHandler SessionClosing;
        event SetMessageEventHandler SessionClosed;
        //event EventHandler SessionStopping;
        //event SetMessageEventHandler SessionStopped;

        event ServiceExecutionEventHandler ServiceExecutionStarted;
        event ServiceExecutionEventHandler ServiceExecutionCompleted;
        event ServiceExecutionEventHandler ServiceExecutionError;
        event ServiceExecutionEventHandler ServiceExecutionCanceled;

        event EventHandler HandshakeInitializing;
        event EventHandler HandshakeInitialized;
        event HandshakeCompletedEventHandler HandshakeCompleted;
        event HandshakeErrorEventHandler HandshakeError;

        event EventHandler NegotiateServiceError;
        event BaudRateChangedEventHandler BaudRateChanged;

        event EventHandler ServicesExecutionStarted;
        event EventHandler ServicesExecutionCompleted;

        event RawDataEventHandler RawDataSent;
        event RawDataEventHandler RawDataReceived;
        event SetMessageEventHandler SetMessage;
        #endregion

        #region Methods
        void Start();
        void Start(System.Threading.CancellationToken cancellation);
        void Start(IList<IService> services);
        void Pause();
        void Resume();
        void Stop();

        void StartServicesExecution();
        void StartServicesExecution(IList<IService> services);
        void RestartServicesExecution();
        #endregion
    }
}
