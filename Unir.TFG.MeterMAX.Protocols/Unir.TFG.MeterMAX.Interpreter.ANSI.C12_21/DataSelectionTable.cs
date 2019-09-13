using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class DataSelectionTable : TableParser
    {
        public ActualRegisterTable ActualRegisterTable { get; set; }
        public ElsterSourceDefinitionTable SourceDefinitionTable { get; set; }

        public readonly IDictionary<int, Source> SummationSelect;
        public readonly IDictionary<int, Source> DemandSelect;
        public readonly IDictionary<int, Source> CoincidentSelect;
        
        public DataSelectionTable() 
            : base()
        {
            SummationSelect = new Dictionary<int, Source>();
            DemandSelect = new Dictionary<int, Source>();
            CoincidentSelect = new Dictionary<int, Source>();
        }

        public DataSelectionTable(ActualRegisterTable actualRegisterTable, ElsterSourceDefinitionTable sourceDefinitionTable)
            : this()
        {
            ActualRegisterTable = actualRegisterTable;
            SourceDefinitionTable = sourceDefinitionTable;
        }

        protected override void OnParse(byte[] table)
        {
            if ((ActualRegisterTable == null) || (SourceDefinitionTable == null))
                throw new InvalidOperationException("Las propiedades ActualRegisterTable y SourceDefinitionTable no pueden ser nulas.");

            int offSet = 0;
            for (int index = 0; index < ActualRegisterTable.NumberOfSummations; index++)
			{
                SummationSelect.Add(index, SourceDefinitionTable.Sources[table[offSet]]);
                offSet++;
			}

            for (int index = 0; index < ActualRegisterTable.NumberOfDemmands; index++)
			{
                DemandSelect.Add(index, SourceDefinitionTable.Sources[table[offSet]]);
                offSet++;
			}

            offSet += (ActualRegisterTable.NumberOfDemmands + 7) / 8;

            for (int index = 0; index < ActualRegisterTable.NumberOfCoincidentValues; index++)
			{
                CoincidentSelect.Add(index, SourceDefinitionTable.Sources[table[offSet]]);
                offSet++;
			}
        }
    }
}
