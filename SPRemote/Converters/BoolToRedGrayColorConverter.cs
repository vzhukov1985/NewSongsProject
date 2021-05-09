using System;
using System.Globalization;
using Xamarin.Forms;

namespace SPRemote.Converters
{
    public class BoolToRedGrayColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Color.FromHex("#990000") : Color.LightGray;

            return new Color(0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
