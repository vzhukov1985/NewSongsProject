using NewSongsProject.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NewSongsProject.Converters
{
    public class TrackCategoryToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
           if ((bool)values[0])
            {
                return new SolidColorBrush(((List<TrackCategory>)values[2])[(int)values[1]].Color);
            }
           else
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
