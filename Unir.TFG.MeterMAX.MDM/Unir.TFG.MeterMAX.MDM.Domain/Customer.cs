using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class Customer : Noanet.XamArch.Domain.Entity
    {
        public string IdentificationNumber { get; set; }
        public string Name { get; set; }
    }
}
