using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class MeterSessionTypeRepository : Noanet.XamArch.Infrastructure.Repository<MeterSessionType>, IMeterSessionTypeRepository
    {
        protected override MeterSessionType BuildNewEntity(object[] data, [CallerMemberName] string callerMethodName = null)
        {
            return new MeterSessionType() {
                 Id = Convert.ToInt32(data[0]),
                 Name = data[1].ToString(),
                 Description = data[2]?.ToString()
            };
        }
    }
}
