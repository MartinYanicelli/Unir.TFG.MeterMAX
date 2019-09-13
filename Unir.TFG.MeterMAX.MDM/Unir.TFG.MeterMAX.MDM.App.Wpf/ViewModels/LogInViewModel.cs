using Noanet.XamArch.Logic;
using Noanet.XamArch.Mvvm.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PubSub.Extension;
using Unir.TFG.MeterMAX.MDM.Logic.Contracts;
using System.Security;
using Unir.TFG.MeterMAX.MDM.App.Wpf.Extensions;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class LogInViewModel : Noanet.XamArch.Mvvm.ViewModelBase
    {
        private string userName;
        private SecureString password;
        private bool isUserNameValid;
        private bool isPasswordValid;
        
        private readonly IAccountManager accountManager;

        public ICommand LogOnCommand => new AsyncCommand(async () => await LogOnAsync());
        public ICommand CloseCommand => new Command(System.Windows.Application.Current.Shutdown);
        
        public string UserName
        {
            get => userName;
            set => SetValue(ref userName, value);
        }

        public SecureString Password
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
            var operationResult = await TryExecuteAsync(accountManager.LogOnAsync(userName, password.ConvertToString()));
            if (operationResult.IsSuccess)
            {
                if (operationResult.Value.ValidationResult == ValidationResult.Success)
                {
                    // autenticado correctamente, ir al home de la aplicación!
                    this.Publish(operationResult.Value);
                }
                else
                {
                    await dialogService.ShowValidationMessage(operationResult.Value.ValidationResult.ErrorMessage, "¡ATENCIÓN!");
                }
            }
        }
        
    }
}
