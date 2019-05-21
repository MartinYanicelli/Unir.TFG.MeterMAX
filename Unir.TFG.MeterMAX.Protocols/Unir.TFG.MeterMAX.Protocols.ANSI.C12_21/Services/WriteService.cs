using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services.Enumerations;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services
{
    public class WriteService : Service
    {
        private readonly WritingType _writingType;
        private readonly object[] _parameters;

        public Table Table { get; private set; }

        #region Constructor
        public WriteService(WritingType writingType, Table table, object[] args)
        {
            _writingType = writingType;
            Table = table;
            _parameters = args;
        }
        #endregion

        protected override void OnProcessResponseData()
        {
            throw new NotImplementedException();
        }
    }
}
