using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Services
{
    public class DialogService : IDialogService
    {
        private readonly Notifier notifier;
        public DialogService()
        {
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 40,
                    offsetY: 40);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public async Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                notifier.ShowError(message, new ToastNotifications.Core.MessageOptions() { FreezeOnMouseEnter = true, UnfreezeOnMouseLeave = true, ShowCloseButton = true });
                afterHideCallback?.Invoke();
            });
        }

        public async Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            await ShowError(error.Message, null, null, null);
        }

        public Task ShowMessage(string message, string title)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessageBox(string message, string title)
        {
            throw new NotImplementedException();
        }

        public async Task ShowValidationMessage(string message, string title)
        {
            await ShowValidationMessage(message, null, "ACEPTAR", null);
        }

        public async Task ShowValidationMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                notifier.ShowWarning(message, new ToastNotifications.Core.MessageOptions() { ShowCloseButton = true, UnfreezeOnMouseLeave = true, FreezeOnMouseEnter = true });
                afterHideCallback?.Invoke();
            });
        }

        public async Task<bool> PromptMessage(string message, string title, Action callBackAction)
        {
            return (bool) await MaterialDesignThemes.Wpf.DialogHost.Show(new Controls.PromptDialogBox() { Caption = message }.Content);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    notifier.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DialogService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
