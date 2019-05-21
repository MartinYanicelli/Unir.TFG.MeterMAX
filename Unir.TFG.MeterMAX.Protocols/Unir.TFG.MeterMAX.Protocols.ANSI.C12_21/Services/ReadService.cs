using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    /*
     * 
     * The Read Service is used to transfer Table data to the requesting device 
     * and shall be initiated only during a session that was successfully established using the Logon Service.
     * 
     * Request:
     * The Read Service request supports both complete and partial Table transfers. The retrieval of a portion of a Table is possible through the use of
     * both index/element-count and offset/octet-count methods. 
     *      Codes 30H–39H, 3EH and 3FH are the Read Service request codes. 
     *      Request code 30H specifies a complete Table transfer. 
     *      Request codes 31H through 39H specify a partial Table transfer using 1 to 9 indices. 
     *      Request code 3EH specifies a default Table transfer. 
     *      Request code 3FH specifies a partial Table transfer using the offset/octet-count method.
     *      
     *          <read> ::= <full-read> | <pread-index> | <pread-offset> |  <pread-default>
     *          
     *          <full-read> ::= 30H <tableid>
     *          
     *          <pread-index> ::= <read-index-type><tableid><index>+<element-count>
     *              <read-index-type> ::= 31H |                 {1 <index> included in request}
     *                                    32H |                 {2 <index> included in request}
     *                                    33H |                 {3 <index> included in request}
     *                                    34H |                 {4 <index> included in request}
     *                                    35H |                 {5 <index> included in request}
     *                                    36H |                 {6 <index> included in request}
     *                                    37H |                 {7 <index> included in request}
     *                                    38H |                 {8 <index> included in request}
     *                                    39H                   {9 <index> included in request}
     *          
     *          <pread-default> ::= 3EH                         {Transfer default Table as defined by the C12.19 Device.}
     *          
     *          <pread-offset> ::= 3FH <tableid><offset><octet-count>
     *              <tableid> ::= <word16>                      {Table identifier as defined in ANSI C12.19.}
     *              <offset> ::= <word24>                       {Offset into data Table in bytes. Offset 0000H is the offset to the first table element 
     *                                                          of the Table selected by <tableid>.}
     *              <index> ::= <word16>                        {Index value used to locate start of data. Index 0000H is the index of the first Table element 
     *                                                          of the Table selected by <tableid>.}
     *              <element-count> ::= <word16>                {Number of Table elements to read starting at the requested index.}
     *              <octet-count> ::= <word16>                  {Length of Table data requested starting at Table <offset>, in bytes.}
     * 
     * Response:
     * Responses of type <nok> indicate a problem with the received Read Service request.
     * The response <ok> indicates the Read Service was accepted and the data is part of the response.
     *          <read-r> ::= <nok> | <ok><table-data>
     *              <table-data> ::= <count><data><cksum>
     *                  <count> ::= <word16>                    {Length of <data> returned, in bytes.}
     *                  <data> ::= <byte>*                      {The returned Table data including the optional pending header as per ANSI C12.19, when requested.}
     *                  <cksum> ::= <byte>                      {2's compliment checksum computed only on the <data> portion of <table-data>. 
     *                                                          The checksum is computed by summing the bytes (ignoring overflow) and negating the result.}
     * 
     * 
     * */
    public class ReadService : Service
    {
        private readonly ReadingType _readingType;
        private readonly object[] _parameters;

        public Table Table { get; private set; }
        public List<byte> TableData { get; private set; }
        public int TableDataLength { get; private set; }

        #region Constructor
        public ReadService(ReadingType readingType, object[] args)
            : base()
        {
            if (readingType != ReadingType.PartialReadDefault)
            {
                if ((args == null) || (args.Length == 0))
                    throw new ArgumentNullException("args", "Debe especificar los parámetros requeridos para la lectura de la Tabla.");

                if ((readingType == ReadingType.PartialReadWithIndex) && !Enum.IsDefined(typeof(IndexType), args[0]))
                    throw new InvalidCastException("El parámetro para especificar el tipo de índice de lectura no tiene un formato válido.");
                
                if (((readingType == ReadingType.FullRead) && (args.Length != 1)) || 
                    ((readingType != ReadingType.FullRead) && (args.Length != 3)))
                    //((readingType == ReadingType.PartialReadWithOffset) && (args.Length != 3)) ||
                    //((readingType == ReadingType.PartialReadWithIndex) && ((args.Length < 4) && ((((byte)args[0] + 1) * 2) != (args.Length - 2)))))
                        throw new ArgumentOutOfRangeException("args", "Los parámetros especificados no coinciden con la cantidad esperada.");

                int index = (readingType == ReadingType.PartialReadWithIndex) ? 1 : 0;
                Table = args[index] as Table;

                if (Table == null)
                    throw new ArgumentOutOfRangeException("args", string.Format("El parámetro que define la tabla no tiene un valor válido. Valor {0}", args[index]));
            }
            else
            {
                if (args != null)
                    throw new ArgumentException("Table, args", "El tipo de lectura solicitado no requiere de parámetros adicionales.");
            }

            _readingType = readingType;
            _parameters = args;
            TableData = new List<byte>();
        }
        
        #endregion

        protected override void OnFillRequestDataPacket(Packet requestPacket)
        {
            byte readServiceValue = (byte)_readingType;
            switch (_readingType)
            {
                case ReadingType.FullRead:
                    // <full-read> ::= 30H <tableid>
                    requestPacket.DATA.Add(readServiceValue);
                    requestPacket.DATA.AddRange(Table.GetId());
                    break;
                case ReadingType.PartialReadWithIndex:
                    // <pread-index> ::= <read-index-type><tableid><index>+<element-count>
                    requestPacket.DATA.Add((byte)(readServiceValue + (byte)_parameters[0]));
                    byte[] index = BitConverter.GetBytes(Convert.ToUInt16(_parameters[2])).ToArray();
                    byte[] elementCount = BitConverter.GetBytes(Convert.ToUInt16(_parameters[3])).ToArray();
                    if (BitConverter.IsLittleEndian) 
                    {
                        Array.Reverse(index); Array.Reverse(elementCount);
                    } 
                    requestPacket.DATA.AddRange(Table.GetId());
                    requestPacket.DATA.AddRange(index);
                    requestPacket.DATA.AddRange(elementCount);
                    break;
                case ReadingType.PartialReadWithOffset:
                    // <pread-offset> ::= 3FH <tableid><offset><octet-count>
                    requestPacket.DATA.Add(readServiceValue);
                    byte[] offset = BitConverter.GetBytes(Convert.ToInt32(_parameters[1])).Take(3).ToArray(); 
                    byte[] octetCount = BitConverter.GetBytes(Convert.ToUInt16(_parameters[2])).ToArray();
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(offset); Array.Reverse(octetCount);
                    }
                    requestPacket.DATA.AddRange(Table.GetId());
                    requestPacket.DATA.AddRange(offset);
                    requestPacket.DATA.AddRange(octetCount);
                    break;
                case ReadingType.PartialReadDefault:
                    // <pread-default> ::= 3EH
                    requestPacket.DATA.Add(readServiceValue);
                    break;
                default:
                    break;
            }
        }

        protected override Packet OnCreateResponsePacket(byte[] data)
        {
            Packet packet = base.OnCreateResponsePacket(data);
            //TODO: determinar si hay más paquetes...
            /* <ctrl> ::= <byte>                     {Control field.
             *                                      Bit 7. If true (01H) then this   packet is part of a multiple packet transmission.
             *                                      Bit 6. If true (01H), then this packet is the first packet of a ANSI C12.18 – 2005  23  multiple packet transmission, and   Bit 7 shall also be true.
             *                                      Bit 5. Represents a toggle bit to   reject duplicate packets. This bit  shall be toggled for each new   packet sent. Retransmitted packets  keep the same state as the original packet sent. It should be noted that the initial state of the   toggle bit is not specified and could initially be either zero (0)  or one (1). 
             *                                      Bits 4 to 2, Reserved. The bits shall be set to zero (0) by the transmitter.
             *                                      Bit 0 to 1: DATA_FORMAT
             *                                          0 = C12.18 or C12.21
             *                                          1 = C12.22
             *                                          2 = Reserved
             *                                          3 = Reserved
             *                                          
             *                                      C12.18 Compliant implementations shall set Bits 0 and 1 to zero (0).}
             *                                      
             * <seq-nbr> ::= <byte>                 {Number that is decremented by one (1) for each new packet sent. The first packet in a multiple packet transmission shall have a <seq-nbr> equal to the total number of packets minus one (1). A value of zero (0) in this field indicates that this packet is the last packet of a multiple packet transmission. If only one (1) packet in a transmission, this field shall be set to zero (0), and Bit 7 and Bit 6 shall be set to zero (0).}
             * */
            return packet;
        }

        protected override void OnProcessResponseData()
        {
            /*
             * <table-data> ::= <count><data><cksum>
             *      <count> ::= <word16>
             *      <data> ::= <byte>*
             *      <cksum> ::= <byte>           
             **/
            // Extraer los datos puros de la tabla y comprobar su integridad.
            // verificar el tipo de paquete y determinar si es el primero
            if (!IsMultiplePacket || IsFirstPacket)
            {
                TableDataLength = BitConverter.ToInt16(new byte[] { responsePacket.DATA[2], responsePacket.DATA[1] }, 0);
                TableData.AddRange(responsePacket.DATA.GetRange(3, responsePacket.DATA.Count - 3));
            }
            else
            {
                TableData.AddRange(responsePacket.DATA);
            }
            
            if (!HasMorePackets)
            {
                // last packet
                //TODO: comprobar checksum de datos de la tabla.
                byte checkSum = responsePacket.DATA[responsePacket.DATA.Count - 1];
            }
        }
        
    }
}
