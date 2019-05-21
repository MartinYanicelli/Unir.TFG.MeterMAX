using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets
{
    public class RemoteDataLinkPacket
    {
        //<stp><identity><ctrl><seq-nbr><length><data><crc>
        private enum PACKET_STATE { 
            STX, 
            STYPE, 
            ACK, 
            STAT, 
            LEN, 
            DATA, 
            CRCL, 
            CRCH 
        };

        //private enum ProcessState {
        //    Idle,
        //    InProcess
        //}

        private int _dataOffset = 0;
        private PACKET_STATE _packetState;
        //private ProcessState _processState;
        private byte[] _dataLength;
        private int _dataSize;

        private Queue<byte> _recievedData;
        private Queue<byte> _packetData;

        private readonly RemoteServiceType _serviceType;
        private readonly RemoteServiceResponseFormat _serviceResponseFormat;
        
        #region Constructor
        public RemoteDataLinkPacket(RemoteServiceType serviceType, RemoteServiceResponseFormat serviceResponseFormat)
        {
            _serviceType = serviceType;
            _serviceResponseFormat = serviceResponseFormat;

            _dataSize = 0;
            _dataOffset = 0;
            //_processState = ProcessState.Idle;
            _packetState = PACKET_STATE.STX;
            _dataLength = new byte[2];
            _recievedData = new Queue<byte>();
            _packetData = new Queue<byte>();
        }
        #endregion

        public bool PacketCompleted { private set; get; }

        public void AddBytes(byte[] buffer)
        {
            buffer.ToList().ForEach(b => _recievedData.Enqueue(b));
        }

        public void ProcessPacket()
        {
            if (_packetState == PACKET_STATE.STX) // (_processState == ProcessState.Idle)
            {
                _packetData.Clear();

                byte first = _recievedData.Dequeue();

                _packetData.Enqueue(first);

                if (first == 0x02)
                {
                    PacketCompleted = false;
                    //_processState = ProcessState.InProcess;
                    _packetState = PACKET_STATE.STYPE;
                }
            }

            while ((_recievedData.Count > 0) && (_packetState != PACKET_STATE.STX)) //  (_processState == ProcessState.InProcess))
            {
                byte data = _recievedData.Dequeue();
                
                switch (_packetState)
                {
                    //case PACKET_STATE.STX:
                    //    _packetState = PACKET_STATE.STYPE;
                    //    break;
                    case PACKET_STATE.STYPE:
                        //if ((byte)_serviceType != data) // Throw exception!!
                        _packetState = PACKET_STATE.ACK;
                        break;
                    case PACKET_STATE.ACK:
                        _packetState = PACKET_STATE.STAT;
                        break;
                    case PACKET_STATE.STAT:
                        _packetState = (_serviceResponseFormat == RemoteServiceResponseFormat.ResponseWithData) ? PACKET_STATE.LEN : PACKET_STATE.CRCH;
                        break;
                    case PACKET_STATE.LEN:
                        _dataSize = data;
                        _dataOffset = 0;
                        _packetState = PACKET_STATE.DATA;
                        break;
                    case PACKET_STATE.DATA:
                        if (_dataOffset < _dataSize)
                        {
                            _dataOffset++;
                        }
                        if (_dataOffset == _dataSize)
                        {
                            _dataOffset = 0;
                            _packetState = PACKET_STATE.CRCH;
                        }
                        break;
                    case PACKET_STATE.CRCH:
                        _packetState = PACKET_STATE.CRCL;
                        break;
                    case PACKET_STATE.CRCL:
                        PacketCompleted = true;
                        //_processState = ProcessState.Idle;
                        _packetState = PACKET_STATE.STX;
                        break;
                }

                _packetData.Enqueue(data);
                        
            }

        }

        public void Reset()
        {
            _packetData.Clear();
        }
        
        public byte[] GetPacket()
        {
            return _packetData.ToArray();
        }
        
        public int GetRemainingLength()
        {
            return _recievedData.Count;
        }
    }
}
