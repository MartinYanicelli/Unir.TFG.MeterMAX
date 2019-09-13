using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Services
{
    public interface IDialogService : Noanet.XamArch.Mvvm.Services.IDialogService, IDisposable
    {
        Task<bool> PromptMessage(string message, string title, Action callBackAction);
    }
}
