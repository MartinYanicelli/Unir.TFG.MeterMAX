using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;
using Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services.Enumerations;
using Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21
{
    public class RemoteSession : SessionBase<Socket>
    {
        #region Private Members
        private const int BUFFER_LENGHT = 2048;
        private const int DefaultStartRemoteSessionAttempts = 3;

        private readonly EndPoint _endPoint;
        private readonly int _startRemoteSessionAttempts;

        // flag para indicar si debemos utilizar el protocolo de comunicación propietario para el inicio de sesión remota.
        private readonly bool _userMaxProtocol;

        protected override bool IsPortReady
        {
            get { return Port?.Connected ?? false; }
        }

        protected override int DefaultCommunicationTimeout => 30000; // El Protocolo ANSI 12.21 define 30 segundos como tiempo máximo de espera en el canal de comunicación durante un lectura por su puerto RS232
        #endregion

        #region Constructor
        public RemoteSession(string ip, int portNumber, int userId, string userName, string password, int? sendAckResponseThershold, int? startRemoteSessionAttempts, NegotiationSetting negotiationSetting = null, TimingSetting timingSetting = null,  bool useMaxProtocol = true)
            : base(userId, userName, password, sendAckResponseThershold, null, negotiationSetting, timingSetting)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), portNumber);
            _startRemoteSessionAttempts = startRemoteSessionAttempts ?? DefaultStartRemoteSessionAttempts;
            _userMaxProtocol = useMaxProtocol;
        }

        #endregion

        protected override void CreatePort()
        {
            Port = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //TODO: analizar la posibilidad de utilizar esta técnica de control de timeout en lugar de utilizar la implementada por defecto en la clase base watchDogTimer
            //{
            //    ReceiveTimeout = CommunicationTimeout
            //};
        }

        protected override async Task OpenPortAsync()
        {
            await Task.Run(() => {
                try
                {
                    Port.Connect(_endPoint);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, nameof(OpenPortAsync));

                    //if (IsPortReady)
                    //{
                    //    ClosePort();
                    //    OnSessionClosed("Sesión cancelada por problemas con puerto de comunicación remoto.");
                    //}
                }
            }, cancellationTokenSource.Token);
        }
    

        protected override void ClosePort()
        {
            cancellationTokenSource?.Cancel();

            try
            {
                if (IsPortReady)
                {
                    Port.Shutdown(SocketShutdown.Both);
                    Port.Close(100);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Port.Dispose();
                Port = null;
            }
        }

        protected override bool OnSendToBuffer(byte[] data)
        {
            return Port?.Send(data) == data.Length;
        }

        public override void Pause()
        {
            ClosePort();

            //if (IsSessionReady && (socketDataReceivedThread != null))
            //{
            //    socketDataReceivedThread.Abort("Session Communication Paused by the user!");
            //    socketDataReceivedThread = null;
            //}
        }

        protected override async void OnResume()
        {
            await OpenPortAsync();

            //if (IsSessionReady && (socketDataReceivedThread == null))
            //{
            //    socketDataReceivedThread = new Thread(new ThreadStart(SocketDataReceived));
            //    socketDataReceivedThread.Start();
            //}
        }

        private void SocketDataReceived(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && IsPortReady)
            {
                var buffer = new byte[BUFFER_LENGHT];
                int bytesReceived = Port?.Receive(buffer) ?? 0;
                if (bytesReceived > 0)
                {
                    var data = new byte[bytesReceived];
                    Buffer.BlockCopy(buffer, 0, data, 0, data.Length);

                    OnRawDataReceived(data);
                    dataLinkPacket.AddBytes(data);

                    while ((dataLinkPacket.GetRemainingLength() > 0) && !cancellationToken.IsCancellationRequested)
                    {
                        dataLinkPacket.ProcessPacket();
                        if (dataLinkPacket.PacketCompleted)
                        {
                            if (dataLinkPacket.Type == PacketType.Response)
                            {
                                OnDataReceived(dataLinkPacket.GetPacket());
                            }
                            else if (dataLinkPacket.Type == PacketType.Ack)
                            {
                                // ACK
                                System.Diagnostics.Debug.WriteLine(string.Format("ACK received {0}", Phase));
                            }
                        }
                    }

                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                if (IsPortReady)
                {
                    ClosePort();
                    OnSessionClosed("Sesión remota cancelada por el usuario..");
                }
            }
        }

        protected override void OnSessionStart()
        {
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            if (_userMaxProtocol)
            {
                var maxServiceResponseCode = MaxServiceResponseCode.Unknow;

                try
                {
                    var startRemoteSessionService = new StartRemoteSessionService(CommunicationTimeout);
                    Port.ReceiveTimeout = CommunicationTimeout;
                    var startRemoteSessionCounter = 1;

                    while ((maxServiceResponseCode == MaxServiceResponseCode.Unknow) && (startRemoteSessionCounter <= _startRemoteSessionAttempts) && !cancellationToken.IsCancellationRequested)
                    {
                        SendToBuffer(startRemoteSessionService.SendRequest());

                        try
                        {
                            var maxDataLinkPacket = new MaxProtocol.Packets.MaxDataLinkPacket(MaxServiceType.ShortService, MaxServiceResponseFormat.ResponseWithoutData);
                            var buffer = new byte[BUFFER_LENGHT];
                            int bytesReceived = Port.Receive(buffer);
                            var data = new byte[bytesReceived];
                            Buffer.BlockCopy(buffer, 0, data, 0, data.Length);

                            maxDataLinkPacket.AddBytes(data);
                            while ((maxDataLinkPacket.GetRemainingLength() > 0) && !cancellationToken.IsCancellationRequested)
                            {
                                maxDataLinkPacket.ProcessPacket();
                                if (maxDataLinkPacket.PacketCompleted)
                                {
                                    startRemoteSessionService.ProcessResponse(maxDataLinkPacket.GetPacket());
                                    maxServiceResponseCode = startRemoteSessionService.ResponseCode;
                                }
                            }
                            startRemoteSessionCounter++;
                        }
                        catch (Exception ex)
                        {
                            if ((ex is SocketException socketException) && (socketException.SocketErrorCode == SocketError.TimedOut))
                            {
                                startRemoteSessionCounter++;
                            }
                            else
                            {
                                logger.Error(ex, nameof(OnSessionStart));

                                if (ex is MaxServiceException maxServiceException)
                                {
                                    switch (maxServiceException.Type)
                                    {
                                        case MaxServiceExceptionType.PacketIntegrity:
                                            maxServiceResponseCode = MaxServiceResponseCode.BadCRC;
                                            break;
                                        case MaxServiceExceptionType.PacketLenght:
                                            maxServiceResponseCode = MaxServiceResponseCode.IllegalComandSyntaxLenght;
                                            break;
                                        case MaxServiceExceptionType.ArgumentError:
                                            maxServiceResponseCode = MaxServiceResponseCode.RequestNotSupported;
                                            break;
                                        case MaxServiceExceptionType.FatalError:
                                            maxServiceResponseCode = MaxServiceResponseCode.FatalError;
                                            break;
                                    }
                                }
                                else
                                {
                                    maxServiceResponseCode = MaxServiceResponseCode.FatalError;
                                }
                            }
                        }
                    }

                    if ((startRemoteSessionCounter > _startRemoteSessionAttempts) && (maxServiceResponseCode == MaxServiceResponseCode.Unknow))
                    {
                        maxServiceResponseCode = MaxServiceResponseCode.TimeOutError;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, nameof(OnSessionStart));
                    maxServiceResponseCode = MaxServiceResponseCode.FatalError;
                }
                finally
                {
                    Port.ReceiveTimeout = 0; // dejamos nuevamente el valor por defecto (timeout infinito)
                }

                if (maxServiceResponseCode == MaxServiceResponseCode.Ok)
                {
                    Task.Run(() => SocketDataReceived(cancellationToken), cancellationToken);
                    base.OnSessionStart();
                }
                else
                {
                    ClosePort();
                    OnSessionClosed($"No se puede establecer comunicación remota con el medidor utilizando el protocolo MeterMAX. Código de Error: {maxServiceResponseCode}");
                }
            }
            else
            {
                Task.Run(() => SocketDataReceived(cancellationToken), cancellationToken);
                base.OnSessionStart();
            }
        }
    }
}
