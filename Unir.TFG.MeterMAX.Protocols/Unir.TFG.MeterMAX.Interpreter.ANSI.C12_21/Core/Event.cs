using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class Event
    {
        public DateTime DateTime { get; set; }
        public int SequenceNumber { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Descripcion { get; set; }
    }
}
