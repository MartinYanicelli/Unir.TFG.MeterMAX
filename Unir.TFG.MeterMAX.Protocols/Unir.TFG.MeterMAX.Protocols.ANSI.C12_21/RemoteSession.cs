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

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21
{
    public class RemoteSession : SessionBase<Socket>
    {
        #region Private Members
        private const int BUFFER_LENGHT = 2048;
        private const int DefaultStartRemoteSessionAttempts = 3;

        private readonly EndPoint _endPoint;
        private readonly int _startRemoteSessionAttempts;

        // Inicia una sesión de comunicación enviando el mensaje StartRemoteSession propietario de Noanet al dispositivo remoto.
        private readonly bool _startCustomSession;

        protected override bool IsPortReady
        {
            get { return (Port != null) && Port.Connected; }
        }

        protected override int DefaultCommunicationTimeout => 30500;
        #endregion


        #region Constructor
        public RemoteSession(string ip, int portNumber, int userId, string userName, string password, int? sendAckResponseThershold, int? startRemoteSessionAttempts, NegotiationSetting negotiationSetting = null, TimingSetting timingSetting = null,  bool startCustomSession = true)
            : base(userId, userName, password, sendAckResponseThershold, null, negotiationSetting, timingSetting)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), portNumber);
            _startRemoteSessionAttempts = startRemoteSessionAttempts ?? DefaultStartRemoteSessionAttempts;
            _startCustomSession = startCustomSession;
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

        protected override bool OpenPort()
        {
            try
            {
                Port.Connect(_endPoint);
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                if (IsPortReady)
                {
                    logger.Error(ex);

                    ClosePort();
                    OnSessionClosed("Sesión cancelada por problemas con puerto de comunicación remoto.");
                }
            }

            return IsPortReady;
        }

        protected override bool ClosePort()
        {
            var result = false;
            try
            {
                if (IsPortReady)
                {
                    Port.Shutdown(SocketShutdown.Both);
                    Port.Close(100);
                    Port.Dispose();
                }
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Port = null;
            }
            return result;
        }

        protected override bool OnSendToBuffer(byte[] data)
        {
            return Port.Send(data) == data.Length;
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

        protected override void OnResume()
        {
            OpenPort();

            //if (IsSessionReady && (socketDataReceivedThread == null))
            //{
            //    socketDataReceivedThread = new Thread(new ThreadStart(SocketDataReceived));
            //    socketDataReceivedThread.Start();
            //}
        }

        protected virtual void SocketDataReceived()
        {
            try
            {
                while (IsPortReady)
                {
                    var buffer = new byte[BUFFER_LENGHT];
                    int bytesReceived = Port.Receive(buffer);
                    var data = new byte[bytesReceived];
                    Buffer.BlockCopy(buffer, 0, data, 0, data.Length);

                    OnRawDataReceived(data);
                    dataLinkPacket.AddBytes(data);

                    while (dataLinkPacket.GetRemainingLength() > 0)
                    {
                        dataLinkPacket.ProcessPacket();
                        if (dataLinkPacket.PacketCompleted)
                        {
                            if (dataLinkPacket.Type == PacketType.Response)
                            {
                                OnDataReceived(dataLinkPacket.GetPacket());
                            }
                            else if (dataLinkPacket.Type == PacketType.ACK)
                            {
                                // ACK
                                System.Diagnostics.Debug.WriteLine(string.Format("ACK received {0}", Phase));
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException abortException)
            {
                logger.Error((string)abortException.ExceptionState);
            }
            catch (Exception ex)
            {
                if (IsPortReady)
                {
                    logger.Error(ex);

                    ClosePort();
                    OnSessionClosed("Sesión cancelada por problemas con puerto de comunicación remoto.");
                }
            }
        }

        protected override void OnSessionStart()
        {
            if (_startCustomSession)
            {
                var remoteServiceResponseCode = RemoteServiceResponseCode.Unknow;

                try
                {
                    var startSessionService = new StartSessionService(CommunicationTimeout);
                    Port.ReceiveTimeout = CommunicationTimeout;
                    var startRemoteSessionCounter = 1;

                    while ((remoteServiceResponseCode == RemoteServiceResponseCode.Unknow) && (startRemoteSessionCounter <= _startRemoteSessionAttempts))
                    {
                        SendToBuffer(startSessionService.SendRequest());

                        try
                        {
                            var remoteDataLinkPacket = new Packets.RemoteDataLinkPacket(RemoteServiceType.ShortService, RemoteServiceResponseFormat.ResponseWithoutData);
                            var buffer = new byte[BUFFER_LENGHT];
                            int bytesReceived = Port.Receive(buffer);
                            var data = new byte[bytesReceived];
                            Buffer.BlockCopy(buffer, 0, data, 0, data.Length);

                            remoteDataLinkPacket.AddBytes(data);
                            while (remoteDataLinkPacket.GetRemainingLength() > 0)
                            {
                                remoteDataLinkPacket.ProcessPacket();
                                if (remoteDataLinkPacket.PacketCompleted)
                                {
                                    startSessionService.ProcessResponse(remoteDataLinkPacket.GetPacket());
                                    remoteServiceResponseCode = startSessionService.ResponseCode;
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

                                if (ex is ServiceException serviceException)
                                {
                                    switch (serviceException.Type)
                                    {
                                        case ServiceExceptionType.PacketIntegrity:
                                            remoteServiceResponseCode = RemoteServiceResponseCode.BadCRC;
                                            break;
                                        case ServiceExceptionType.PacketLenght:
                                            remoteServiceResponseCode = RemoteServiceResponseCode.IllegalComandSyntaxLenght;
                                            break;
                                        case ServiceExceptionType.ArgumentError:
                                            remoteServiceResponseCode = RemoteServiceResponseCode.RequestNotSupported;
                                            break;
                                        case ServiceExceptionType.FatalError:
                                            remoteServiceResponseCode = RemoteServiceResponseCode.FatalError;
                                            break;
                                    }
                                }
                                else
                                {
                                    remoteServiceResponseCode = RemoteServiceResponseCode.FatalError;
                                }
                            }
                        }
                    }

                    if ((startRemoteSessionCounter > _startRemoteSessionAttempts) && (remoteServiceResponseCode == RemoteServiceResponseCode.Unknow))
                    {
                        remoteServiceResponseCode = RemoteServiceResponseCode.TimeOutError;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, nameof(OnSessionStart));
                    remoteServiceResponseCode = RemoteServiceResponseCode.FatalError;
                }
                finally
                {
                    Port.ReceiveTimeout = 0; // dejamos nuevamente el valor por defecto (timeout infinito)
                }

                if (remoteServiceResponseCode == RemoteServiceResponseCode.Ok)
                {
                    var socketDataReceivedThread = new Thread(new ThreadStart(SocketDataReceived));
                    socketDataReceivedThread.Start();
                    base.OnSessionStart();
                }
                else
                {
                    ClosePort();
                    OnSessionClosed($"No se puede establecer comunicación remota con el medidor. Código de Error: {remoteServiceResponseCode}");
                }
            }
            else
            {
                var socketDataReceivedThread = new Thread(new ThreadStart(SocketDataReceived));
                socketDataReceivedThread.Start();
                base.OnSessionStart();
            }
        }
    }
}
