using System;
using System.Globalization;
using SPRemote.Models;
using Xamarin.Forms;

namespace SPRemote.Converters
{
    public class PlayStateToBoolConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PlayState)
            {
                if ((PlayState)value == PlayState.None)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
