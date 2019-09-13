using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Services
{
    public interface INavigationService
    {
        string CurrentKey { get; }
        void NavigateTo(string pageKey);
        void NavigateTo(string pageKey, object parameter);
        void GoBack();
    }
}
