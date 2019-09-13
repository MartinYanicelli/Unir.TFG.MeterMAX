using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class GeoCordinate : Noanet.XamArch.Domain.Entity
    {
        public double? Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
