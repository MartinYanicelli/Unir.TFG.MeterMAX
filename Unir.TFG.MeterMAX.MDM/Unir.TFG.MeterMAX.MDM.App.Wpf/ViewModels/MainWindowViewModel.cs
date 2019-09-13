using Noanet.XamArch.Mvvm.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PubSub.Extension;
using System.Windows.Input;
using Unir.TFG.MeterMAX.MDM.App.Wpf.Views;
using Unir.TFG.MeterMAX.MDM.Logic.Contracts;
using System.Windows;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class MainWindowViewModel : Noanet.XamArch.Mvvm.ViewModelBase
    {
        private MainWindowItemViewModel[] items;
        private readonly IAccountManager accountManager;

        public bool IsAuthenticated => accountManager.CurrentSession != null;
        public string UserAuthenticated => accountManager.CurrentSession?.User?.UserName;


        public ICommand LogOffCommand => new Command(LogOff, () => !IsBusy);
        public ICommand CloseCommand => new AsyncCommand(async () => await CloseAsync(), () => !IsBusy);
        
        public MainWindowItemViewModel[] Items
        {
            get => items;
            set => SetValue(ref items, value);
        }

        public MainWindowViewModel(IAccountManager accountManager)
        {
            this.accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));

            if (!IsAuthenticated)
                throw new InvalidOperationException("El Usuario no está autenticado!");

            Items = new[]
                {
                    new MainWindowItemViewModel("HomeOutline", "Inicio", new HomeView()),
                    new MainWindowItemViewModel("Cogs", "Herramientas y Servicios", new Views.ToolsAndServices.HomeView()) { MarginRequirement = new Thickness(0) },
                    new MainWindowItemViewModel("EqualiserVertical", "Configuraciones", new Views.Settings.HomeView()),
                    new MainWindowItemViewModel("ChartBar", "Reportes", new Views.Reports.HomeView())
                };
        }

        private void LogOff()
        {
            Logout();
            // sesión cerrada, ir al login de la aplicación!
            this.Publish(new Logic.Validations.AuthenticationResult() { UserSession = accountManager.CurrentSession });
        }

        private async Task CloseAsync()
        {
            var result = await ((Services.DialogService)dialogService).PromptMessage("¿Está seguro que desea salir de la Aplicación?", "ATENCIÓN", null);
            if (result)
            {
                Logout();
                // sesión cerrada, ir al login de la aplicación!
                this.Publish("ShutdownRequest");
            }
        }

        private void Logout()
        {
            accountManager.LogOff();
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(UserAuthenticated));
        }
    }
}
