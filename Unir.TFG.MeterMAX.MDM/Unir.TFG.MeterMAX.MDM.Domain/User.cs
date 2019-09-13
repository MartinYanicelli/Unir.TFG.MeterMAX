
using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class User : Noanet.XamArch.Domain.Entity
    {
        public string UserName { get; set; }
        public string Identication { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
