using Noanet.XamArch.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;

namespace Unir.TFG.MeterMAX.MDM.Logic.Validations
{
    public class AuthenticationResult
    {
        public ValidationResult ValidationResult { get; set; }
        public UserSession UserSession { get; set; }
    }
}
