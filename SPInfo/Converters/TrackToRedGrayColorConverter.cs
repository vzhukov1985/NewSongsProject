using System;
using System.Globalization;
using Common.Models;
using Xamarin.Forms;

namespace SPInfo.Converters
{
    public class TrackToRedGrayColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrackListItem)
            {
                return ((TrackListItem)value).Caption == "Нет" ? Color.DarkGray : Color.Red;
            }
            return Color.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
