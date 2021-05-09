using System;
using System.Globalization;
using Xamarin.Forms;

namespace SPRemote.Converters
{
    public class BoolToLEDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? ImageSource.FromResource("SPRemote.Images.LEDGreen.png") : ImageSource.FromResource("SPRemote.Images.LEDRed.png");

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
