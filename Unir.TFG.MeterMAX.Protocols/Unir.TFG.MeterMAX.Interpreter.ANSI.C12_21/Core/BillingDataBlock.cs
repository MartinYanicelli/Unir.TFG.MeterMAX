using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class BillingDataBlock
    {
        public string Name { get; set; }
        public IList<BillingRegister> Summations { get; set; }
        public IList<Demand> Demands { get; set; }
        public IList<BillingRegister> Coincidents { get; set; }

        public BillingDataBlock()
        {
            Summations = new List<BillingRegister>();
            Demands = new List<Demand>();
            Coincidents = new List<BillingRegister>();
        }

        public BillingDataBlock(string name) : this()
        {
            Name = name;
        }
    }
}
