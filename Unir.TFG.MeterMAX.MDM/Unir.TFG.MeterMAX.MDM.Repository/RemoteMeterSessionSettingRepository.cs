using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;
using Unir.TFG.MeterMAX.MDM.Domain;
using Noanet.XamArch.Domain;
using System.Data;
using Noanet.XamArch.Infrastructure;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class RemoteMeterSessionSettingRepository : MeterSessionSettingRepository<RemoteMeterSessionSetting>, IRemoteMeterSessionSettingRepository
    {
        protected override string[] GetPropertyNames()
        {
            return new string[] {
                $"{TableAlias}.{nameof(RemoteMeterSessionSetting.UseMeterMAXProtocol)}"
            };
        }

        protected override void SetPropertyValue(RemoteMeterSessionSetting entity, string propertyName, object propertyValue)
        {
            if (propertyName == nameof(entity.UseMeterMAXProtocol))
            {
                entity.UseMeterMAXProtocol = Convert.ToBoolean(propertyValue);
            }
        }
    }
}
