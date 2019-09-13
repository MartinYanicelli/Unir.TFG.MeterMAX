using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.MDM.Domain;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class MeterSessionProgressViewModel : Noanet.XamArch.Mvvm.ObservableObject
    {
        private MeterSessionTask meterSessionTask;
        private int progressPercent;
        private TimeSpan elapsedTime;
        
        public MeterSessionTask MeterSessionTask
        {
            get => meterSessionTask;
            set => SetValue(ref meterSessionTask, value);
        }

        public int ProgressPercent
        {
            get => progressPercent;
            set => SetValue(ref progressPercent, value);
        }

        public TimeSpan ElapsedTime
        {
            get => elapsedTime;
            set => SetValue(ref elapsedTime, value);
        }
    }
}
