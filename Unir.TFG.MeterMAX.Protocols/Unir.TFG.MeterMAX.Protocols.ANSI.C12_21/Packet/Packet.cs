using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

/* [STP] [IDENTITY] [CTRL] [SEQ_NUMBER] [LEN] [DATA] [CRC]  Formato base de un mensaje Alpha3: 
 *  STP         = inicio de Paquete.
 *  IDENTITY    = Identificación del Dispositivo. Identifica al dispositivo tanto en las solicitudes como en las respuestas.
 *  CTRL        = Campo de Control.
 *                  - Bit 7: Verdadero indica que el paquete (Packet) es parte de una transmisión de múltiples paquetes.
 *                  - Bit 6: Verdadero indica que el paquete es el primero dentro de la transmisión de múltiples paquetes.
 *                  - Bit 5: Representa un bit de "toogle" para rechazar paquetes duplicados.
 *                  - Bit 4-2: Reservados. Se deben colocar con el valor 0.
 *                  - Bit 0-1: Formato de Datos
 *                      #0 = C12.18 or C12.21
 *                      #1 = C12.22
 *                      #2 = Reserved
 *                      #3 = Reserved
 *  SEQ_NUMBER  = Sequencia de paquete.
 *                Número que debe ser decrementado en 1 por cada nuevo paquete enviado.
 *                El valor de secuencia del primer paquete enviado en una transmisión de múltiples paquetes debe ser igual al número total de paquetes menos uno.
 *  LEN         = número de bytes de datos del paquete (2 bytes)
 *  DATA        = datos propiamente dichos del paquetes (N bytes). 
 *  CRC         = comprobación de redundancia cíclica del mensaje (2 bytes)
 */
namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Packets
{
    public class Packet
    {
        #region Public Properties
        public byte STP { get; set; }
        public byte IDENTITY { get; set; }
        public byte CTRL { get; set; }
        public byte SEQ_NUMBER { get; set; }
        public byte LENH { get; set; }
        public byte LENL { get; set; }
        public List<byte> DATA { get; set; }
        public byte? CRCL { get; set; }
        public byte? CRCH { get; set; }
        #endregion

        #region constructor
        public Packet()
        {
            STP = 0xEE;
            IDENTITY = 0x00;
            CTRL = 0x00;
            SEQ_NUMBER = 0x00;
            DATA = new List<byte>();
        }
        #endregion

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendFormat("{0:X2} {1:X2} {2:X2} {3:X2} {4:x2} {5:X2} ", STP, IDENTITY, CTRL, SEQ_NUMBER, LENH, LENL);
            
            foreach (var data in DATA)
            {
                result.AppendFormat("{0:X2} ", data);
            }

            if (CRCH.HasValue)
                result.AppendFormat("{0:X2} ", CRCH.Value);

            if (CRCL.HasValue)
                result.AppendFormat("{0:X2} ", CRCL.Value);

            result.AppendLine();

            return result.ToString();
        }

        public byte[] ToArray()
        {
            var buffer = new List<byte> { STP, IDENTITY, CTRL, SEQ_NUMBER, LENH, LENL };
            buffer.AddRange(DATA);
            if (CRCH.HasValue)
                buffer.Add(CRCH.Value);
            if (CRCL.HasValue)
                buffer.Add(CRCL.Value);
            return buffer.ToArray();
        }

        public bool IsMultiple()
        {
            return (CTRL & (1 << 7)) != 0;
        }

        public bool IsFirstMultiple()
        {
            return (CTRL & (1 << 6)) != 0;
        }
    }
}
