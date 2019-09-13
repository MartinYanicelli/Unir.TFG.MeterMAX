using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.MDM.Domain;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class MeterSesstionTypeViewModel : Noanet.XamArch.Mvvm.ObservableObject
    {
        private MeterSessionType meterSessionType;
        private string iconKind;

        public MeterSessionType MeterSessionType
        {
            get => meterSessionType;
            set => SetValue(ref meterSessionType, value);
        }

        public string IconKind
        {
            get => iconKind;
            set => SetValue(ref iconKind, value);
        }
    }
}
