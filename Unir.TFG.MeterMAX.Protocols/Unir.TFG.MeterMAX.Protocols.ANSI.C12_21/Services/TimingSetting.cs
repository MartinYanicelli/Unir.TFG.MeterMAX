using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class TimingSetting
    {
        /// <summary>
        /// Traffic Timeout in seconds
        /// </summary>
        public byte ChannelTrafficTimeout { get; set; }
        /// <summary>
        /// Inter-character time-out in seconds
        /// </summary>
        public byte InterCharacterTimeout { get; set; }
        /// <summary>
        /// Response time-out in seconds
        /// </summary>
        public byte ResponseTimeout { get; set; }
        /// <summary>
        /// Maximum number of retry attempts
        /// </summary>
        public byte NumberOfRetries { get; set; }
    }
}
