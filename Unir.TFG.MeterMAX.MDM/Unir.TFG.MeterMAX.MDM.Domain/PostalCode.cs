using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class PostalCode : Noanet.XamArch.Domain.Entity
    {
        public string Code { get; set; }
        public City City { get; set;}
    }
}
