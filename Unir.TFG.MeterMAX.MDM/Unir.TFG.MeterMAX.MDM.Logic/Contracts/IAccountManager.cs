using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Logic.Validations;

namespace Unir.TFG.MeterMAX.MDM.Logic.Contracts
{
    public interface IAccountManager
    {
        UserSession CurrentSession { get; }
        Task<AuthenticationResult> LogOnAsync(string userName, string userPassword);
        void LogOff();
    }
}
