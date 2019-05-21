using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class Table
    {
        public TableName Name { get; set; }
        
        public Table(TableName name)
        {
            Name = name;
        }
        
        public byte[] GetId()
        {
            return BitConverter.GetBytes((short)Name);
        }

        public short CalculateTableId(short tableNumber, TableType type)
        {
            byte[] tableId = (type == TableType.Standard) ? BitConverter.GetBytes(tableNumber) : BitConverter.GetBytes(tableNumber | (1 << 11));
            if (BitConverter.IsLittleEndian) Array.Reverse(tableId);
            return BitConverter.ToInt16(tableId, 0);
        }
    }
}
