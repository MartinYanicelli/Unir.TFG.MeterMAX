using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class GeneralManufacturerIdentificationTable : TableParser
    {
        public string Manufacturer { get; private set; }
        public string Model { get; private set; }
        public string SerialNumber { get; private set; }
        public string HardwareVersion { get; private set; }
        public string FirmwareVersion { get; private set; }

        public GeneralManufacturerIdentificationTable() { }

        protected override void OnParse(byte[] table)
        {
            Manufacturer = System.Text.Encoding.ASCII.GetString(table, 0, 4).Trim();
            Model = System.Text.Encoding.ASCII.GetString(table, 2, 8).Trim();

            HardwareVersion = string.Format("{0}.{1}", table[12], table[13]);
            FirmwareVersion = string.Format("{0}.{1}", table[14], table[15]);
            
            SerialNumber = System.Text.Encoding.ASCII.GetString(table, 16, 16).Trim();

        }

        public override string ToString()
        {
            return string.Format("Manufacturer: {0}, Model: {1}, Hardware Version: {2}, Firmware Version: {3}, SerialNumber: {4}", Manufacturer, 
                Model, HardwareVersion, FirmwareVersion, SerialNumber); 
        }
    }
}
