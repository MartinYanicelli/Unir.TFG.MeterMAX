using System;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class Meter : Noanet.XamArch.Domain.Entity
    {
        public string Model { get; set; }
        public long SerialNumber { get; set; }
        public EnergySupply EnergySupply { get; set; }
        public MeterManufacturer MeterManufacturer { get; set; }
        public RemoteDevice RemoteDevice { get; set; }

    }
}
