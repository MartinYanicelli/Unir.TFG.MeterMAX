using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class Vendor : Noanet.XamArch.Domain.Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
    }
}
