using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

/*
 * The Identification Service shall be the first service issued upon C12.18
 * Returns the version and revision of the protocol where the version is positioned left of the decimal indicating 
 * a major change and the revision is positioned right of the decimal indicating a minor change. 
 * It may also return a device class or device identity. 
 * The size of the returned response shall never exceed the default packet size.
 * The Identification Service is a required service
 *      Request:
 *              <ident> ::= 20H
 * 
 * 
 *      Response:
 *              
 *              <ident-r> ::=   <isss> | <bsy> | <err> | 
 *                              <ok><std><ver><rev><feature>*<end-of-list>
 *                              
 * */
namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class IdentificationService : Service
    {
        public AnsiProtocol AnsiProtocol { get; private set; }
        public string VersionNumber { get; private set; }
        public IdentificationType IdentificationType { get; private set; }
        public AuthenticationType AuthenticationType { get; private set; }

        #region Constructor
        public IdentificationService()
            : base(ServiceRequestCode.Identification)
        {
            
        }
        #endregion
        
        protected override void OnProcessResponseData()
        {
            if (!Enum.IsDefined(typeof(AnsiProtocol), (int)responsePacket.DATA[1]))
                throw new ServiceException(ServiceExceptionType.PacketIntegrity, "La información sobre Protocolo de Comunicación no tiene un formato válido.");

            AnsiProtocol = (AnsiProtocol)responsePacket.DATA[1];
            VersionNumber = string.Format("{0}.{1}", responsePacket.DATA[2], responsePacket.DATA[3]);
            if (Enum.IsDefined(typeof(IdentificationType), responsePacket.DATA[4]))
            {
                IdentificationType = (IdentificationType)responsePacket.DATA[4];
                if ((IdentificationType == IdentificationType.AuthenticationService) || (IdentificationType == IdentificationType.AuthenticationServiceWithTicket))
                {
                    AuthenticationType = new AuthenticationType();
                    AuthenticationType.SesionLevelAuthentication = responsePacket.DATA[5].GetBit(0);
                    AuthenticationType.AlgorithmUsed = (AuthenticationAlgorithm)responsePacket.DATA[6];
                    if (IdentificationType == IdentificationType.AuthenticationServiceWithTicket)
                    {
                        AuthenticationType.Ticket = responsePacket.DATA.GetRange(8, responsePacket.DATA[7]).ToArray();
                    }
                }
            }
        }
    }
}
