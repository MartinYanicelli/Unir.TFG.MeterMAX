using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Noanet.XamArch.Domain;
using Noanet.XamArch.Infrastructure;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class InsituMeterSessionSettingRepository : MeterSessionSettingRepository<InsituMeterSessionSetting>, IInsituMeterSessionSettingRepository
    {
        protected override string[] GetPropertyNames()
        {
            return new string[] {
                $"{TableAlias}.{nameof(InsituMeterSessionSetting.PortName)}",
                $"{TableAlias}.{nameof(InsituMeterSessionSetting.DtrEnabled)}"
            };
        }

        protected override void SetPropertyValue(InsituMeterSessionSetting entity, string propertyName, object propertyValue)
        {
            if (propertyName == nameof(entity.PortName))
            {
                entity.PortName = (string)propertyValue;
            }
            else if (propertyName == nameof(entity.DtrEnabled))
            {
                entity.DtrEnabled = (bool?)propertyValue; // propetyValue != null ? (bool)propertyValue : (bool?)null;
            }
        }
    }
}
