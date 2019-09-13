using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class EnergySupply : Noanet.XamArch.Domain.Entity
    {
        public string Code { get; set; }
        public Address Address { get; set; }
        public Customer Customer { get; set; }
        public GeoCordinate GeoCordinate { get; set; }
        public EnergySupplyGroup Group { get; set; }
    }
}
