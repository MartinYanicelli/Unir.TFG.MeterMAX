using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class DataSetComponentRepository : Noanet.XamArch.Infrastructure.Repository<DataSetComponent>, IDataSetComponentRepository
    {
        protected override DataSetComponent BuildNewEntity(object[] data, [CallerMemberName] string callerMethodName = null)
        {
            return new DataSetComponent() {
                Id = Convert.ToInt32(data[0]),
                Name = data[1].ToString(),
                Description = data[2]?.ToString()
            };
        }

    }
}
