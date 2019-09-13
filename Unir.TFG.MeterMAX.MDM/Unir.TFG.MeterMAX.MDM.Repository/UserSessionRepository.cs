using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Noanet.XamArch.Domain;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class UserSessionRepository : Noanet.XamArch.Infrastructure.Repository<UserSession>, IUserSessionRepository
    {

        public UserSessionRepository() : base()
        { }


        protected override IEnumerable<IEntityPropertyInfo> GetInsertPropertyInfo(UserSession entity)
        {
            return new EntityPropertyInfo[] {
                new EntityPropertyInfo(nameof(entity.StartDate), entity.StartDate),
                new EntityPropertyInfo("UserId", entity.User.Id)
            };
        }

        protected override IEnumerable<IEntityPropertyInfo> GetUpdatePropertyInfo(UserSession entity)
        {
            return new EntityPropertyInfo[]
            {
                new EntityPropertyInfo(nameof(entity.EndDate), entity.EndDate)
            };
        }
    }
}
