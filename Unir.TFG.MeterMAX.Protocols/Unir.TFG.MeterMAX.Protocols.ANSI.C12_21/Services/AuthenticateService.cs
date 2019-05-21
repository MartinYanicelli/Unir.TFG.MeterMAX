using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

/*
 * The Authenticate Service is used to provide a two-way authentication with playback rejection
 * during a session. It is recommended that the Authentication Service be used to complement the
 * Security Service, not be used exclusively. The contents of the <auth-request> and <authresponse> fields are a function of the authentication algorithm used. 
 * This algorithm is returned by the Identification Service response.
 * The Authenticate Service is an optional service.
 * 
 * Request:
 *          <authenticate> ::= 53H <auth-req-length><auth-request>
 *                  <auth-req-length> ::= <byte> {<auth-req-length> number of bytes of the <auth-request> field.}
 *                  <auth-request> ::= <byte>+ {Information used to authenticate the initiator of this service.}
 * 
 * Response:
 *  The responses <sns>, <isss>, <bsy>, and <err> indicate a problem with the received Authenticate Service request.
 *  The response <isc> indicates the authentication failure of the requester.
 *  The response <ok> indicates the Authenticate Service was successfully completed and the access permission associated with the <auth-request> field was granted.
 *          <authenticate-r> ::= <sns> | <isss> | <isc> | <bsy> | <err> | <ok><auth-res-length><auth-response>
 *                  <auth-res-length> ::= <byte> {Number of bytes of the <authresponse> field.}
 *                  <auth-response> ::= <byte>+ {Information used to authenticate the recipient of this service.}
 * */
namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class AuthenticateService : Service
    {
        private readonly AuthenticationType _authenticationType;
        public byte[] AuthenticationReponse { get; private set; }
        private readonly byte[] _password;
   
        #region constructor
        public AuthenticateService(string password, AuthenticationType authenticationType) 
            : base(ServiceRequestCode.Authenticate) 
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "La contraseña de seguridad no puede ser una una cadena nula o vacía.");

            _password = Encoding.ASCII.GetBytes((password.Length < authenticationType.Ticket.Length ) ? password.PadRight(authenticationType.Ticket.Length) : password.Substring(0, authenticationType.Ticket.Length));

            _authenticationType = authenticationType;
        }
        #endregion

        protected override void OnFillRequestDataPacket(Packets.Packet requestPacket)
        {
            // <authenticate> ::= 53H <auth-req-length><auth-request>
            byte[] authenticationRequest = DESEncryptor.EncryptTicket(_authenticationType.Ticket, _password);
            base.OnFillRequestDataPacket(requestPacket);
            requestPacket.DATA.AddRange(new byte[] { (byte) (authenticationRequest.Length + 1), 0x00 }); // muy raro pero hay que colocar un 0x00 antes del password encriptado.
            requestPacket.DATA.AddRange(authenticationRequest);
        }

        protected override void OnProcessResponseData()
        {
            AuthenticationReponse = new byte[responsePacket.DATA[1]];
            AuthenticationReponse = responsePacket.DATA.GetRange(2, AuthenticationReponse.Length).ToArray();
        }
    }
}
