using System;
using System.Globalization;
using System.Windows.Data;

namespace GPM.Converters
{
    public class SubtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double originalValue && parameter is string paramString && double.TryParse(paramString, out double subtractValue))
            {
                return originalValue - subtractValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}