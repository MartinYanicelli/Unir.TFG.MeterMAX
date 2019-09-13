namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public enum MeterSessionStatus
    {
        StartingSession,
        SessionStarted,
        SessionStartingFailed,
        StartingMeterCommunication,
        MeterCommunicationFailed,
        NegotiatingMeterCommunication,
        MeterCommunicationEstablished,
        ExecutingDataSetComponent,
        MeterSessionItemTaskSuccess,
        MeterSessionItemTaskFailed,
        MeterSessionItemTaskAborted,
        MeterSessionTaskCompleted,
        SessionTimeout,
        EndingSession,
        SessionEnded,
        ReconnectionAttemptsReached,
        FatalError
    }
}