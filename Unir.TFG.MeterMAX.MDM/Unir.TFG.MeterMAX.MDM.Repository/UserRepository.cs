using System;
using System.Runtime.CompilerServices;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class UserRepository : Noanet.XamArch.Infrastructure.Repository<User>, IUserRepository
    {
        public UserRepository() : base()
        { }

        protected override string GetSelectStatment()
        {
            StringBuilder query = new StringBuilder();
            query.Append($"SELECT {TableAlias}.Id, {TableAlias}.Identification, {TableAlias}.UserName, {TableAlias}.Password, r.Id, r.Name ");
            query.Append($"FROM [{TableName}] AS {TableAlias} INNER JOIN [Roles] AS r ON (r.Id=u.RoleId) ");
            return query.ToString();
        }

        protected override User BuildNewEntity(object[] data, [CallerMemberName] string callerMethodName = null)
        {
            return new User()
            {
                Id = Convert.ToInt32(data[0]),
                Identication = data[1].ToString(),
                UserName = data[2].ToString(),
                Password = data[3].ToString(),
                Role = new Role()
                {
                    Id = Convert.ToInt32(data[4]),
                    Name = data[5].ToString()
                }
            };
        }
    }
}
