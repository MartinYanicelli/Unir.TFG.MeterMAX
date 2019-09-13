using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;
using Unir.TFG.MeterMAX.MDM.Logic.Contracts;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.MDM.Logic
{
    public class MeterSessionManager : IMeterSessionManager
    {
        private static readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly StringBuilder sessionTracer = new StringBuilder();
        private ISession ansiProtocolSession;

        private readonly IInsituMeterSessionSettingRepository insituMeterSessionSettingRepository;
        private readonly IRemoteMeterSessionSettingRepository remoteMeterSessionSettingRepository;
        private readonly IDataSetComponentRepository dataSetComponentRepository;

        private TimingSetting timingSetting;
        private NegotiationSetting negotiationSetting;

        // controlar la cantidad de reconexiones automáticas permitidas.
        private int handshakeInitializingCount;
        private const int MAX_HANDSHAKE_INITIALIZING_COUNT = 3;

        // Reconnection Timer.
        private Timer reconnectionCountdownTimer = null;
        private TimeSpan currentReconnectionCountdown = TimeSpan.Zero;
        private int reconnectionAttempts;

        private long totalBytesSent = 0;
        private long totalBytesReceived = 0;

        public event EventHandler<SessionProgressEventArgs> SessionProgressChanged;
        public event EventHandler<ReconnectionCountdownEventArgs> ReconnectionCountdownChanged;
        public event EventHandler SessionStarted;
        public event EventHandler<SessionEndedEventArgs> SessionEnded;
        public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;

        public MeterSession CurrentMeterSession { get; private set; }

        #region Constructor
        public MeterSessionManager(IInsituMeterSessionSettingRepository insituMeterSessionSettingRepository, IRemoteMeterSessionSettingRepository remoteMeterSessionSettingRepository,  IDataSetComponentRepository dataSetComponentRepository)
        {
            this.insituMeterSessionSettingRepository = insituMeterSessionSettingRepository ?? throw new ArgumentNullException(nameof(insituMeterSessionSettingRepository));
            this.remoteMeterSessionSettingRepository = remoteMeterSessionSettingRepository ?? throw new ArgumentNullException(nameof(remoteMeterSessionSettingRepository));
            this.dataSetComponentRepository = dataSetComponentRepository ?? throw new ArgumentNullException(nameof(dataSetComponentRepository));
        }
        #endregion

        #region Public Methods
        public async Task StartAsync(Meter meter, DataSet dataSet, MeterSessionSetting sessionSetting)
        {
            CurrentMeterSession = new MeterSession(meter, dataSet) {
                Guid = Guid.NewGuid(),
                SessionSetting = sessionSetting ?? throw new ArgumentNullException(nameof(sessionSetting)),
                StartDate = DateTime.UtcNow
            };

            sessionTracer.Clear();

            if (sessionSetting.CommunicationChannelSetting != null)
            {
                if ((sessionSetting.CommunicationChannelSetting.BaudRate != BaudRate.BaudRate_9600) ||
                   (sessionSetting.CommunicationChannelSetting.NumberOfPackets != ((sessionSetting is InsituMeterSessionSetting) ? InsituSession.DefaultNumberOfPackets : RemoteSession.DefaultNumberOfPackets)) ||
                   (sessionSetting.CommunicationChannelSetting.PacketSize != (PacketSize) ((sessionSetting is InsituMeterSessionSetting) ? InsituSession.DefaultPacketSize : RemoteSession.DefaultPacketSize)))
                {
                    negotiationSetting = new NegotiationSetting()
                    {
                        BaudRate = (BaudRateValue)sessionSetting.CommunicationChannelSetting.BaudRate,
                        BaudRateSelector = BaudRateSelector.OneBaudRateRequested,
                        NumberOfPackets = sessionSetting.CommunicationChannelSetting.NumberOfPackets,
                        PacketSize = (short)sessionSetting.CommunicationChannelSetting.PacketSize
                    };
                }

                if ((sessionSetting.CommunicationChannelSetting.ChannelTrafficTimeout != ((sessionSetting is InsituMeterSessionSetting) ? InsituSession.DefaultChannelTrafficTimeout : RemoteSession.DefaultChannelTrafficTimeout)) ||
                   (sessionSetting.CommunicationChannelSetting.InterCharacterTimeout != ((sessionSetting is InsituMeterSessionSetting) ? InsituSession.DefaultInterCharacterTimeout : RemoteSession.DefaultInterCharacterTimeout)) ||
                   (sessionSetting.CommunicationChannelSetting.ResponseTimeout != ((sessionSetting is InsituMeterSessionSetting) ? InsituSession.DefaultResponseTimeout : RemoteSession.DefaultResponseTimeout)) ||
                   (sessionSetting.CommunicationChannelSetting.NumberOfRetries != ((sessionSetting is InsituMeterSessionSetting) ? InsituSession.DefaultRetryAttempts : RemoteSession.DefaultRetryAttempts))) 
                {
                    timingSetting = new TimingSetting()
                    {
                        ChannelTrafficTimeout = sessionSetting.CommunicationChannelSetting.ChannelTrafficTimeout,
                        InterCharacterTimeout = sessionSetting.CommunicationChannelSetting.InterCharacterTimeout,
                        ResponseTimeout = sessionSetting.CommunicationChannelSetting.ResponseTimeout,
                        NumberOfRetries = sessionSetting.CommunicationChannelSetting.NumberOfRetries
                    };
                }

                reconnectionCountdownTimer = (sessionSetting.ReconnectionSchema != null) ? new Timer(OnReconnectionCountDown, null, -1, -1) : null;
                totalBytesReceived = 0; totalBytesSent = 0;
            }

            ansiProtocolSession = CreateProtocolSession();
            await ansiProtocolSession.StartAsync();
        }

        public async Task StartAsync(Meter meter, DataSet dataSet, MeterSessionTypeCode sessionType)
        {
            MeterSessionSetting meterSessionSetting = (sessionType == MeterSessionTypeCode.Insitu)
                ? insituMeterSessionSettingRepository.FindOne(new Noanet.XamArch.Domain.EntityPropertyInfo(nameof(RemoteMeterSessionSetting.Name), "Default"))
                : (MeterSessionSetting) remoteMeterSessionSettingRepository.FindOne(new Noanet.XamArch.Domain.EntityPropertyInfo(nameof(InsituMeterSessionSetting.Name), "Default"));

            await StartAsync(meter, dataSet, meterSessionSetting);
        }

        public void Stop()
        {
            if (ansiProtocolSession == null)
                return;

            OnClose();
        }

        /// <summary>
        /// Pausa temporalmente el proceso de lectura contra un Medidor.
        /// </summary>
        public void Pause()
        {
            if (ansiProtocolSession == null)
                return;

            handshakeInitializingCount = 0;
            ansiProtocolSession.Pause();
        }

        /// <summary>
        /// Continúa con un proceso de lectuar previamente pausado.
        /// </summary>
        public void Continue()
        {
            ansiProtocolSession?.Resume();
        }

        #endregion  

        #region Private Methods

        private async Task Restart()
        {
            ansiProtocolSession = CreateProtocolSession();
            await ansiProtocolSession.StartAsync();
        }

        private ISession CreateProtocolSession()
        {
            var session = (CurrentMeterSession.SessionSetting is InsituMeterSessionSetting) 
                ? new InsituSession(((InsituMeterSessionSetting)CurrentMeterSession.SessionSetting).PortName, 
                    CurrentMeterSession.SessionSetting.ProtocolSetting.UserId, 
                    CurrentMeterSession.SessionSetting.ProtocolSetting.UserName, 
                    CurrentMeterSession.SessionSetting.ProtocolSetting.Password, 
                    CurrentMeterSession.SessionSetting.CommunicationChannelSetting.SendAckResponseThershold, 
                    (int)CurrentMeterSession.SessionSetting.CommunicationChannelSetting.BaudRate,
                    ((InsituMeterSessionSetting)CurrentMeterSession.SessionSetting).DtrEnabled,
                    null, // receivedBytesThershold
                    negotiationSetting, timingSetting) 
               : (ISession) new RemoteSession(CurrentMeterSession.Meter.RemoteDevice.Ip,
                    CurrentMeterSession.Meter.RemoteDevice.PortNumber,
                    CurrentMeterSession.SessionSetting.ProtocolSetting.UserId,
                    CurrentMeterSession.SessionSetting.ProtocolSetting.UserName,
                    CurrentMeterSession.SessionSetting.ProtocolSetting.Password,
                    CurrentMeterSession.SessionSetting.CommunicationChannelSetting.SendAckResponseThershold, 
                    CurrentMeterSession.SessionSetting.InternalReconnectionAttempts, 
                    negotiationSetting, timingSetting, ((RemoteMeterSessionSetting)CurrentMeterSession.SessionSetting).UseMeterMAXProtocol);
                
            session.SessionStarting += Session_Starting;
            session.SessionStartError += Session_StartError;
            session.SessionStarted += Session_Started;
            session.SessionTimeout += Session_Timeout;

            session.HandshakeInitializing += Session_HandshakeInitializing;
            session.HandshakeInitialized += Session_HandshakeInitialized;
            session.HandshakeError += Session_HandshakeError;
            session.HandshakeCompleted += Session_HandshakeCompleted;

            session.ServiceExecutionStarted += Session_ServiceExecutionStarted;
            session.ServiceExecutionCompleted += Session_ServiceExecutionCompleted;
            session.ServiceExecutionError += Session_ServiceExecutionError;
            session.ServiceExecutionCanceled += Session_ServiceExecutionCanceled;

            session.ServicesExecutionCompleted += Session_ServicesExecutionCompleted;

            session.SessionClosing += Session_Closing;
            session.SessionClosed += Session_Closed;

#if DEBUG
            session.RawDataSent += Session_RawDataSent;
            session.RawDataReceived += Session_RawDataReceived;
#endif

            handshakeInitializingCount = 0;

            PrepareMeterSessionTasks();

            session.Services = CurrentMeterSession.SessionTasks.Values
                .SelectMany(x => x.Items.Select(z => z.Key as IService))
                .ToList();

            return session;
        }


        private void TerminateMeterSession()
        {
            IList<IService> services = ansiProtocolSession.Services;

            try
            {
#if DEBUG
                ansiProtocolSession.RawDataSent -= Session_RawDataSent;
                ansiProtocolSession.RawDataReceived -= Session_RawDataReceived;
#endif

                ansiProtocolSession.SessionStarting -= Session_Starting;
                ansiProtocolSession.SessionStartError -= Session_StartError;
                ansiProtocolSession.SessionStarted -= Session_Started;
                ansiProtocolSession.SessionTimeout -= Session_Timeout;

                ansiProtocolSession.HandshakeInitializing -= Session_HandshakeInitializing;
                ansiProtocolSession.HandshakeInitialized -= Session_HandshakeInitialized;
                ansiProtocolSession.HandshakeError -= Session_HandshakeError;
                ansiProtocolSession.HandshakeCompleted -= Session_HandshakeCompleted;

                ansiProtocolSession.ServiceExecutionStarted -= Session_ServiceExecutionStarted;
                ansiProtocolSession.ServiceExecutionCompleted -= Session_ServiceExecutionCompleted;
                ansiProtocolSession.ServiceExecutionError -= Session_ServiceExecutionError;
                ansiProtocolSession.ServiceExecutionCanceled -= Session_ServiceExecutionCanceled;

                ansiProtocolSession.ServicesExecutionCompleted -= Session_ServicesExecutionCompleted;

                ansiProtocolSession.SessionClosing -= Session_Closing;
                ansiProtocolSession.SessionClosed -= Session_Closed;

                ansiProtocolSession.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dispose Session Error. {ex.Message}");
            }
            finally
            {
                ansiProtocolSession = null;
            }

            reconnectionAttempts++;

            var taskCompletedCount = CurrentMeterSession.SessionTasks.Values.Count(x => x.Completed);

            if ((taskCompletedCount > 0) || 
                (CurrentMeterSession.SessionSetting.ReconnectionSchema == null) || 
                (reconnectionAttempts > CurrentMeterSession.SessionSetting.ReconnectionSchema.MaxReconnectionAttempts))
            {
                reconnectionCountdownTimer?.Dispose();

                CurrentMeterSession.EndDate = DateTime.UtcNow;
                CurrentMeterSession.SessionTrace = sessionTracer.ToString();


                int dataSetExecutionPercent = taskCompletedCount * 100 / CurrentMeterSession.SessionTasks.Count;
                var dataSetExecutionQuality = (dataSetExecutionPercent == 0) ? DataSetExecutionQuality.Critical
                                                                                                : (dataSetExecutionPercent <= 25) ? DataSetExecutionQuality.VeryPoor
                                                                                                : (dataSetExecutionPercent <= 50) ? DataSetExecutionQuality.Poor
                                                                                                : (dataSetExecutionPercent <= 75) ? DataSetExecutionQuality.Moderate
                                                                                                : (dataSetExecutionPercent < 100) ? DataSetExecutionQuality.Good
                                                                                                : DataSetExecutionQuality.Great;
                var sessionEndedEventArgs = new SessionEndedEventArgs() {
                    MeterSession = CurrentMeterSession,
                    TotalBytesSent = totalBytesSent,
                    TotalBytesReceived = totalBytesReceived,
                    DataSetExecutionSuccess = taskCompletedCount == CurrentMeterSession.SessionTasks.Count,
                    DataSetExecutionPercent = dataSetExecutionPercent,
                    DataSetExecutionQuality = dataSetExecutionQuality
                };

                SessionEnded?.Invoke(this, sessionEndedEventArgs);

                reconnectionAttempts = 0;
                sessionTracer.Clear();
                totalBytesSent = 0; totalBytesReceived = 0;
            }
            else
            {
                currentReconnectionCountdown = CurrentMeterSession.SessionSetting.ReconnectionSchema.ReconnectionSchedules[reconnectionAttempts - 1].Schedule; 
                reconnectionCountdownTimer.Change(1000, 1000); // disparamos cada 1 segundos hasta que se waitTimeSpan llegue a cero.
            }
        }

        private void OnClose()
        {
            try
            {
                ansiProtocolSession.Stop();
            }
            catch (Exception ex)
            {
                // consumimos, solo logueamos...
                logger.Error(ex, nameof(OnClose));
            }
        }

        private void OnFatalError(string message)
        {
            OnClose();
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.FatalError, $"#{message}");
        }

        private void OnReconnectionAttemptsReached()
        {
            OnClose();
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.ReconnectionAttemptsReached, "#Se excedió el límite de intentos automáticos de comunicación remota");
        }

        private void PrepareMeterSessionTasks()
        {
            foreach (var dataSetComponentId in CurrentMeterSession.SessionTasks.Keys)
            {
                var sessionTask = CurrentMeterSession.SessionTasks[dataSetComponentId];
                var dataSetComponentCode = (DataSetComponentCodes)dataSetComponentId;
                var services = new List<Service>();

                switch (dataSetComponentCode)
                {
                    case DataSetComponentCodes.FirmwareAndIdentification:
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.GeneralManufacturerIdentification) }));
                        break;
                    case DataSetComponentCodes.BillingConfiguration:
                        services.Add(new ReadService(ReadingType.PartialReadWithOffset, new object[] { new Table(TableName.ElsterSourceDefinitionTable), 0, 350 /* (7 * 50) */ }));
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.ActualRegisterTable) }));
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.DataSelectionTable) }));
                        break;
                    case DataSetComponentCodes.CurrentBilling:
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.CurrentRegisterDataTable) }));
                        break;
                    case DataSetComponentCodes.PreviousBilling:
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.PreviousDemandResetDataTable) }));
                        break;
                    case DataSetComponentCodes.StatusData:
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.ModeAndStatusStandardTable) }));
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.StatusManufacturerTable) }));
                        break;
                    case DataSetComponentCodes.EventAndLogs:
                        break;
                    case DataSetComponentCodes.Instrumentation:
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.Multipliers) }));
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.PrimaryMeteringInformation) }));
                        break;
                    case DataSetComponentCodes.LoadProfileConfiguration:
                        services.Add(new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.ActualLoadProfileTable) }));
                        break;
                    case DataSetComponentCodes.LoadProfileData:
                        break;
                    default:
                        throw new NotImplementedException($"El Componente {sessionTask.DataSetComponent.Name} no tiene implementado una tarea de sesión asociada");
                }

                sessionTask.Items = services.ToDictionary(k => (object)k, v => new MeterSessionItemTask() { Name = v.GetType().AssemblyQualifiedName, SessionTask = sessionTask, Success = false });
            }

            if ((CurrentMeterSession.SessionTasks.ContainsKey((int)DataSetComponentCodes.CurrentBilling) || CurrentMeterSession.SessionTasks.ContainsKey((int)DataSetComponentCodes.PreviousBilling)) && !CurrentMeterSession.SessionTasks.ContainsKey((int)DataSetComponentCodes.BillingConfiguration))
            {
                AddMeterSessionTask((int)DataSetComponentCodes.BillingConfiguration,
                    new ReadService(ReadingType.PartialReadWithOffset, new object[] { new Table(TableName.ElsterSourceDefinitionTable), 0, 350 /* (7 * 50) */ }),
                    new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.ActualRegisterTable) }),
                    new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.DataSelectionTable) }));
            }

            if (CurrentMeterSession.SessionTasks.ContainsKey((int)DataSetComponentCodes.LoadProfileData) && !CurrentMeterSession.SessionTasks.ContainsKey((int)DataSetComponentCodes.LoadProfileConfiguration))
            {
                AddMeterSessionTask((int)DataSetComponentCodes.LoadProfileConfiguration,
                new ReadService(ReadingType.FullRead, new object[] { new Table(TableName.ActualLoadProfileTable) }));
            }
        }

        private void AddMeterSessionTask(int dataSetComponentId, params Service[] services)
        {
            var dataSetComponent = dataSetComponentRepository.Get(dataSetComponentId);
            CurrentMeterSession.SessionTasks.Add(dataSetComponentId,
                    new MeterSessionTask()
                    {
                        Name = dataSetComponent.Name,
                        DataSetComponent = dataSetComponent,
                        Items = new Dictionary<object, MeterSessionItemTask>()
                    });
            var sessionTask = CurrentMeterSession.SessionTasks[(int)DataSetComponentCodes.BillingConfiguration];
            foreach (var service in services)
            {
                sessionTask.Items.Add(service, new MeterSessionItemTask()
                {
                    Name = service.GetType().AssemblyQualifiedName,
                    SessionTask = sessionTask,
                    Success = false
                });
            }
        }

        private MeterSessionItemTask GetMeterSessionItemTask(IService service) => CurrentMeterSession.SessionTasks.Values.Single(x => x.Items.ContainsKey(service)).Items[service] as MeterSessionItemTask; 

        private void RaiseSessionStatusChangedAndTrace(MeterSessionStatus sessionStatus, string message)
        {
            sessionTracer.AppendLine(message);
            SessionStatusChanged?.Invoke(this, new SessionStatusChangedEventArgs() { SessionStatus = sessionStatus, CurrentSessionTrace = sessionTracer.ToString() });
        }

        private void OnReconnectionCountDown(object wait)
        {
            if (currentReconnectionCountdown > TimeSpan.Zero)
            {
                ReconnectionCountdownChanged?.Invoke(this, new ReconnectionCountdownEventArgs() { RemainingTime = currentReconnectionCountdown });
                currentReconnectionCountdown -= TimeSpan.FromSeconds(1);
            }
            else
            {
                reconnectionCountdownTimer.Change(-1, -1);
                Task.Run(() => Restart());
            }
        }

        #endregion

        #region Protocol Session Event Handlers

        private void Session_Starting(object sender, EventArgs e)
        {
           RaiseSessionStatusChangedAndTrace(MeterSessionStatus.StartingSession, $"#Iniciando Sesión de Comunicación. Fecha y Hora (UTC) {DateTime.UtcNow.ToString("dd-MM-yy HH:mm:ss fff")}");
        }

        private void Session_Started(object sender, EventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.SessionStarted, "#Sesión Iniciada. Estableciendo Conexión");
            SessionStarted?.Invoke(this, new EventArgs());
        }

        private void Session_StartError(object sender, SetMessageEventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.SessionStartingFailed, $"#Error al iniciar Sesión de Comunicación. {e.Message}");
            TerminateMeterSession();
        }

        private void Session_Timeout(object sender, SessionTimeoutEventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.SessionTimeout, $"#Timeout de Comunicación. Fase: {ansiProtocolSession.Phase} - Medidor N°: {CurrentMeterSession.Meter.SerialNumber}, Modelo: {CurrentMeterSession.Meter.Model ?? "N/E"}");
        }

        private void Session_HandshakeInitializing(object sender, EventArgs e)
        {
            handshakeInitializingCount++;
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.StartingMeterCommunication, "#Solicitando comunicación al medidor");
            if (handshakeInitializingCount > MAX_HANDSHAKE_INITIALIZING_COUNT)
            {
                OnReconnectionAttemptsReached();
            }
        }

        private void Session_HandshakeInitialized(object sender, EventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.NegotiatingMeterCommunication, "#¡Conectado! Negociando Comunicación");
        }

        private void Session_HandshakeError(object sender, HandshakeErrorEventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.MeterCommunicationFailed, $"#{e.Message}");
        }

        private void Session_HandshakeCompleted(object sender, BaudRateEventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.MeterCommunicationEstablished, $"Conectado a {e.BaudRate}");
        }

        private void Session_ServiceExecutionStarted(object sender, ServiceExecutionEventArgs e)
        {
            var currentMeterSessionItemTask = GetMeterSessionItemTask(e.Service);
            currentMeterSessionItemTask.StartDate = DateTime.UtcNow;
            if (e.Service is ReadService readService)
            {
                RaiseSessionStatusChangedAndTrace(MeterSessionStatus.ExecutingDataSetComponent, $"#Iniciando Lectura de Tabla {readService.Table.Name}. Fecha y Hora (UTC): {currentMeterSessionItemTask.StartDate.ToString("dd-MM-yy HH:mm:ss fff")}");
            }
        }

        private void Session_ServiceExecutionCompleted(object sender, ServiceExecutionEventArgs e)
        {
            handshakeInitializingCount = 0;

            var currentMeterSessionItemTask = GetMeterSessionItemTask(e.Service);
            var currentSessionTask = currentMeterSessionItemTask.SessionTask;
            currentMeterSessionItemTask.Success = true;
            currentMeterSessionItemTask.EndDate = DateTime.UtcNow;

            // verificamos si todos los items de la tarea están completos...
            if (currentSessionTask.Items.Count(x => !x.Value.Success) == 0)
            {
                currentSessionTask.Completed = true;
                currentSessionTask.EndDate = DateTime.UtcNow;
                SessionProgressChanged?.Invoke(this, new SessionProgressEventArgs() { SessionTask = currentSessionTask, ElapsedTime = e.ElapsedTime, PercentAdvance =  e.ProgressPercent });
            }

            if (e.Service is ReadService readService)
            {
                byte[] rawData = readService.TableData.ToArray();
                currentMeterSessionItemTask.TaskResult = rawData;
                RaiseSessionStatusChangedAndTrace(MeterSessionStatus.MeterSessionItemTaskSuccess, $"#Lectura satisfactoria de Tabla {readService.Table.Name}. Tiempo Requerido: {e.ElapsedTime.ToString()}. Porcentaje de Avance: {e.ProgressPercent}");

                switch (readService.Table.Name)
                {
                    case TableName.GeneralManufacturerIdentification:
                        if (!OnConfigurationTablesCompleted(rawData))
                        { 
                            OnFatalError("No se puede determinar las caractarísticas técnicas del Medidor. Intente la lectura nuevamente.");
                        }
                        break;
                    case TableName.ActualLoadProfileTable:
                        if (!OnLoadProfileConfigurationCompleted(rawData))
                        {
                            OnFatalError("No se puede determinar la configuración del Perfil de Carga del Medidor. Intente la Lectura nuevamente.");
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (e.Service is WriteService writeService)
            {
                //if (writeService.Table.Name == TableName.HistoryLogControlTable)
                //{
                //    _sessionTracer.AppendLine("#AutoReset: Realizado");
                //    DemandResetStatus = OpticalDemandResetStatus.Performed;
                //}
                //else if (writeService.Table.Name == TableName.EventLogControlTable)
                //{
                //    _sessionTracer.AppendLine("#Fecha del Medidor: Actualizada");
                //}
            }
            
        }

        private void Session_ServiceExecutionError(object sender, ServiceExecutionEventArgs e)
        {
            if (e.Service is ReadService readService)
            {
                RaiseSessionStatusChangedAndTrace(MeterSessionStatus.MeterSessionItemTaskFailed, $"#Error durante Lectura de Tabla {readService.Table.Name}. Código de Resultado: {readService.ResponseCode}. Fecha y Hora (UTC): {DateTime.UtcNow.ToString("dd-MM-yy HH:mm:ss fff")}");
            }
            else if (e.Service is WriteService writeService)
            {
                //if (writeService.Table.Name == TableName.EventLogControlTable)
                //{
                //    AddSessionTrace("#Fecha Medidor: No actualizada. Error al intentar corregir");
                //}
                //else if (writeService.Table.Name == TableName.HistoryLogControlTable)
                //{
                //    AddSessionTrace("#AutoReset: No realizado. Error al intentar un Reseteo de Demanda Automático.");
                //    DemandResetStatus = OpticalDemandResetStatus.NotPerformed;
                //}
            }
        }

        private void Session_ServiceExecutionCanceled(object sender, ServiceExecutionEventArgs e)
        {
            if (e.Service is ReadService readService)
            {
                RaiseSessionStatusChangedAndTrace(MeterSessionStatus.MeterSessionItemTaskAborted, $"#Lectura Cancelada de Tabla {readService.Table.Name}. Fecha y Hora (UTC): {DateTime.UtcNow.ToString("dd-MM-yy HH:mm:ss fff")}");
            }
            if (e.Service is WriteService writeService)
            {
                //if (writeService.Table.Name == TableName.HistoryLogControlTable)
                //{
                //    AddSessionTrace("#Fecha Medidor: Lectura cancelada durante el cambio de fecha y hora");
                //}
                //else if (writeService.Table.Name == TableName.EventLogControlTable)
                //{
                //    AddSessionTrace("#AutoReset: Lectura cancelada durante el Reseteo de Demanda");
                //    DemandResetStatus = OpticalDemandResetStatus.Unknow;
                //}
            }
        }

        void Session_ServicesExecutionCompleted(object sender, EventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.MeterSessionTaskCompleted, "#¡ENHORABUENA! Ejecución Completa del DataSet");
        }

        void Session_Closing(object sender, EventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.EndingSession, "#Cerrando Sesión de comunicación");
        }

        void Session_Closed(object sender, SetMessageEventArgs e)
        {
            RaiseSessionStatusChangedAndTrace(MeterSessionStatus.SessionEnded, $"#{e.Message}");
            TerminateMeterSession();
        }

        void Session_RawDataSent(object sender, RawDataEventArgs e)
        {
            totalBytesSent += e.Data.LongLength;
            StringBuilder data = new StringBuilder($"SessionPhase: {e.Phase} - RawDataSended: ");
            foreach (var item in e.Data)
            {
                data.AppendFormat("{0:X2} ", item);
            }
            System.Diagnostics.Debug.WriteLine(data.ToString());
        }

        void Session_RawDataReceived(object sender, RawDataEventArgs e)
        {
            totalBytesReceived += e.Data.LongLength;
            StringBuilder data = new StringBuilder($"SessionPhase: {e.Phase} - RawDataReceived: ");
            foreach (var item in e.Data)
            {
                data.AppendFormat("{0:X2} ", item);
            }
            System.Diagnostics.Debug.WriteLine(data.ToString());
        }

        #endregion Protocol Session Event Handlers

        #region Control Each Read Section

        protected virtual bool OnConfigurationTablesCompleted(byte[] data)
        {
            bool result = true;
            try
            {
                GeneralManufacturerIdentificationTable tableParser = new GeneralManufacturerIdentificationTable();
                tableParser.Parse(data);
                long meterSerialNumber = long.Parse(tableParser.SerialNumber, System.Globalization.NumberStyles.AllowLeadingWhite & System.Globalization.NumberStyles.AllowTrailingWhite);
                if (meterSerialNumber != CurrentMeterSession.Meter.SerialNumber)
                {
                    //TODO: que pasaría si no es el medidor esperado??
                }
                //currentMeterSession.MeterModel = tableParser.Model;
                //TODO: comprobar las capacidades del medidor, hasta la fecha no sabemos como determinar si un Alpha3 tiene o no perfil de carga, teóricamente lo tienen todos y además no se puede habilitar/deshabilitar (mmm...)
                //currentMeterSession.LoadProfileEnabled = true;
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error(ex, nameof(OnConfigurationTablesCompleted));
            }

            return result;
        }

        protected virtual bool OnLoadProfileConfigurationCompleted(byte[] data)
        {
            bool result = true;
            try
            {
                ActualLoadProfileTable actualLoadProfileTableParser = new ActualLoadProfileTable();
                actualLoadProfileTableParser.Parse(data);
                ansiProtocolSession.Services.Add(new ReadService(ReadingType.PartialReadWithOffset, new object[] { new Table(TableName.LoadProfileDataSet1Table), actualLoadProfileTableParser.LpMemoryLen, 0 }));
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error(ex, nameof(OnLoadProfileConfigurationCompleted));
            }
            return result;
        }

        #endregion Control Each Read Section

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ansiProtocolSession?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MeterSessionManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
