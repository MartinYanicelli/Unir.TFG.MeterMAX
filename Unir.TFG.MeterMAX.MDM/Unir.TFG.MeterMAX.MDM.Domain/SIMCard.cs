using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class SIMCard : Noanet.XamArch.Domain.Entity
    {
        public string SerialNumber { get; set; }
        public string PhoneNumber { get; set; }
        public PhoneCompany PhoneCompany { get; set; }
        public SIMCardService Service { get; set; }
    }
}
