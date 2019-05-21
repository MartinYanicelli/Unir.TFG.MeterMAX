using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class WaitService : Service
    {
        public readonly byte WaitTime;

        #region Constructor
        public WaitService(byte secondsWait)
            : base(ServiceRequestCode.Wait)
        {
            WaitTime = secondsWait;
        }
        #endregion

        protected override void OnFillRequestDataPacket(Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets.Packet requestPacket)
        {
            base.OnFillRequestDataPacket(requestPacket);
            requestPacket.DATA.Add(WaitTime);
        }

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
