using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21
{
    /*
     * Protocol Details
     * Following the guidelines established by the OSI seven-layer model, the protocol described in this Standard provides three (3) functions:
     *      1) Establishment and modification of the communication channel
     *      2) Transport of information to and from the C12.18 Device
     *      3) Orderly closure of the communication channel when communications are complete
     * 
     * Three (3) layers are used to provide these communication capabilities. These are the Physical, Data Link and Application layers.
     * With the default conditions established by this Standard, the communication channel is considered always available once the physical connection 
     * has been completed. The required Identification Service is used to obtain the protocol version and revision in use by the C12.18 Device. 
     * Certain communication parameters may be modified through the use of the Negotiate Service in the Application Layer. 
     * The Negotiate Service is optional and, if not implemented in the C12.18 Device or not used during actual communications, the communication channel 
     * parameters shall remain at the default settings specified by this Standard. 
     * 
     * Once the C12.18 Device identification and communication parameters have been established, the Application Layer provides 
     * Logon, Security and Logoff services for session activation, access control and deactivation, Read and Write services for issuing data transmission requests, 
     * a Terminate service for shutdown of the communication channel, and a response structure that provides information regarding the success or failure of the service requests. 
     * 
     * An example of a typical communications session would consist of the following services with appropriate responses, in the order listed: 
     *      Identification, 
     *      Negotiate, 
     *      Logon, 
     *      Security, 
     *      Reads or Writes, 
     *      Logoff and Terminate. 
     * */
    public abstract class SessionBase<TPort> : ISession
        where TPort : IDisposable
    {
        #region Miembros Privados
        //Servicio para gestionar el cierre de sesión de comunicación (LogoffService y TerminateService)
        private IService _closeSessionService;
        private Timer _watchdogTimer;
        private Stopwatch _serviceExecutionStopwatch;
        private readonly string _password;
        #endregion

        #region Miembros Protegidos
        protected static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // índice del servicio PSEM (Read or Write) que se encuentra actualmente en ejecución.
        protected int serviceIndex;
        // objeto Port, nuestra interfaz de comunicación para enviar y recibir datos hacia/desde el medidor (SerialPort/Socket/etc)
        protected TPort Port;
        protected abstract bool IsPortReady { get; }
        protected int? ReceivedBytesThreshold;
        protected readonly int DefaultWaitMilliseconds = 300;
        protected readonly int DefaultSendAckResponseThreshold = 100;
        protected int AckResponseThreshold;
        protected Packets.DataLinkPacket dataLinkPacket;
        protected IdentificationService identificationService;
        protected NegotiateService negotiateService;
        protected TimingSetupService timingSetupService;
        protected LogonService logonService;
        protected IService securityOrAuthenticateService;
        protected abstract int DefaultCommunicationTimeout { get; }
        /// <summary>
        /// Communication Timeout in milliseconds
        /// </summary>
        protected int CommunicationTimeout;
        protected CancellationTokenSource cancellationTokenSource;
        #endregion

        #region Events
        public event EventHandler SessionStarting;
        public event EventHandler SessionStarted;
        public event SetMessageEventHandler SessionStartError;
        public event SessionTimeoutEventHandler SessionTimeout;
        public event EventHandler SessionClosing;
        public event SetMessageEventHandler SessionClosed;
        //public event EventHandler SessionStopping;
        //public event SetMessageEventHandler SessionStopped;

        public event ServiceExecutionEventHandler ServiceExecutionStarted;
        public event ServiceExecutionEventHandler ServiceExecutionCompleted;
        public event ServiceExecutionEventHandler ServiceExecutionError;
        public event ServiceExecutionEventHandler ServiceExecutionCanceled;

        public event EventHandler HandshakeInitializing;
        public event EventHandler HandshakeInitialized;
        public event HandshakeCompletedEventHandler HandshakeCompleted;
        public event HandshakeErrorEventHandler HandshakeError;

        public event EventHandler NegotiateServiceError;
        public event EventHandler TimingSetupServiceError;
        public event BaudRateChangedEventHandler BaudRateChanged;

        public event EventHandler ServicesExecutionStarted;
        public event EventHandler ServicesExecutionCompleted;

        public event RawDataEventHandler RawDataSent;
        public event RawDataEventHandler RawDataReceived;
        public event SetMessageEventHandler SetMessage;
        #endregion

        #region Public Constants
        // default Negotiate Service Values
        public const int DefaultNumberOfPackets = 1;
        public const int DefaultPacketSize = 64;

        // default TimingSetup Service Values
        public const int DefaultChannelTrafficTimeout = 30;
        public const int DefaultInterCharacterTimeout = 1;
        public const int DefaultResponseTimeout = 4;
        public const int DefaultRetryAttempts = 3;
        #endregion

        #region Public Properties
        public SessionStatus SessionStatus { get; private set; }

        public SessionPhase Phase { get; protected set; }

        public HandshakeState HandshakeState { get; protected set; }

        public NegotiationSetting NegotiationSetting
        {
            set
            {
                negotiateService = (value != null) ? new NegotiateService(value) : null;
            }
        }

        public TimingSetting TimingSetup
        {
            set
            {
                timingSetupService = (value != null) ? new TimingSetupService(value) : null;
            }
        }
        /// <summary>
        /// Arreglo de Comandos que se ejecutarán automáticamente cuando finalice la fase de negociación (Handshake).
        /// </summary>
        public IList<IService> Services { get; set; }

        public IService CurrentService
        {
            get
            {
                return ((Phase == SessionPhase.ReadAndWriteRequest) && (serviceIndex < Services.Count))
                    ? Services[serviceIndex]
                    : null;
            }
        }

        public bool IsSessionReady { get { return SessionStatus == SessionStatus.Started; } }

        // manejo del patrón dispose..
        public bool IsDisposed { get; protected set; }

        #endregion

        #region Constructor
        protected SessionBase(int userId, string userName, string password, int? sendAckResponseThershold, int? receivedBytesThershold, NegotiationSetting negotiationSetting = null, TimingSetting timingSetting = null)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("El nombre de usuario no puede ser una cadena vacía o nula.");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("La contraseña de autenticación requerida no puede ser una cadena nula o vacía.");

            _password = password;

            CommunicationTimeout = DefaultCommunicationTimeout;
            AckResponseThreshold = sendAckResponseThershold ?? DefaultSendAckResponseThreshold;
            ReceivedBytesThreshold = receivedBytesThershold;

            _serviceExecutionStopwatch = new Stopwatch();
            SessionStatus = SessionStatus.Closed;

            Phase = SessionPhase.Idle;
            HandshakeState = HandshakeState.Identification;
            dataLinkPacket = new DataLinkPacket();

            Services = new List<IService>();
            identificationService = new IdentificationService();
            logonService = new LogonService(userId, userName);

            if (negotiationSetting != null)
            {
                negotiateService = new NegotiateService(negotiationSetting);
            }
            if (timingSetting != null)
            {
                timingSetupService = new TimingSetupService(timingSetting);
            }
        }

        protected SessionBase(int userId, string userName, string password, int? communicationTimeout)
            : this(userId, userName, password, communicationTimeout, null, null) { }

        protected SessionBase(int userId, string userName, string password)
            : this(userId, userName, password, null)
        { }
        #endregion

        #region Public Methods
        public virtual async System.Threading.Tasks.Task StartAsync()
        {
            if ((Services == null) || (Services.Count == 0))
                throw new InvalidOperationException("No hay servicios PSEM disponibles para ejecutar.");

            if (IsSessionReady)
                return;

            cancellationTokenSource = new CancellationTokenSource();

            if (!IsPortReady)
            {
                OnSessionStarting();

                if (Port == null)
                {
                    CreatePort();
                }

                await OpenPortAsync();
            }

            if (IsPortReady)
            {
                OnSessionStarted();
                OnSessionStart();
            }
            else
            {
                OnSessionStartError("Se produjo un Error durante el inicio de sesión remota.");
            }
        }
       
        public async System.Threading.Tasks.Task StartAsync(IList<IService> services)
        {
            Services = services;
            await StartAsync();
        }

        public virtual void Stop()
        {
            if (IsSessionReady)
            {
                StartCloseSession();
            }
            else if (SessionStatus == SessionStatus.Starting)
            {
                OnSetMessage("No puede cancelar la sesión mientras se intenta abrir el puerto de comunicación, por favor aguarde un momento.");
            }
        }

        public abstract void Pause();

        public virtual void Resume()
        {
            if (IsSessionReady)
            {
                BeginHandshake();

                OnResume();
            }
        }

        /// <summary>
        /// Reinicia la execución de los servicios a partir del último servicio ejecutado completo antes del cierre de sesión.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async System.Threading.Tasks.Task RestartAsync()
        {
            if (!IsSessionReady)
            {
                await StartAsync();
            }

            if (IsSessionReady)
            {
                if (serviceIndex >= Services.Count) serviceIndex = 0;
            }
        }
       
        #endregion

        #region Protected Methods

        protected abstract void CreatePort();

        protected void OnSessionStarting()
        {
            SessionStatus = SessionStatus.Starting;
            SessionStarting?.Invoke(this, new EventArgs());
        }

        protected void OnSessionStarted()
        {
            SessionStatus = SessionStatus.Started;
            Phase = SessionPhase.Idle;
            SessionStarted?.Invoke(this, new EventArgs());
        }

        protected void OnSessionStartError(string errorMessage)
        {
            SessionStatus = SessionStatus.Error;
            SessionStartError?.Invoke(this, new SetMessageEventArgs(errorMessage));
        }

        protected void OnSessionTimeout(object ticks)
        {
            _watchdogTimer?.Change(-1, -1);

            lock (this)
            {

                var startTicks = ticks;
                TimeSpan elapsedTime = (startTicks != null) ? TimeSpan.FromTicks(DateTime.Now.Ticks - (long)startTicks) : TimeSpan.Zero;

                SessionTimeout?.Invoke(this, new SessionTimeoutEventArgs(elapsedTime));

                if (Phase != SessionPhase.Terminate) 
                {
                    //OnSessionClosed("Sesión cerrada para restablecer sesión automáticamente.");
                    OnSessionStarted();
                    BeginHandshake();
                }
                else if (IsPortReady)
                {
                    ClosePort();
                    OnSessionClosed($"No se puedo enviar la petición de cierre de sesión a causa de Timeout. Tiempo sin comunicación {elapsedTime.TotalSeconds} segundos");
                }
            }
        }

        protected virtual void OnSessionClosing()
        {
            SessionStatus = SessionStatus.Closing;
            SessionClosing?.Invoke(this, new EventArgs());
        }

        protected void OnSessionClosed(string message)
        {
            SessionStatus = SessionStatus.Closed;
            SessionClosed?.Invoke(this, new SetMessageEventArgs(message));
        }

        protected void OnHandshakeInitializing()
        {
            HandshakeInitializing?.Invoke(this, new EventArgs());
        }

        protected void OnHandshakeInitialized()
        {
            HandshakeInitialized?.Invoke(this, new EventArgs());
        }

        protected void OnHandshakeCompleted(BaudRateEventArgs e)
        {
            HandshakeCompleted?.Invoke(this, e);
        }

        protected void OnHandshakeError(HandshakeState handshakeState, string errorMessage)
        {
            HandshakeError?.Invoke(this, new HandshakeErrorEventArgs(handshakeState, errorMessage));
        }

        protected void OnNegotiateServiceError()
        {
            NegotiateServiceError?.Invoke(this, new EventArgs());
        }

        protected void OnTimingSetupServiceError()
        {
            TimingSetupServiceError?.Invoke(this, new EventArgs());
        }

        protected void OnBaudRateChanged(int newBaudRate)
        {
            BaudRateChanged?.Invoke(this, new BaudRateEventArgs(newBaudRate));
        }

        protected void OnServiceExecutionStarted(IService service)
        {
            _serviceExecutionStopwatch.Start();
            ServiceExecutionStarted?.Invoke(this, new ServiceExecutionEventArgs(service));
        }

        protected void OnServiceExecutionInvoke(IService service, ServiceExecutionEventHandler eventHandler)
        {
            _serviceExecutionStopwatch.Stop();
            eventHandler?.Invoke(this, new ServiceExecutionEventArgs(service, _serviceExecutionStopwatch.Elapsed, (serviceIndex + 1) * 100 / Services.Count));
            _serviceExecutionStopwatch.Reset();
        }
      
        protected void OnServicesExecutionStarted()
        {
            ServicesExecutionStarted?.Invoke(this, new EventArgs());
        }

        protected void OnServicesExecutionCompleted()
        {
            ServicesExecutionCompleted?.Invoke(this, new EventArgs());
        }

        protected void OnRawDataSent(byte[] data)
        {
            RawDataSent?.Invoke(this, new RawDataEventArgs(Phase, data));
        }

        protected void OnRawDataReceived(byte[] data)
        {
            RawDataReceived?.Invoke(this, new RawDataEventArgs(Phase, data));
        }

        protected void OnSetMessage(string message)
        {
            SetMessage?.Invoke(this, new SetMessageEventArgs(message));
        }

        protected abstract System.Threading.Tasks.Task OpenPortAsync();

        protected abstract void ClosePort();

        protected void DisposePort()
        {
            if (IsPortReady)
            {
                ClosePort();
            }

            OnDisposePort();
        }

        protected virtual void OnDisposePort()
        {
            if (Port != null)
            {
                Port.Dispose();
                Port = default(TPort);
            }
        }

        protected bool SendToBuffer(byte[] data)
        {
            var result = false;
            if (IsPortReady)
            {
                try
                {
                    result = OnSendToBuffer(data);
                    if (result)
                    {
                        OnRawDataSent(data);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, nameof(SendToBuffer));
                }
            }
            return result;
        }

        protected abstract bool OnSendToBuffer(byte[] data);

        protected abstract void OnResume();

        protected virtual void OnSessionStart()
        {
            // inicializamos el contador del script
            if (serviceIndex >= Services.Count)
            {
                serviceIndex = 0;
            }

            // inicializamos nuevo watchdog de timeout.
            _watchdogTimer = new Timer(OnSessionTimeout, DateTime.Now.Ticks, -1, -1);

            BeginHandshake();
        }

        protected async void BeginHandshake(bool waitTimeout = false)
        {
            _watchdogTimer.Change(-1, -1);

            if (Phase == SessionPhase.ReadAndWriteRequest)
            {
                CancelServiceExecution();
            }

            if (waitTimeout)
            {
                await System.Threading.Tasks.Task.Delay(4000, cancellationTokenSource.Token); 
            }

            Phase = SessionPhase.Handshake;

            OnBeginHandshake();

            _watchdogTimer.Change(CommunicationTimeout, CommunicationTimeout);

            OnHandshakeInitializing();
        }

        protected virtual void OnBeginHandshake()
        {
            dataLinkPacket = new Packets.DataLinkPacket();
            SendToBuffer(identificationService.SendRequest());
            HandshakeState = HandshakeState.Identification;
        }
        
        protected virtual byte[] ExecuteCurrentService()
        {
            if ((Services == null) || (Services.Count == 0))
                throw new InvalidOperationException("La lista de servicios PSEM no puede estar nula o vacía.");

            byte[] request = null;

            try
            {
                if (serviceIndex < Services.Count)
                {
                    if (serviceIndex == 0)
                        OnServicesExecutionStarted();

                    request = CurrentService.SendRequest();

                    OnServiceExecutionStarted(CurrentService);
                }
                else
                {
                    OnServicesExecutionCompleted();
                    StartCloseSession();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, nameof(ExecuteCurrentService), new object[] { nameof(serviceIndex), serviceIndex });
            }

            return request;
        }

        protected virtual byte[] ExecuteNextService()
        {
            serviceIndex++;
            return ExecuteCurrentService();
        }

        protected virtual void CancelServiceExecution()
        {
            IService service = CurrentService;

            if (service == null)
                return;

            service.Reset();
            dataLinkPacket.Reset();

            OnServiceExecutionInvoke(service, ServiceExecutionCanceled);
        }

        protected virtual void StartCloseSession(bool restartSession = false)
        {
            if (Phase == SessionPhase.Idle) // acaba de iniciar la sesión y todavía no se ejecutó el primer BeginHandshake
                return;

            _watchdogTimer.Change(-1, -1); // desactivamos el temporizador de vigilancia de timeout

            if ((Phase == SessionPhase.ReadAndWriteRequest) && (serviceIndex < Services.Count))
            {
                CancelServiceExecution();
            }

            Phase = restartSession ? SessionPhase.TerminateAndRestart : SessionPhase.Terminate;

            /*
             * Logoff Service requests are accepted in the Session State only. Acceptance of a Logoff Service 
             * request transitions communications to the ID State. This service can originate from either end of the communication channel.
             * 
             * Terminate Service requests are accepted in all states. Acceptance of a Terminate Service request transitions communications to the Base State. 
             * This service can originate from either end of the communication channel.
             * 
             * Disconnect Service requests are accepted in all states. Acceptance of a Disconnect Service request disconnects the communication channel. 
             * This service can originate from either end of the communication channel.
             * 
             * */
            _closeSessionService = ((HandshakeState == HandshakeState.Security)
                || (HandshakeState == HandshakeState.Authenticate)
                || (HandshakeState == HandshakeState.Completed)) ? new LogoffService() : (identificationService.AnsiProtocol == AnsiProtocol.ANSI_C12_18) ? new TerminateService() : (IService)new DisconnectService();

            bool requestSended = SendToBuffer(_closeSessionService.SendRequest());
            OnSessionClosing();
            if (requestSended)
            {
                _watchdogTimer.Change(CommunicationTimeout, CommunicationTimeout); // reactivamos el temporizador de vigilancia del timeout.
            }
            else
            {
                ClosePort();
                OnSessionClosed("Error al enviar la petición de cirre de comunicación al medidor.");
            }
        }

        protected virtual byte[] OnHandshakePhase(byte[] data)
        {
            byte[] request = null;
            string handshakeErrorMessage = null;
            //bool handshakeExceptionOcurred = false;

            try
            {
                /* Fase de handshake, esto es: 
                    *  1. Respuesta de Identificación.
                    *  2. Solicitud y Respuesta de Negociación
                    *  3. Solicitud y Respuesta de Logon
                    *  4. Solicitud y Respuesta de Seguridad { En este punto setear Phase = SessionPhase.ReadAndWriteRequest }
                    *  Caso de error: 
                    *  Lanzar OnHandshakeError(message) con mensaje descriptivo e iniciar nuevamente el proceso de Handshake con BeginHandshake()
                * */
                switch (HandshakeState)
                {
                    case HandshakeState.Identification:
                        /*
                         *   Response:
                         *          <ident-r> ::=   <isss> | <bsy> | <err> | 
                         *                          <ok><std><ver><rev><feature>*<end-of-list>
                         * */
                        identificationService.ProcessResponse(data);
                        if (identificationService.ResponseCode.Value == ServiceResponseCode.Ok)
                        {
                            OnHandshakeInitialized();
                            if (negotiateService != null)
                            {
                                request = negotiateService.SendRequest();
                                HandshakeState = HandshakeState.Negotiation;
                            }
                            else
                            {
                                if ((identificationService.AnsiProtocol == AnsiProtocol.ANSI_C12_18) || (timingSetupService == null))
                                {
                                    request = logonService.SendRequest();
                                    HandshakeState = HandshakeState.Logon;
                                }
                                else
                                {
                                    request = timingSetupService.SendRequest();
                                    HandshakeState = HandshakeState.TimingSetup;
                                }
                            }
                        }
                        else if (identificationService.ResponseCode.Value == ServiceResponseCode.DeviceBusy)
                        {
                            request = identificationService.SendRequest();
                            Thread.Sleep(500);
                        }
                        else if (identificationService.ResponseCode.Value == ServiceResponseCode.InvalidServiceSequenceState)
                        {
                            StartCloseSession(true);
                        }
                        else
                        {
                            handshakeErrorMessage = $"El Medidor rechazó la comunicación. Identificación inválida. ResponseCode: {identificationService.ResponseCode}";
                        }
                        break;
                    case HandshakeState.Negotiation:
                        /*
                         *  Response:
                         *      The responses <sns>, <isss>, <bsy> and <err> indicate a problem with the received Negotiate Service request and that the communication channel will maintain its current settings.
                         *      The response <ok> indicates the Negotiate Service request was accepted and all new settings now apply to both devices. 
                         *      The data following the <ok> indicates the new settings. If the target cannot accept the Negotiate Service request baud rates, the original baud rate will be echoed in the response.
                         *          <negotiate-r> ::=   <sns> | <isss> | <bsy> | <err> | 
                         *                              <ok><packet-size><nbr-packets><baud-rate>
                         * */
                        negotiateService.ProcessResponse(data);
                        if (negotiateService.ResponseCode.Value == ServiceResponseCode.DeviceBusy)
                        {
                            request = negotiateService.SendRequest();
                            Thread.Sleep(500);
                        }
                        else if (negotiateService.ResponseCode.Value == ServiceResponseCode.InvalidServiceSequenceState)
                        {
                            StartCloseSession(true);
                        }
                        else
                        {
                            // se produzca o no un error continuar con el siguiente estado
                            if ((identificationService.AnsiProtocol == AnsiProtocol.ANSI_C12_18) || (timingSetupService == null))
                            {
                                request = logonService.SendRequest();
                                HandshakeState = HandshakeState.Logon;
                            }
                            else
                            {
                                request = timingSetupService.SendRequest();
                                HandshakeState = HandshakeState.TimingSetup;
                            }

                            if (negotiateService.ResponseCode.Value != ServiceResponseCode.Ok)
                                OnNegotiateServiceError();
                        }
                        break;
                    case HandshakeState.TimingSetup:
                        timingSetupService.ProcessResponse(data);
                        if (timingSetupService.ResponseCode.Value == ServiceResponseCode.DeviceBusy)
                        {
                            request = timingSetupService.SendRequest();
                            Thread.Sleep(500);
                        }
                        else if (timingSetupService.ResponseCode.Value == ServiceResponseCode.InvalidServiceSequenceState)
                        {
                            StartCloseSession(true);
                        }
                        else
                        {
                            // se produzca o no un error continuar con el siguiente estado
                            request = logonService.SendRequest();
                            HandshakeState = HandshakeState.Logon;

                            if (timingSetupService.ResponseCode.Value == ServiceResponseCode.Ok)
                            {
                                // actualizamos el timeout de comunicación que está por defecto al nuevo valor negociado.
                                CommunicationTimeout = (int)TimeSpan.FromSeconds(timingSetupService.Setting.ChannelTrafficTimeout).TotalMilliseconds;
                            }
                            else
                            {
                                OnTimingSetupServiceError();
                            }
                             
                        }
                        break;
                    case HandshakeState.Logon:
                        /*
                         * Response:
                         *      The responses <isss>, <iar>, <bsy> and <err> indicate a problem with the received Logon Service request.
                         *      The response <ok> indicates the Logon Service was successfully completed and the session was established.
                         *          
                         *          <logon-r> ::= <isss> | <iar> | <bsy> | <err> | <ok>
                         * */
                        logonService.ProcessResponse(data);
                        if (logonService.ResponseCode.Value == ServiceResponseCode.Ok)
                        {
                            if (identificationService.AuthenticationType == null)
                            {
                                securityOrAuthenticateService = new SecurityService(_password);
                                request = securityOrAuthenticateService.SendRequest();
                                HandshakeState = HandshakeState.Security;
                            }
                            else
                            {
                                securityOrAuthenticateService = new AuthenticateService(_password, identificationService.AuthenticationType);
                                request = securityOrAuthenticateService.SendRequest();
                                HandshakeState = HandshakeState.Authenticate;
                            }
                        }
                        else if (logonService.ResponseCode.Value == ServiceResponseCode.DeviceBusy)
                        {
                            request = logonService.SendRequest();
                            Thread.Sleep(500);
                        }
                        else if (logonService.ResponseCode.Value == ServiceResponseCode.InvalidServiceSequenceState)
                        {
                            StartCloseSession(true);
                        }
                        else
                        {
                            handshakeErrorMessage = $"El Medidor rechazó la comunicación. Usuario inválido. ResponseCode: {logonService.ResponseCode}";
                        }
                        break;
                    case HandshakeState.Security:
                    case HandshakeState.Authenticate:
                        /*
                         * Security Response:
                         *      The responses <sns>, <isss>, <bsy> and <err> indicate a problem with the received Security Service request.
                         *      The response <ok> indicates the Security Service was successfully completed and the access permissions associated with the password were granted.
                         *          
                         *          <security-r> ::= <sns> | <isss> | <bsy> | <err> | <ok>
                         * */
                        /*
                         * Authenticate Response:
                         *          The responses <sns>, <isss>, <bsy>, and <err> indicate a problem with the received Authenticate Service request.
                         *          The response <isc> indicates the authentication failure of the requester. 
                         *          The response <ok> indicates the Authenticate Service was successfully completed and the access permission associated with the <auth-request> field was granted.
                         *          
                         *          <authenticate-r> ::= <sns> | <isss> | <isc> | <bsy> | <err> | <ok><auth-res-length><auth-response>
                         *                  <auth-res-length> ::= <byte> {Number of bytes of the <authresponse> field.}
                         *                  <auth-response> ::= <byte>+ {Information used to authenticate the recipient of this service.}
                         *          
                         * */
                        securityOrAuthenticateService.ProcessResponse(data);
                        if (securityOrAuthenticateService.ResponseCode.Value == ServiceResponseCode.Ok)
                        {
                            securityOrAuthenticateService = null;
                            HandshakeState = HandshakeState.Completed;
                            Phase = SessionPhase.ReadAndWriteRequest;
                            OnHandshakeCompleted(new BaudRateEventArgs((negotiateService.Setting.BaudRate != BaudRateValue.ExternallyDefined) ? int.Parse(negotiateService.Setting.BaudRate.ToString().Split('_')[1]) : 0));
                            request = ExecuteCurrentService();
                        }
                        else if (securityOrAuthenticateService.ResponseCode.Value == ServiceResponseCode.DeviceBusy)
                        {
                            request = securityOrAuthenticateService.SendRequest();
                            Thread.Sleep(500);
                        }
                        else if (securityOrAuthenticateService.ResponseCode.Value == ServiceResponseCode.InvalidServiceSequenceState)
                        {
                            StartCloseSession(true);
                        }
                        else
                        {
                            handshakeErrorMessage = $"El Medidor rechazó la comunicación. Autenticación inválida. ResponseCode: {securityOrAuthenticateService.ResponseCode}";
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                handshakeErrorMessage = (ex is ServiceException)
                    ? $"Se produjo un error durante la negociación de comunicación con el medidor. Tipo de Error: {((ServiceException)ex).Type.ToString()} - Detalle: {ex.Message}"
                    : $"Se produjo un error inesperado durante la negociación de comunicación con el medidor. Detalle {ex.Message}";

                //handshakeExceptionOcurred = true;
            }


            if (handshakeErrorMessage != null)
            {
                OnHandshakeError(HandshakeState, handshakeErrorMessage);
                //if (handshakeExceptionOcurred)
                //{
                //    BeginHandshake();
                //}
                //else
                //{
                //    StartCloseSession();
                //}
                StartCloseSession();
            }

            return request;
        }

        /// <summary>
        /// Método para propósitos de Test Automatizado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] HandshakePhaseTest(byte[] data)
        {
            return OnHandshakePhase(data);
        }

        protected virtual byte[] OnReadAndWriteRequestPhase(byte[] data)
        {
            byte[] request = null;

            IService service = CurrentService;

            try
            {
                service.ProcessResponse(data);
                if (service is ReadService readService)
                {
                    request = OnProcessReadService(readService, data);
                }
                else if (service is WriteService writeService)
                {
                    request = OnProcessWriteService(writeService, data);
                }
                else
                {
                    // Otro tipo de servicio?? Se puede dar?
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, nameof(OnReadAndWriteRequestPhase));
                OnServiceExecutionInvoke(service, ServiceExecutionError);
                StartCloseSession(true);
            }

            return request;
        }

        protected virtual byte[] OnTerminatePhase(byte[] data)
        {
            byte[] request = null;

            try
            {
                _closeSessionService.ProcessResponse(data);

                if (_closeSessionService.ResponseCode.Value == ServiceResponseCode.Ok)
                {
                    if (_closeSessionService is LogoffService)
                    {
                        _closeSessionService = (identificationService.AnsiProtocol == AnsiProtocol.ANSI_C12_18) ? new TerminateService() : (IService)new DisconnectService();
                        request = _closeSessionService.SendRequest();
                    }
                    else
                    {
                        if (Phase == SessionPhase.Terminate)
                        {
                            ClosePort();
                            OnSessionClosed("Finalización de comunicación exitosa con el medidor");
                        }
                        else if (Phase == SessionPhase.TerminateAndRestart)
                        {
                            //OnSessionClosed("Sesión cerrada para restablecer sesión automáticamente.");
                            System.Threading.Thread.Sleep(DefaultChannelTrafficTimeout);
                            OnSessionStarted();
                            BeginHandshake();
                        }
                    }
                }
                else
                {
                    ClosePort();
                    OnSessionClosed(string.Format("No se pudo realizar el cierre explícito de comunicación con el medidor. Servicio: {0} - Código de Respuesta: {1}",
                        _closeSessionService.GetType().Name, _closeSessionService.ResponseCode));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, nameof(OnTerminatePhase));
                ClosePort();
                OnSessionClosed("Se produjo un error inesperado durante el proceso de cierre de comunicación.");
            }

            return request;
        }

        /*
         * Response:
         *      Responses of type <nok> indicate a problem with the received Read Service request.
         *      The response <ok> indicates the Read Service was accepted and the data is part of the response.
         *          <read-r> ::= <nok> | <ok><table-data>
         * 
         * */
        protected virtual byte[] OnProcessReadService(ReadService service, byte[] data)
        {
            byte[] request = null;
            // verificar si el paquete recibido tiene un ACK
            if (service.ResponseCode.Value == ServiceResponseCode.Ok)
            {
                // Verificar si necesita mas paquetes
                if (!service.HasMorePackets)
                {
                    OnServiceExecutionInvoke(service, ServiceExecutionCompleted);
                    request = ExecuteNextService();
                }
                else if (service.IsDuplicatePacket)
                {
                    request = new byte[] { 0x06 }; // ACK
                }
            }
            else
            {
                OnServiceExecutionInvoke(service, ServiceExecutionError);
                StartCloseSession(true);
            }

            return request;
        }

        /*
         * Response:
         *          Responses of type <nok> indicate a problem with the received Write Service request. 
         *          The response <ok> indicates the Write Service was successfully completed and the data was successfully transmitted to the requesting device.
         *          
         *          <write-r> ::= <nok> | <ok>
         * */
        protected virtual byte[] OnProcessWriteService(WriteService service, byte[] data)
        {
            byte[] request = null;


            return request;
        }
        /// <summary>
        /// Método para propósitos de Test Automatizado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] ServicesExecutionPhaseTest(byte[] data)
        {
            return OnReadAndWriteRequestPhase(data);
        }

        protected void OnDataReceived(byte[] buffer)
        {
            _watchdogTimer?.Change(-1, -1); // stop the timer

            Thread.Sleep(AckResponseThreshold);
            SendToBuffer(new byte[] { 0x06 }); // ACK Service Response IN

            byte[] request = null;

            switch (Phase)
            {
                case SessionPhase.Handshake:
                    /* Estamos en la fase de negociación de transmisión
                     * ya sea por inicio de comunicación o por reinicio por tiempo agotado o error de transmisión.
                     * 
                     * Solicitud de Servicios requeridos:
                     * Identificación
                     * Negociación (Opcional)
                     * Logon
                     * Seguridad (Opcional)
                     * */
                    request = OnHandshakePhase(buffer);
                    break;
                case SessionPhase.ReadAndWriteRequest:
                    /* Fase de transferencia de datos. Se ejecutan los servicios de lectura y escritura deseados. */
                    request = OnReadAndWriteRequestPhase(buffer);
                    break;

                case SessionPhase.Terminate:
                case SessionPhase.TerminateAndRestart:
                    /* Fase de cierre de sesión y canal de comunicación.
                     * Estamos en este punto ya sea porque se ejecutaron todos los servicios de Lectura/Escritura 
                     * o porque el usuario canceló explícitamente la comunicación.
                     * En esta fase se ejecutan los servicios:
                     *      Logoff
                     *      Terminate
                     * */
                    request = OnTerminatePhase(buffer);
                    break;
            }

            if (request != null)
            {
                SendToBuffer(request);
            }

            //RawDataReceived?.Invoke(this, new RawDataEventArgs() { Data = buffer });

            //SetMessage?.Invoke(this, new SetMessageEventArgs() { Message = string.Format("Phase: {0}", Phase) });

            if (IsPortReady) // if (IsSessionReady)
            {
                // reset the timer again..
                _watchdogTimer?.Change(CommunicationTimeout, CommunicationTimeout);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    lock (this)
                    {
                        cancellationTokenSource?.Dispose();
                        _watchdogTimer?.Dispose();
                        _watchdogTimer = null;
                    }

                    if (Phase == SessionPhase.ReadAndWriteRequest)
                    {
                        CancelServiceExecution();
                    }

                    DisposePort();
                }

                IsDisposed = true;
            }
        }

        #endregion IDisposable Members
    }
}
