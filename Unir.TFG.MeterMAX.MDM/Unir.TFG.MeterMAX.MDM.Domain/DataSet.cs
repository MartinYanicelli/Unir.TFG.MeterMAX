using System;
using System.Collections.Generic;
using System.Text;

namespace Unir.TFG.MeterMAX.MDM.Domain
{
    public class DataSet : Noanet.XamArch.Domain.Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<DataSetComponent> DataSetComponents { get; set; }

        public DataSet()
        {
            DataSetComponents = new List<DataSetComponent>();
        }
    }
}
