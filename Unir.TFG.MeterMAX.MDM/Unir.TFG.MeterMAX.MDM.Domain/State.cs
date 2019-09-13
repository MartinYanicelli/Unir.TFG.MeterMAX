using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class State : Noanet.XamArch.Domain.Entity
    {
        public string Name { get; set; }
        public Country Country { get; set; }
    }
}
