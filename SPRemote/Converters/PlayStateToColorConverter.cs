using System;
using System.Globalization;
using SPRemote.Models;
using Xamarin.Forms;

namespace SPRemote.Converters
{
    public class PlayStateToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PlayState)
            {
                switch ((PlayState)value)
                {
                    case PlayState.None: return Color.LightGray;
                    case PlayState.Playing: return Color.FromHex("#009900");
                    case PlayState.Stopped: return Color.FromHex("#999900");
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
