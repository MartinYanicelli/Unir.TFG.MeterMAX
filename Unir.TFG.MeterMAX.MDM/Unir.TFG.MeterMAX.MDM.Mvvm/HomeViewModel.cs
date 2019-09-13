using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Unir.TFG.MeterMAX.MDM.Logic.Contracts;

namespace Unir.TFG.MeterMAX.MDM.Mvvm.ViewModels
{
    public class HomeViewModel : Noanet.XamArch.Mvvm.ViewModelBase
    {
        private readonly IAccountManager accountManager;

        public ICommand LogOffCommand => new Noanet.XamArch.Mvvm.Command.Command(LogOff, () => !IsBusy);

        public HomeViewModel(IAccountManager accountManager) : base()
        {
            this.accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
        }

        private void LogOff()
        {
            TryExecute(accountManager.LogOff);
        }
    }
}
