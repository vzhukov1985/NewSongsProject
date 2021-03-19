﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace SPRemote.Converters
{
    public class BoolToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? new SolidColorBrush(new Color(0, 200, 0)) : new SolidColorBrush(new Color(200, 0, 0));

            return new SolidColorBrush(new Color(0, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
