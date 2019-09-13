using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core
{
    public class Source
    {
        public UOMCode UOMCode { get; set; }
        public SourceFlow Flow { get; set; }
        public SourceSegmentation Segmentation { get; set; }
        public SourceUsage Usage { get; set; }
        public SourceHarmonic Harmonic { get; set; }
        public int ScaleFactor { get; set; }
        public int MultiplierSelect { get; set; }
    }
}
