using Noanet.XamArch.Logic;
using Noanet.XamArch.Mvvm.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Unir.TFG.MeterMAX.MDM.Logic.Contracts;

namespace Unir.TFG.MeterMAX.MDM.Mvvm
{
    public class LogInViewModel : Noanet.XamArch.Mvvm.ViewModelBase
    {
        private string userName;
        private string password;
        private bool isUserNameValid;
        private bool isPasswordValid;
        
        private readonly IAccountManager accountManager;

        public ICommand LogOnCommand => new AsyncCommand(async () => await LogOnAsync());

        public bool IsNotAuthenticated => accountManager?.CurrentSession == null;
     
        public string UserName
        {
            get => userName;
            set => SetValue(ref userName, value);
        }

        public string Password
        {
            get => password;
            set => SetValue(ref password, value);
        }

        public bool IsUserNameValid
        {
            get => isUserNameValid;
            set => SetValue(ref isUserNameValid, value);
        }

        public bool IsPasswordValid
        {
            get => isPasswordValid;
            set => SetValue(ref isPasswordValid, value);
        }

        public LogInViewModel(IAccountManager accountManager) : base()
        {
            this.accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
        }

        private async Task LogOnAsync()
        {
            var operationResult = await TryExecuteAsync(accountManager.LogOnAsync(userName, password));
            if (operationResult.IsSuccess && (operationResult.Value.ValidationResult == ValidationResult.Success))
            {
                // autenticado correctamente, ir al home de la aplicación!
                OnPropertyChanged(nameof(IsNotAuthenticated));
            }
        }

        private void LogOff()
        {
            accountManager.LogOff();
            OnPropertyChanged(nameof(IsNotAuthenticated));
        }
    }
}
