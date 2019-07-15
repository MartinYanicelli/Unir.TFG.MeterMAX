using Unir.TFG.MeterMAX.Protocols.MaxProtocol.Packets;
using Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

/*
 * Protocol Field Definitions
 * The list below defines the protocol fields, or bytes, that comprise a message.
 * 
 * STX 
 * Start of text character ($02); binary format. Must be the first byte of every message.
 * 
 * SB 
 * Service byte; binary format. 
 *      There are two general remote service types, short format and long format, each of which has a different 
 *      interpretation for the service code byte.
 * 
 *      Short format bit map: f 0 0 c c c c c 
 *          f = format type; 1 indicates message type is short format.
 *          c = command code.
 *      Long format bit map: f 0 0 rw fu rd 0 cl
 *          f = format type; 0 indicates message type is long format.
 *          rw = read/write flag; 0 indicates read command.
 *          fu = function flag; 1 indicates function command.
 *          rd = read flag; 1 indicates read command.
 *          cl = class request flag; 1 indicates class request command.
 * 
 * SC 
 * Service code; binary format. 
 * 
 * LEN 
 * Byte length of data field; binary format. This single byte length field is used in function 
 * with data and data response.
 * It is limited to a range of 1-64. It defines the length of the data field that is included in the message.
 * 
 *      Bit map: ls l l l l l l l
 *          ls = last data set flag; 1 when modem response contains last dataset.
 *          l = length; range 1-64. Modem Response can involve more than 64 bytes of data. 
 *                                  In this case, the modem transmits the data 64 bytes at a time. 
 *                                  When the modem responds to request, 
 *                                  the most significant bit of LEN is set to indicate 
 *                                  that the response contains the last dataset.
 *                                  
 * 
 * DATA 
 * Data field (1-64 bytes); format is data dependent. 
 * The data field is limited to 64 bytes.
 * 
 * ACK/NAK 
 * Acknowledge / Negative acknowledge byte. The ACK/NAK byte is transmitted to indicate 
 * the validity of last message received. An ACK (0) means the last message was valid. 
 * A NAK (any value other than 0) indicates an error. The value of an ACK/NAK byte transmitted 
 * by the modem can be checked to determine the type of error:
 *      ACK/NAK Description 
 *          0x00    ACK: No error 
 *          0x01    NAK: Bad CRC
 *          0x03    NAK: Illegal command, syntax, or length
 *          0x04    NAK: Framing error
 *          0x05    NAK: Timeout error
 *          0x06    NAK: Invalid password
 *          0x07    NAK: NAK received from computer
 *          0x0B    NAK: FunctionIsNotAllowed
 *          0x0C    NAK: RequestInProcess,
 *          0x0D    NAK: TooBusyToHonor,
 *          0x0F    NAK: RequestNotSupported,
 *          0x40    NAK: MeterUnreachable,
 *          0x41    NAK: ModemHardwareFailure,
 *          0xFF    NAK: Unknow
 *          
 * STAT 
 * Status code; binary format. This one byte field indicates events that have occurred. 
 *          
 * CRCH CRCL 
 * Cyclic redundancy check (CRC) code. The CRC code is a 2 byte value appended to the end of every 
 * message packet and is used to help detect communication error. It is calculated for all 
 * bytes of the message packet by a 16 bit polynomial equation
 * 
 * **/
namespace Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services
{
    public abstract class MaxService
    {
        protected MaxServiceRequestCode serviceRequestCode;
        protected MaxServiceType serviceType;
        protected MaxPacket requestPacket;
        protected MaxPacket responsePacket;

        public MaxServiceResponseCode ResponseCode => (responsePacket != null) ? (MaxServiceResponseCode)responsePacket.ACK : MaxServiceResponseCode.Unknow;
        public bool HasMorePackets { get; private set; }
        public bool IsFirstPacket { get; private set; }
        
        #region Constructor
        public MaxService()
        {

        }

        protected MaxService(MaxServiceType serviceType, MaxServiceRequestCode requestCode)
        {
            this.serviceType = serviceType;
            serviceRequestCode = requestCode;
        }
        #endregion

        #region Public virtual Methods

        public virtual byte[] SendRequest()
        {
            requestPacket = OnCreateRequestPacket();
            return ConvertRequestPacketToArray(requestPacket);
        }

        //public virtual bool IsRequestOk(byte ackValue)
        //{
        //    return (ackValue == 0x06);
        //}

        public virtual void ProcessResponse(byte[] rawResponse)
        {
            responsePacket = OnCreateResponsePacket(rawResponse);

            if (responsePacket.DATA.Count > 0)
            {
                OnProcessResponseData();
            }
        }

        public virtual void Reset()
        {
            if (requestPacket != null)
            {
                requestPacket = null;
            }

            if (responsePacket != null)
            {
                responsePacket = null;
            }
        }

        protected virtual MaxPacket OnCreatePacket()
        {
            return new MaxPacket() { STYPE = (byte)serviceType };
        }
  
        protected virtual MaxPacket OnCreateRequestPacket()
        {
            MaxPacket packet = OnCreatePacket();
            packet.SCODE = (byte)serviceRequestCode;
            packet.PAD = 0x00;
            OnFillRequestDataPacket(packet);
            if (packet.DATA.Count > 0)
            {
                packet.LEN = (byte)packet.DATA.Count;
            }
            return packet;
        }

        protected virtual void OnFillRequestDataPacket(MaxPacket requestPacket)
        {
            
        }

        protected virtual MaxPacket OnCreateResponsePacket(byte[] data)
        {
            /* Formato de respuesta:
             * Sin datos:
             *  STX STYPE ACK|NACK STAT CRCH CRCL
             * 
             * Con datos:
             *  STX STYPE ACK|NACK STAT LEN DATA CRCH CRCL
             * 
             * */

            if (data == null)
                throw new ArgumentNullException("data", "El paquete de datos MeterMAX no puede ser nulo.");

            if (data.Length < 6)
                throw new MaxServiceException(MaxServiceExceptionType.PacketLenght, "El paquete de datos MeterMAX debe tener un longitud mínima de 6 bytes.");

            // verificamos que el CRC sea correcto...
            if (!CheckCRC(data))
                throw new MaxServiceException(MaxServiceExceptionType.PacketIntegrity, "Error en la integridad del paquete de datos MeterMAX. CRC Inválido.");

            MaxPacket packet = OnCreatePacket();

            if (data[0] != packet.STX)
                throw new MaxServiceException(MaxServiceExceptionType.PacketIntegrity, $"Error en formato del paquete de datos MeterMAX. Cabecera inválida. {data[0]:X2}");

            if (!Enum.IsDefined(typeof(MaxServiceType), data[1]))
                throw new MaxServiceException(MaxServiceExceptionType.PacketIntegrity, $"Error en el formato del paquete de datos MeterMax. Tipo de Servicio inválido. {data[1]:X2}");

            if (!Enum.IsDefined(typeof(MaxServiceResponseCode), data[2]))
                throw new MaxServiceException(MaxServiceExceptionType.PacketIntegrity, $"Error en el formato del paquete de datos MeterMAX. Código de Respuesta inválido. {data[2]:X2}");

            packet.STYPE = data[1];
            packet.ACK = data[2];
            packet.STAT = data[3];

            if (data.Length == 6)
            {
                packet.CRCH = data[4];
                packet.CRCL = data[5];
            }
            else
            {
                byte len = (byte) (7 + data[4]);
                if (data.Length != len)
                    throw new MaxServiceException(MaxServiceExceptionType.PacketLenght, $"Error en la longitud del paquete de datos MeterMAX. Longitud del paquete: {data.Length} - Longitud esperada: {len}");

                packet.LEN = len;
                byte[] buffer = new byte[len];
                Buffer.BlockCopy(data, 5, buffer, 0, len);
                packet.DATA.AddRange(buffer);
                packet.CRCH = data[data.Length - 2];
                packet.CRCL = data[data.Length - 1];
            }
            
            return packet;
        }

        protected virtual byte[] OnComputeCRC(byte[] data)
        {
            return MaxCRCCalculator.ChecksumToArray(data);
        }

        protected virtual bool CheckCRC(byte[] data)
        {
            var crcH = data[data.Length - 2];
            var crcL = data[data.Length - 1];

            var buffer = new byte[data.Length - 2];
            Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            var crc = MaxCRCCalculator.ChecksumToArray(buffer);

            return (crcL == crc[1]) && (crcH == crc[0]);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("MaxServiceType {0} ", this.GetType());
            stringBuilder.AppendLine();
            stringBuilder.Append("Request MeterMAX Packet: ");
            if (requestPacket != null)
            {
                stringBuilder.Append(requestPacket.ToString());
            }
            else
            {
                stringBuilder.AppendLine(" ---- ");
            }

            stringBuilder.Append("Response MeterMAX Packet: ");
            if (responsePacket != null)
            {
                stringBuilder.Append(responsePacket.ToString());
            }
            else
            {
                stringBuilder.AppendLine(" ---- ");
            }

            return stringBuilder.ToString();
        }
        #endregion

        #region Abstract Methods
        protected abstract void OnProcessResponseData();
        #endregion

        #region Private Methods

        private byte[] ConvertRequestPacketToArray(MaxPacket packet)
        {
            int packetSize = packet.LEN.HasValue ? (7 + packet.LEN.Value) : 6;
            List<byte> rawPacket = new List<byte>();

            rawPacket.AddRange(new byte[] { packet.STX, packet.STYPE, packet.SCODE.Value, packet.PAD.Value });
            if (packet.LEN.HasValue)
            {
                rawPacket.Add(packet.LEN.Value);
                rawPacket.AddRange(packet.DATA);
            }

            if (!packet.CRCL.HasValue || !packet.CRCH.HasValue)
            {
                var crc = OnComputeCRC(rawPacket.ToArray());
                if (BitConverter.IsLittleEndian) Array.Reverse(crc);
                packet.CRCL = crc[0]; 
                packet.CRCH = crc[1]; 
            }

            rawPacket.Add(packet.CRCH.Value);
            rawPacket.Add(packet.CRCL.Value);

            return rawPacket.ToArray();
        }

        #endregion
    }
}
