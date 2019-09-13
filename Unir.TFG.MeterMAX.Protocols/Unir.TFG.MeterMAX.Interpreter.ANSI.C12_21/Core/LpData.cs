using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class LpData
    {
        public DateTime DateTime { get; set; }
        public string DataBlock { get; set; }
        public IList<IList<short>> Blocks { get; set; }
        
        public LpData()
        {
            Blocks = new List<IList<short>>();
        }
    }
}
