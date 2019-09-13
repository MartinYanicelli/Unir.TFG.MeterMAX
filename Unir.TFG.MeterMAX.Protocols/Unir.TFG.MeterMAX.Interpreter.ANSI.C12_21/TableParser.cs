using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public abstract class TableParser : ITableParser
    {
        public string Name { get { return this.GetType().Name; } }
        
        public TableParser() 
        {
            
        }

        public void Parse(byte[] table)
        {
            int dataLen = table.Length - 1;
            byte[] data = new byte[dataLen]; // excluyo el checksum
            Array.Copy(table, data, data.Length);
            
            if (CheckSum(data) != table[dataLen])
            {
                throw new InvalidOperationException("Chesum Error!!"); 
            }

            OnParse(data);
        }

        protected abstract void OnParse(byte[] table);

        private byte CheckSum(byte[] data)
        {
            int checksum = 0;
            foreach (var item in data)
            {
                checksum += item;
            }
            checksum = (~checksum & 0xff) + 1;
            return (byte)(checksum & 0xff);
        }
    }
}
