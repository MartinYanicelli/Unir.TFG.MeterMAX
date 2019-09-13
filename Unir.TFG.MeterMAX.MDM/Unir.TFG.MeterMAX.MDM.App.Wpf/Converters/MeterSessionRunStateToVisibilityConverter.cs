using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels.OnDemandMeterSessionViewModel;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Converters
{
    public class MeterSessionRunStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (parameter == null))
                return Visibility.Collapsed;

            MeterSessionRunStates meterSessionRunState = (MeterSessionRunStates)value;
            int option = System.Convert.ToInt32(parameter);
            Visibility result;
            switch (option)
            {
                case 0:
                    result = (meterSessionRunState == MeterSessionRunStates.Idle || meterSessionRunState == MeterSessionRunStates.Starting) ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case 1:
                    result = (meterSessionRunState != MeterSessionRunStates.Idle) ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case 2:
                    result = (meterSessionRunState == MeterSessionRunStates.Starting || meterSessionRunState == MeterSessionRunStates.Running) ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case 3:
                    result = (meterSessionRunState == MeterSessionRunStates.Running || meterSessionRunState == MeterSessionRunStates.Ended) ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case 4:
                    result = (meterSessionRunState == MeterSessionRunStates.Ended) ? Visibility.Visible : Visibility.Collapsed;
                    break;
                default:
                    result = Visibility.Collapsed;
                    break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
