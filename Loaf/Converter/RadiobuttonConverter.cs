using System;
using System.Globalization;
using System.Windows.Data;

namespace Loaf.Converter
{
    public class RadioButtonConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not int selectedValue || parameter is not string paramValue)
                return false;
            if (int.TryParse(paramValue, out var buttonValue))
                return selectedValue == buttonValue;
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool isChecked || !isChecked || parameter is not string paramValue)
                return 0;
            return int.TryParse(paramValue, out var buttonValue) ? buttonValue : 0;
        }
    }
}