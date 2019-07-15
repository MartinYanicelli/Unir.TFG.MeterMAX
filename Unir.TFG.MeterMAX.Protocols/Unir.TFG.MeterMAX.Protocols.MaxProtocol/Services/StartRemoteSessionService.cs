using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.MaxProtocol.Packets;
using Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.MaxProtocol.Services
{
    public class StartRemoteSessionService : MaxService
    {
        private readonly byte _interfaceType = 0x00;
        private readonly byte _communicationTimeout;
        private readonly byte[] _password = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        private readonly MaxBaudRate _baudRate = MaxBaudRate.BaudRate_9600;

        #region Constructor
        public StartRemoteSessionService(int communicationTimout = 20000) : base(MaxServiceType.ServiceWithData, MaxServiceRequestCode.StartRemoteSession)
        {
            _communicationTimeout = (byte) TimeSpan.FromMilliseconds(communicationTimout).TotalSeconds;
        }
        #endregion

        protected override void OnFillRequestDataPacket(MaxPacket requestPacket)
        {
            requestPacket.DATA.Add(_interfaceType);
            requestPacket.DATA.Add((byte)_baudRate);
            requestPacket.DATA.Add(_communicationTimeout);
            requestPacket.DATA.AddRange(_password);
        }

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
