using Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace NewSongsProject.Converters
{
    class PlaylistIndexCaptionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() == 2 && values[0] is TrackListItem && values[1] is ListView)
            {
                var item = (TrackListItem)values[0];
                var playlist = (ListView)values[1];

                return $"{playlist.Items.IndexOf(item) + 1}. {item.Caption}";
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
