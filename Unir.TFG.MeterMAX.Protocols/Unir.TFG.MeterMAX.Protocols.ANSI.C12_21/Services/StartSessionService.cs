using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class StartSessionService : RemoteService
    {
        private readonly byte _interfaceType = 0x00;
        private readonly byte _communicationTimeout;
        private readonly byte[] _password = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        private readonly BaudRateValue _baudRateValue = BaudRateValue.BaudRate_9600;

        #region Constructor
        public StartSessionService(int communicationTimout = 20000) : base(RemoteServiceType.ServiceWithData, RemoteServiceRequestCode.StartRemoteSession)
        {
            _communicationTimeout = (byte) TimeSpan.FromMilliseconds(communicationTimout).TotalSeconds;
        }
        #endregion

        protected override void OnFillRequestDataPacket(RemotePacket requestPacket)
        {
            requestPacket.DATA.Add(_interfaceType);
            requestPacket.DATA.Add((byte)_baudRateValue);
            requestPacket.DATA.Add((byte)_communicationTimeout);
            requestPacket.DATA.AddRange(_password);
        }

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
