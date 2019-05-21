using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    /*
     * The Security Service is provided for setting access permissions and shall be initiated only during a session 
     * that was successfully established using the Logon Service. The Security Service is an optional service.
     * 
     * Request:
     *  <security> ::= 51H <password>
     *      <password> ::= <byte>+20                {20-byte field containing password}
     * 
     * Response:
     * The responses <sns>, <isss>, <bsy> and <err> indicate a problem with the received Security Service request.
     * The response <ok> indicates the Security Service was successfully completed and the access permissions associated with the password were granted.
     *  <security-r> ::= <sns> | <isss> | <bsy> | <err> | <ok> 
     * 
     * */
    public class SecurityService : Service
    {
        private readonly byte[] _password;

        #region Constructor
        public SecurityService(string password)
            : base(ServiceRequestCode.Security)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "La contraseña de seguridad no puede ser una cadena nula o vacía.");

            _password = System.Text.ASCIIEncoding.ASCII.GetBytes(((password.Length < 20) ? password.PadRight(20) : password.Substring(0, 20)));

        }
        #endregion

        protected override Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets.Packet OnCreateRequestPacket()
        {
            Packet packet = base.OnCreateRequestPacket();
            packet.CTRL = 0x20;
            return packet;
        }

        protected override void OnFillRequestDataPacket(Packet requestPacket)
        {
            base.OnFillRequestDataPacket(requestPacket);
            requestPacket.DATA.AddRange(_password);
        }

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
