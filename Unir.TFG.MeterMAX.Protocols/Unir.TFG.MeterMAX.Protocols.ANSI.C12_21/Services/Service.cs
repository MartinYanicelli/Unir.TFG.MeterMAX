using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public abstract class Service : IService
    {
        //protected static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected ServiceRequestCode serviceRequestCode;
        
        protected Packet requestPacket;
        protected Packet responsePacket;

        protected int? lastSequenceNumber;

        public ServiceResponseCode? ResponseCode { get; private set; }
        public bool HasMorePackets { get; private set; }
        public bool IsFirstPacket { get; private set; }
        public bool IsDuplicatePacket { get; private set; }
        public bool IsMultiplePacket { get; private set; }
        public DataFormat DataFormat { get; private set; }
        
        #region Constructor
        public Service()
        {

        }

        protected Service(ServiceRequestCode requestCode)
        {
            serviceRequestCode = requestCode;
        }
        #endregion

        #region Public virtual Methods

        public virtual byte[] SendRequest()
        {
            requestPacket = OnCreateRequestPacket();
            return ConvertPacketToArray(requestPacket);
        }

        //public virtual bool IsRequestOk(byte ackValue)
        //{
        //    return (ackValue == 0x06);
        //}

        public virtual void ProcessResponse(byte[] rawResponse)
        {
            responsePacket = OnCreateResponsePacket(rawResponse);

            if (IsDuplicatePacket)
                return;

            if (!IsMultiplePacket || IsFirstPacket)
            {
                ResponseCode = (ServiceResponseCode)responsePacket.DATA[0];
                if ((ResponseCode == ServiceResponseCode.Ok) && (responsePacket.DATA.Count > 1))
                {
                    OnProcessResponseData();
                }
            }
            else if (responsePacket.DATA.Count > 0)
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

        protected virtual Packet OnCreatePacket()
        {
            return new Packet();
        }

        protected virtual Packet OnCreateRequestPacket()
        {
            Packet packet = OnCreatePacket();
            OnFillRequestDataPacket(packet);
            byte[] dataLen = BitConverter.GetBytes(packet.DATA.Count);
            packet.LENL = dataLen[0]; packet.LENH = dataLen[1];
            lastSequenceNumber = null;
            return packet;
        }

        protected virtual void OnFillRequestDataPacket(Packet requestPacket)
        {
            requestPacket.DATA.Add((byte)serviceRequestCode);
        }

        protected virtual Packet OnCreateResponsePacket(byte[] data)
        {
            // Formato de respuesta.
            // STP IDENT CTRL SEQ_NUMBER LENH LENL DATA CRCH CRCL
            if (data == null)  
                throw new ArgumentNullException("data", "El paquete de datos no puede ser nulo.");

            if (data.Length < 9)
                throw new ServiceException(ServiceExceptionType.PacketLenght, "El paquete de datos debe tener un longitud mínima de 9 bytes.");

            // verificamos que el CRC sea correcto...
            if (!CheckCRC(data))
                throw new ServiceException(ServiceExceptionType.PacketIntegrity, "Error en la integridad del paquete de datos. CRC Inválido.");

            Packet packet = OnCreatePacket();
            
            if (data[0] != packet.STP)
                throw new ServiceException(ServiceExceptionType.PacketIntegrity, string.Format("Error en formato del paquete de datos. Cabecera inválida. {0:X2}", data[0]));
            
            packet.IDENTITY = data[1];
            packet.CTRL = data[2];
            packet.SEQ_NUMBER = data[3];
            packet.LENH = data[4];
            packet.LENL = data[5];

            IsMultiplePacket = packet.CTRL.GetBit(7); // True = Transmisión de múltiples paquetes!!
            IsFirstPacket = packet.CTRL.GetBit(6);
            HasMorePackets = (packet.SEQ_NUMBER > 0);

            if (lastSequenceNumber.HasValue)
            {
                IsDuplicatePacket = lastSequenceNumber == packet.SEQ_NUMBER;
                if (!IsDuplicatePacket)
                    lastSequenceNumber--;
            }
            else
            {
                lastSequenceNumber = packet.SEQ_NUMBER;
            }

            int lengthDATA = BitConverter.ToInt16(new byte[] { packet.LENL, packet.LENH }, 0);
            byte[] buffer = new byte[lengthDATA];
            Buffer.BlockCopy(data, 6, buffer, 0, lengthDATA);

            packet.DATA.AddRange(buffer);

            int crcOffset = data.Length - 2;
            packet.CRCH = data[crcOffset];
            packet.CRCL = data[crcOffset + 1];

            return packet;   
        }

        protected virtual byte[] OnComputeCRC(byte[] data)
        {
            return CRCCalculator.ChecksumToArray(data);
        }

        protected virtual bool CheckCRC(byte[] data)
        {
            var crcH = data[data.Length - 2];
            var crcL = data[data.Length - 1];

            var buffer = new byte[data.Length - 2];
            Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            var crc = CRCCalculator.ChecksumToArray(buffer);

            return (crcL == crc[1]) && (crcH == crc[0]);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ServiceType {0} ", this.GetType());
            stringBuilder.AppendLine();
            stringBuilder.Append("Request Packet: ");
            if (requestPacket != null)
            {
                stringBuilder.Append(requestPacket.ToString());
            }
            else
            {
                stringBuilder.AppendLine(" ---- ");
            }

            stringBuilder.Append("Response Packet: ");
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

        private byte[] ConvertPacketToArray(Packet packet)
        {
            if (!packet.CRCL.HasValue || !packet.CRCH.HasValue)
            {
                var crc = OnComputeCRC(packet.ToArray());
                if (BitConverter.IsLittleEndian) Array.Reverse(crc);
                packet.CRCL = crc[0];
                packet.CRCH = crc[1];
            }
            
            return  packet.ToArray();
        }

        #endregion
    }
}
