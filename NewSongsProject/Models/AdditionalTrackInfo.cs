using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Models
{
    public class AdditionalTrackInfo
    {
        public string TrackPath { get; set; }
        public string FullName { get; set; }
        public int Category { get; set; }
        public VocalType VocalType { get; set; }
        public bool IsLounge { get; set; }
        public string Key { get; set; }
        public double Tempo { get; set; }
        public List<string> Tags { get; set; }
        public int TimesOpened { get; set; }

        public AdditionalTrackInfo()
        {
            Category = 0;
            Tags = new List<string>();
            TimesOpened = 0;
            Tempo = 0;
            VocalType = VocalType.Male;
        }
    }
}
