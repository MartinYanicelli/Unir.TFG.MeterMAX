using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Converters
{
    class MeterSessionQualityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result = Brushes.Red;

            if (value is int quality)
            {
                result = (quality <= 2) ? Brushes.Red : (quality >= 2) ? Brushes.DarkOrange : (quality <= 4) ? Brushes.Green : Brushes.Purple;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            if (value is Brush brush)
            {
                result = (brush == Brushes.Red) ? 0 : (brush == Brushes.DarkOrange) ? 2 : (brush == Brushes.Green) ? 4 : (brush == Brushes.Purple) ? 5 : 0;
            }
            return result;
        }
    }
}
