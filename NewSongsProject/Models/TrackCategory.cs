using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewSongsProject.Models
{
    public class TrackCategory: ICloneable
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public bool IsChangeable { get; set; }

        public TrackCategory()
        {
            Color = Color.FromRgb(0, 0, 0);
            IsChangeable = true;
        }

        public object Clone()
        {
            return new TrackCategory()
            {
                Color = this.Color,
                Name = this.Name,
                IsChangeable = this.IsChangeable
            };
        }
    }
}
