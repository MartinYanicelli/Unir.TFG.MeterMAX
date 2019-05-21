using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class AuthenticationType
    {
        public bool SesionLevelAuthentication { get; set; }
        public AuthenticationAlgorithm AlgorithmUsed { get; set; }
        public byte[] Ticket { get; set; }
    }
}
