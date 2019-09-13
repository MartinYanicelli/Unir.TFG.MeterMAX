using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class MeterProtocolSettingRepository : Noanet.XamArch.Infrastructure.Repository<MeterProtocolSetting>, IMeterProtocolSettingRepository
    {

        protected override MeterProtocolSetting BuildNewEntity(object[] data, [CallerMemberName] string callerMethodName = null)
        {
            return new MeterProtocolSetting() {
                Id = Convert.ToInt32(data[0]),
                UserId = Convert.ToInt32(data[1]),
                UserName = data[2].ToString(),
                Password = data[3].ToString()
            };
        }
    }
}
