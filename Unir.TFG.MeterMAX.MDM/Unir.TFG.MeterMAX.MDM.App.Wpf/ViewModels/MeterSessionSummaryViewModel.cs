using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.MDM.Domain;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class MeterSessionSummaryViewModel : Noanet.XamArch.Mvvm.ObservableObject
    {
        private MeterSession meterSession;
        private int quality;
        private bool hasResume;
        private int progressPercent;
        private string bytesSentAndReceived;
        private string resultMessage;

        public int Quality
        {
            get => quality;
            set => SetValue(ref quality, value);
        }

        public bool HasResume
        {
            get => hasResume;
            set => SetValue(ref hasResume, value);
        }

        public int ProgressPercent
        {
            get => progressPercent;
            set => SetValue(ref progressPercent, value);
        }
    
        public string BytesSentAndReceived
        {
            get => bytesSentAndReceived;
            set => SetValue(ref bytesSentAndReceived, value);
        }

        public MeterSession MeterSession
        {
            get => meterSession;
            set => SetValue(ref meterSession, value);
        }
        public string ResultMessage
        {
            get => resultMessage;
            set => SetValue(ref resultMessage, value);
        }
    }
}
