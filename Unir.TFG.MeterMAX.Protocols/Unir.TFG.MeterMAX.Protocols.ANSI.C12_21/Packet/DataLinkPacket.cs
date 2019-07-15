using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets
{
    public class DataLinkPacket
    {
        //<stp><identity><ctrl><seq-nbr><length><data><crc>
        private enum PACKET_STATE { 
            STP, 
            IDENTITY, 
            CTRL, 
            SEQ, 
            LENL, 
            LENH, 
            DATA, 
            CRCL, 
            CRCH 
        };

        private enum ProcessState {
            Idle,
            InProcess
        }

        private int _dataOffset = 0;
        private PACKET_STATE _packetState;
        private ProcessState _processState;
        private byte[] _dataLength;
        private int _dataSize;

        private Queue<byte> _recievedData;
        private Queue<byte> _packetData;

        #region Constructor
        public DataLinkPacket()
        {
            _dataSize = 0;
            _dataOffset = 0;
            _processState = ProcessState.Idle;
            _dataLength = new byte[2];
            _recievedData = new Queue<byte>();
            _packetData = new Queue<byte>();
        }
        #endregion

        public PacketType Type { private set; get; }
        public bool PacketCompleted { private set; get; }

        public void AddBytes(byte[] buffer)
        {
            buffer.ToList().ForEach(b => _recievedData.Enqueue(b));
        }

        public void ProcessPacket()
        {
            if (_processState == ProcessState.Idle)
            {
                _packetData.Clear();

                byte first = _recievedData.Dequeue();

                _packetData.Enqueue(first);

                if (first == 0x06) // ACK Response
                {
                    Type = PacketType.Ack;
                    PacketCompleted = true;
                }
                else if (first == 0xEE)
                {
                    Type = PacketType.Response;
                    PacketCompleted = false;
                    _processState = ProcessState.InProcess;
                    _packetState = PACKET_STATE.STP;
                }
            }

            while ((_recievedData.Count > 0) && (_processState == ProcessState.InProcess))
            {
                byte data = _recievedData.Dequeue();
                
                switch (_packetState)
                {
                    case PACKET_STATE.STP:
                        _packetState = PACKET_STATE.IDENTITY;
                        break;
                    case PACKET_STATE.IDENTITY:
                        _packetState = PACKET_STATE.CTRL;
                        break;
                    case PACKET_STATE.CTRL:
                        _packetState = PACKET_STATE.SEQ;
                        break;
                    case PACKET_STATE.SEQ:
                        _dataLength[0] = data;
                        _packetState = PACKET_STATE.LENL;
                        break;
                    case PACKET_STATE.LENL:
                        _dataLength[1] = data;
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(_dataLength);
                        _dataSize = BitConverter.ToInt16(_dataLength, 0);
                        _packetState = PACKET_STATE.LENH;
                        break;
                    case PACKET_STATE.LENH:
                        if (_dataOffset < _dataSize)
                        {
                            _dataOffset++;
                        }
                        if (_dataOffset == _dataSize)
                        {
                            _dataOffset = 0;
                            _packetState = PACKET_STATE.DATA;
                        }
                        break;
                    case PACKET_STATE.DATA:
                        _packetState = PACKET_STATE.CRCL;
                        break;
                    case PACKET_STATE.CRCL:
                        _packetState = PACKET_STATE.CRCH;
                        PacketCompleted = true;
                        _processState = ProcessState.Idle;
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
