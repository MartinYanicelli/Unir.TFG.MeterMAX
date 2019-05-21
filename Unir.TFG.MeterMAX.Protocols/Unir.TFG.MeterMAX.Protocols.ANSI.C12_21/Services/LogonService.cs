using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    /*
     * The Logon Service establishes a session without establishing access permissions.
     * The Logon Service is a required service. The Logon Service shall be initiated only by a C12.18 Client
     * 
     * Request:
     * The <user-id> parameter is a code, optionally stored by the C12.18 Device, indicating the identity of the operator requesting the creation of a session. 
     * The <user-id> may be inserted in the Event and History Logs as defined in ANSI C12.19. 
     * The <user> field provides the name of the operator requesting the access to the device.
     *  The Logon Service has the following format:
     *      <logon> ::= 50H <user-id><user>
     *          <user-id> ::= <word16>          {User identification code. This field is transferred to USER_ID in  Procedure 18 of C12.19.}
     *          <user> ::= <byte>+10            {10 bytes containing user identification}
     *
     * Response:
     * The responses <isss>, <iar>, <bsy> and <err> indicate a problem with the received Logon Service request.
     * The response <ok> indicates the Logon Service was successfully completed and the session was established.
     *      <logon-r> ::= <isss> | <iar> | <bsy> | <err> | <ok>
     * 
     * 
     * */
    public class LogonService : Service
    {
        private readonly byte[] _userId;
        private readonly byte[] _userName;

        public string UserName {
            get { return System.Text.ASCIIEncoding.ASCII.GetString(_userName, 0, _userName.Length); }
        }
        
        #region Constructor
        public LogonService(int userId, string userName)
            : base(ServiceRequestCode.Logon)
        {
            if (string.IsNullOrEmpty(userName))
                    throw new ArgumentNullException("userName", "El nombre de usuario no puede ser una una cadena nula o vacía.");

            _userId = BitConverter.GetBytes((ushort) userId);
            if (BitConverter.IsLittleEndian) Array.Reverse(_userId);
            userName = (userName.Length < 10) ? userName.PadRight(10) : userName.Substring(0, 10);
            _userName = System.Text.ASCIIEncoding.ASCII.GetBytes(userName);
        }
        #endregion

        protected override void OnFillRequestDataPacket(Packet requestPacket)
        {
            base.OnFillRequestDataPacket(requestPacket);
            requestPacket.DATA.AddRange(_userId);
            requestPacket.DATA.AddRange(_userName);
        }

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
