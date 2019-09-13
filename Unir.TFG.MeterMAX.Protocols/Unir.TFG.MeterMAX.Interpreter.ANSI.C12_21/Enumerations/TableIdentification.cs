using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations
{
    public enum TableIdentification : short
    {
        GeneralManufacturerIdentification = 256,
        ModeAndStatusStandardTable = 768,
        StatusManufacturerTable = 776,
        Multipliers = 3840,
        PrimaryMeteringInformation = 3848,
        ElsterSourceDefinitionTable = 4360,
        ActualRegisterTable = 5376,
        DataSelectionTable = 5632,
        CurrentRegisterDataTable = 5888,
        PreviousSeasonDataTable = 6144,
        PreviousDemandResetDataTable = 6400,
        PresentRegisterDataTable = 7168,
        ActualLoadProfileTable = 15616,
        LoadProfileDataSet1Table = 16384,
        EventLogDataTable = 19456
    }
}
