using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class TabItemViewModel : Noanet.XamArch.Mvvm.ViewModelBase
    {
        private string headerTitle;
        private string headerIconKind;
        private object content;

        public string HeaderTitle
        {
            get => headerTitle;
            set => SetValue(ref headerTitle, value);
        }

        public string HeaderIconKind {
            get => headerIconKind;
            set => SetValue(ref headerIconKind, value);
        }

        public object Content
        {
            get => content;
            set => SetValue(ref content, value);
        }
    }
}
