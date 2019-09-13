using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class UserSession : Noanet.XamArch.Domain.Entity
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public User User { get; set; }
    }
}
