using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class Street : Noanet.XamArch.Domain.Entity
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string Floor { get; set; }
    }
}
