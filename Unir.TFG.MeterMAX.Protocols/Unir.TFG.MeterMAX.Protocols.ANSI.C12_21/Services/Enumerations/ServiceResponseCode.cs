using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

/*
 * PSEM responses always include a one-byte response code. 
 * These codes are listed below in a suggested order of priority. 
 * When more than one response code is capable of indicating the error response condition of a C12.18 Device or a C12.18 Client, 
 * the response code having the highest priority (from left to right) is as follows:
 *      <nok> ::= <sns>|<isss>|<iar>|<isc>|<onp>|<bsy>|<dlk>|<dnr>|<rno>|<err>
 * 
 * <ok> ::= 00H                             {Acknowledge
 *                                          Application Layer response indicating no problems, 
 *                                          request accepted.}
 *                                          
 * <err> ::= 01H                            {Error
 *                                          This Application Layer error code is used to indicate rejection of the received service request. 
 *                                          The reason for the rejection is not provided.}
 *                                          
 * <sns> ::= 02H                            {Service Not Supported
 *                                          This Application Layer error response will be sent to the device when the requested service is not supported. 
 *                                          This error indicates that the message was valid, but the request could not be honored.}
 *                                          
 * 
 * <isc> ::= 03H                            {Insufficient Security Clearance 
 *                                          This Application Layer error indicates that the current authorization level is insufficient to complete the request.}
 *                                          
 * 
 * <onp> ::= 04H                            {Operation Not Possible
 *                                          This Application Layer error will   be sent to the device which requested an action that is not possible. 
 *                                          This error indicates that the message was valid, but the message could not be processed. 
 *                                          Covers conditions such as: invalid length, invalid offset}
 *                                          
 * 
 * <iar> ::= 05H                            {Inappropriate Action Requested
 *                                          This Application Layer error indicates that the action requested was inappropriate. 
 *                                          Covers  conditions such as write request to a read-only table or an invalid table id.}
 *                                          
 * 
 * <bsy> ::= 06H                            {Device Busy
 *                                          This Application Layer error indicates that the request was not acted upon because the device was busy doing something else. 
 *                                          The operation may be retried at a later time with success, 
 *                                          as the data may then be ready for transportation during this active communication.}
 *                                          
 * 
 * <dnr> ::= 07H                            {Data Not Ready
 *                                          This Application Layer error indicates that request was unsuccessful because the requested data is not ready to be accessed.}
 *                                          
 * 
 * <dlk> ::= 08H                            {Data Locked
 *                                          This Application Layer error indicates that the request was unsuccessful because the data cannot be accessed.}
 *                                          
 * 
 * <rno> ::= 09H                            {Renegotiate Request
 *                                          This Application Layer error indicates that the responding device wishes to return to the ID or 
 *                                          Base State and renegotiate communication parameters.}
 *                                          
 * 
 * <isss> ::= 0AH                           {Invalid Service Sequence State
 *                                          This Application Layer error indicates that the request is not accepted at the current service sequence state. 
 *                                          For more information on service sequence states, see Annex C, Service Sequence State Control.}
 *  
 * */
namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations
{
    public enum ServiceResponseCode
    {
        Ok = 0x00,
        Error = 0x01,
        ServiceNotSupported = 0x02,
        InsufficientSecurityClearance = 0x03,
        OperationNotPossible = 0x04,
        InappropriateActionRequested = 0x05,
        DeviceBusy = 0x06,
        DataNotReady = 0x07,
        DataLocked = 0x08,
        RenegotiateRequest = 0x09,
        InvalidServiceSequenceState = 0x0A
    }
}
