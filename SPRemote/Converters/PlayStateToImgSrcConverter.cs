using System;
using System.Globalization;
using SPRemote.Models;
using Xamarin.Forms;

namespace SPRemote.Converters
{
    public class PlayStateToImgSrcConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PlayState)
            {
                switch((PlayState)value)
                {
                    case PlayState.None: return null;
                    case PlayState.Playing: return ImageSource.FromResource("SPRemote.Images.Stop.png");
                    case PlayState.Stopped: return ImageSource.FromResource("SPRemote.Images.Play.png"); 
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
