using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;

namespace Unir.TFG.MeterMAX.MDM.Logic.Contracts
{
    public interface IMeterSessionManager : IDisposable
    {
        Task StartAsync(Meter meter, DataSet dataSet, MeterSessionSetting sessionSetting);
        Task StartAsync(Meter meter, DataSet dataSet, MeterSessionTypeCode sessionType);
        void Stop();
        void Pause();
        void Continue();

        event EventHandler<SessionProgressEventArgs> SessionProgressChanged;

        event EventHandler<ReconnectionCountdownEventArgs> ReconnectionCountdownChanged;

        event EventHandler SessionStarted;

        event EventHandler<SessionEndedEventArgs> SessionEnded;

        event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;

    }
}
