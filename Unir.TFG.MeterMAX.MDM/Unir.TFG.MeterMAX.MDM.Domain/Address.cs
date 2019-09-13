using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class Address : Noanet.XamArch.Domain.Entity
    {
        public Street Street { get; set; }
        public PostalCode PostalCode {get; set; }
        
    }
}
