using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class RemoteDevice : Noanet.XamArch.Domain.Entity
    {
        public Vendor Vendor { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public SIMCard SIMCard { get; set; }
        public string Ip { get; set; }
        public int PortNumber { get; set; }
    }
}
