using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public interface ITableParser
    {
        string Name { get; }
        void Parse(byte[] table);
    }
}
