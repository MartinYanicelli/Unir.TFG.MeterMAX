using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21
{
    public class OnPlaceSession : SessionBase<SerialPort>
    {
        #region Miembros Protegidos
        protected readonly string PortName;
        protected readonly bool? DtrEnabled;
        protected readonly int? BaudRate;
        // cantidad de intentos de cambio de baudrates del puerto COM sin éxito.
        protected int ChangeBaudRateAttempts;
        protected int MaxChangeBaudRateAttempts = 3;

        #region ComPort EventHandlers Declarations
        protected SerialDataReceivedEventHandler serialDataReceivedEventHandler;
        protected SerialErrorReceivedEventHandler serialErrorReceivedEventHandler;
        protected SerialPinChangedEventHandler serialPinChangedEventHandler;
        #endregion

        protected override bool IsPortReady => (Port != null) && Port.IsOpen;

        protected override int DefaultCommunicationTimeout => 6100;

        #endregion

        #region Constructor
        public OnPlaceSession(string portName, int userId, string userName, string password, int? sendAckResponseThershold, int? baudRate, bool? dtrEnabled, int? receivedBytesThershold, NegotiationSetting negotiationSetting = null, TimingSetting timingSetting = null)
            : base(userId, userName, password, sendAckResponseThershold, receivedBytesThershold, negotiationSetting, timingSetting)
        {
            PortName = portName;
            BaudRate = baudRate;
            DtrEnabled = dtrEnabled;

            serialDataReceivedEventHandler = new SerialDataReceivedEventHandler(OnSerialDataReceived);
            serialErrorReceivedEventHandler = new SerialErrorReceivedEventHandler(OnSerialErrorReceived);
            serialPinChangedEventHandler = new SerialPinChangedEventHandler(OnSerialPinChanged);
        }

        public OnPlaceSession(string portName, int userId, string userName, string password, int? baudRate, int? receivedBytesThershold)
            : this(portName, userId, userName, password, null, baudRate, null, receivedBytesThershold)
        {

        }

        public OnPlaceSession(string portName, int userId, string userName, string password, bool? dtrEnable)
            : this(portName, userId, userName, password, null, 9600, dtrEnable, null)
        {
 
        }

        public OnPlaceSession(string portName, int userId, string userName, string password)
            : this(portName, userId, userName, password, null)
        {
 
        }

        #endregion

        #region Public Methods
        public override void Pause()
        {
            if (IsSessionReady)
            {
                Port.DataReceived -= serialDataReceivedEventHandler;
                Port.ErrorReceived -= serialErrorReceivedEventHandler;
                Port.PinChanged -= serialPinChangedEventHandler;

                try
                {
                    // limpio buffer de entrada y salida.
                    Port.DiscardInBuffer();
                    Port.DiscardOutBuffer();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Cleaning In/Out Buffers");
                }
            }
        }
        #endregion

        #region Virtual/Abstract Protected Methods

        protected override void CreatePort()
        {
            Port = new SerialPort(PortName)
            {
                StopBits = StopBits.One,
                Parity = Parity.None,
                Handshake = Handshake.None,
                DataBits = 8
            };
        }

        protected override bool OpenPort()
        {
            return OpenPort(BaudRate, ReceivedBytesThreshold, null);
        }

        protected bool OpenPort(int? baudRate, int? receivedBytesThreshold, int? waitMilliseconds)
        {
            if (!IsPortReady)
            {
                bool? oldDtrEnable = null;
                int? oldReceivedBytesThreshold = null;
                int? oldBaudRate = null;

                try
                {
                    if (DtrEnabled.HasValue)
                    {
                        oldDtrEnable = Port.DtrEnable;
                        Port.DtrEnable = DtrEnabled.Value;
                    }

                    if (baudRate.HasValue)
                    {
                        oldBaudRate = Port.BaudRate;
                        Port.BaudRate = baudRate.Value;
                    }

                    if (receivedBytesThreshold.HasValue)
                    {
                        oldReceivedBytesThreshold = Port.ReceivedBytesThreshold;
                        Port.ReceivedBytesThreshold = receivedBytesThreshold.Value;
                    }

                    Port.Open();

                    if (waitMilliseconds.HasValue)
                        System.Threading.Thread.Sleep(waitMilliseconds.Value);
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                   logger.Warn("startThread Aborted", ex);
                }
                catch (Exception ex)
                {
                    if (oldDtrEnable.HasValue)
                        Port.DtrEnable = oldDtrEnable.Value;
                    if (oldBaudRate.HasValue)
                        Port.BaudRate = oldBaudRate.Value;
                    if (oldReceivedBytesThreshold.HasValue)
                        Port.ReceivedBytesThreshold = oldReceivedBytesThreshold.Value;

                    logger.Error(ex, nameof(OpenPort), new object[] { nameof(Port.PortName), Port.PortName, nameof(Port.BaudRate), Port.BaudRate, nameof(Port.DtrEnable), Port.DtrEnable, nameof(receivedBytesThreshold), receivedBytesThreshold, nameof(CommunicationTimeout), CommunicationTimeout });
                }
            }

            if (IsPortReady)
            {
                Port.DataReceived += serialDataReceivedEventHandler;
                Port.ErrorReceived += serialErrorReceivedEventHandler;
                Port.PinChanged += serialPinChangedEventHandler;
            }

            return IsPortReady;
        }

        protected override bool ClosePort()
        {
            return ClosePort(DefaultWaitMilliseconds);
        }

        protected virtual bool ClosePort(int? waitMilliseconds)
        {
            if (IsPortReady)
            {
                try
                {
                    Port.DataReceived -= serialDataReceivedEventHandler;
                    Port.ErrorReceived -= serialErrorReceivedEventHandler;
                    Port.PinChanged -= serialPinChangedEventHandler;

                    Port.DtrEnable = false;
                    Port.Close();

                    if (waitMilliseconds.HasValue)
                        System.Threading.Thread.Sleep(waitMilliseconds.Value);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, nameof(ClosePort), new object[] { nameof(IsSessionReady), IsSessionReady });
                }
            }

            return !IsPortReady;
        }

        protected bool ChangeBaudRate(int newBaudRate, int? receivedBytesThreshold, int? waitMilliseconds)
        {
            if (Port == null)
                return false;

            var result = false;

            if (ClosePort(waitMilliseconds))
            {
                var oldBaudRate = Port.BaudRate;
                result = OpenPort(newBaudRate, receivedBytesThreshold, null);
            }

            return result;
        }

        protected override void OnResume()
        {
            Port.DataReceived += serialDataReceivedEventHandler;
            Port.ErrorReceived += serialErrorReceivedEventHandler;
            Port.PinChanged += serialPinChangedEventHandler;
        }

        protected override bool OnSendToBuffer(byte[] buffer)
        {
            bool result = true;
            
            try 
	        {	        
		        Port.Write(buffer, 0, buffer.Length);
	        }
	        catch (Exception ex)
	        {
                logger.Error(ex, nameof(OnSendToBuffer));
		        result = false;
	        }
            
            return result;
        }

        protected override void OnBeginHandshake()
        {
            if (IsPortReady)
            {
                if (BaudRate.HasValue && (Port.BaudRate != BaudRate))
                {
                    try
                    {
                        // cambiamos el baudrate sin reabrir puerto!
                        Port.BaudRate = BaudRate.Value;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error changing the baudrate when port is open");
                        // intentamos con método más seguro pero menos performante, cerrando y reabriendo el puerto...
                        ChangeBaudRate(BaudRate.Value, ReceivedBytesThreshold, DefaultWaitMilliseconds);
                    }
                }

                if (ReceivedBytesThreshold.HasValue && (Port.ReceivedBytesThreshold != ReceivedBytesThreshold))
                    Port.ReceivedBytesThreshold = ReceivedBytesThreshold.Value;

                if (DtrEnabled.HasValue && (Port.DtrEnable != DtrEnabled))
                    Port.DtrEnable = DtrEnabled.Value;

                // limpiamos el buffer inmediatamente...
                Port.DiscardInBuffer();

                OnSendToBuffer(new byte[] { 0x55 });
                System.Threading.Thread.Sleep(500);

            }

            base.OnBeginHandshake();
        }

        protected override void CancelServiceExecution()
        {
            base.CancelServiceExecution();
            try
            {
                if (Port != null) Port.DiscardInBuffer();
            }
            catch { }
            
        }

        #endregion

        #region Manejadores de Eventos del Objeto SerialPort
        /* La Clase SerialPort de .NET proporciona E/S sincrónica y orientada a eventos,
         * acceso a los estados de punto de interrupción, así como acceso a las propiedades
         * del controlador serie. 
         * Para una información detallada visitar: http://msdn.microsoft.com/es-es/library/system.io.ports.serialport(VS.80).aspx
        */


        // Este es el núcleo de nuestra sesión. Aquí se desarrolla todo el ciclo de vida
        // de la misma. Se reciben y envían los datos hacia/desde el medidor.
        protected virtual void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Exception dataReceivedException = null;

            byte[] buffer = null;
            
            try
            {
                if (Port.BytesToRead < 1)
                    return;

                // Obtain the number of bytes waiting in the port's buffer
                int buffSize = Port.BytesToRead;

                // Create a byte array buffer to hold the incoming data
                buffer = new byte[buffSize];

                // Read the data from the port and store it in our buffer
                Port.Read(buffer, 0, buffSize);

                OnRawDataReceived(buffer);
            }
            catch (Exception ex)
            {
                dataReceivedException = ex;
            }

            if (dataReceivedException == null)
            {
                if (buffer != null)
                {
                    dataLinkPacket.AddBytes(buffer);
                    while (dataLinkPacket.GetRemainingLength() > 0)
                    {
                        dataLinkPacket.ProcessPacket();
                        if (dataLinkPacket.PacketCompleted)
                        {
                            if (dataLinkPacket.Type == PacketType.Response)
                            {
                                byte[] rawPacket = dataLinkPacket.GetPacket();

                                OnDataReceived(rawPacket);
                            }
                            else
                            {
                                // ACK
                                System.Diagnostics.Debug.WriteLine(string.Format("ACK received {0}", Phase));
                            }
                        }
                    }
                }
            }
            else
            {
                StartCloseSession();
            }
        }

        protected virtual void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            OnSetMessage(string.Format("PinChanged {0}", e.EventType));

            if (e.EventType == SerialPinChange.Break)
            {
                StartCloseSession(true);
            }
        }

        protected virtual void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            OnSetMessage(string.Format("ErrorReceived {0}", e.EventType));

            if (e.EventType == SerialError.Frame)
            {
                StartCloseSession(true);
            }
        }

        #endregion
    }
}
