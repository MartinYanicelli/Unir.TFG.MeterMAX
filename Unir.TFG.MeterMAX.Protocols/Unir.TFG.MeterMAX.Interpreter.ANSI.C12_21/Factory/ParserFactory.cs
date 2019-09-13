using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Factory
{
    public class ParserFactory
    {
        public static ITableParser Creator(byte[] tableId)
        {
            short table = BitConverter.ToInt16(tableId, 0);
            
            if (!Enum.IsDefined(typeof(TableIdentification), table))
                throw new ArgumentOutOfRangeException("tableId", "El identificador de tabla es inválido o no está definido dentro del constructor.");

            return Creator((TableIdentification)table);
            
        }

        public static ITableParser Creator(TableIdentification tableName)
        {
            ITableParser tableParser = null;

            switch (tableName)
            {
                case TableIdentification.GeneralManufacturerIdentification:
                    tableParser = new GeneralManufacturerIdentificationTable();
                    break;
                case TableIdentification.ModeAndStatusStandardTable:
                    break;
                case TableIdentification.StatusManufacturerTable:
                    break;
                case TableIdentification.Multipliers:
                    break;
                case TableIdentification.PrimaryMeteringInformation:
                    tableParser = new PrimaryMeteringInformation();
                    break;
                case TableIdentification.ElsterSourceDefinitionTable:
                    tableParser = new ElsterSourceDefinitionTable();
                    break;
                case TableIdentification.ActualRegisterTable:
                    tableParser = new ActualRegisterTable();
                    break;
                case TableIdentification.DataSelectionTable:
                    tableParser = new DataSelectionTable();
                    break;
                case TableIdentification.CurrentRegisterDataTable:
                    tableParser = new CurrentRegisterDataTable();
                    break;
                case TableIdentification.PreviousSeasonDataTable:
                    tableParser = new PreviousSeasonDataTable();
                    break;
                case TableIdentification.PreviousDemandResetDataTable:
                    tableParser = new PreviousDemandResetDataTable();
                    break;
                case TableIdentification.PresentRegisterDataTable:
                    tableParser = new PresentRegisterDataTable();
                    break;
                case TableIdentification.ActualLoadProfileTable:
                    tableParser = new ActualLoadProfileTable();
                    break;
                case TableIdentification.LoadProfileDataSet1Table:
                    tableParser = new LoadProfileDataSet1Table();
                    break;
                case TableIdentification.EventLogDataTable:
                    tableParser = new EventLogDataTable();
                    break;
                default:
                    break;
            }

            return tableParser;
        }
    }
}
