using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Models
{
    public class TrackListItem
    {
        public string Caption { get; set; }
        public bool IsDirectory { get; set; }
        public string FullPath { get; set; }
        public int Category { get; set; }
        public VocalType VocalType { get; set; }
        public bool IsLounge { get; set; }
        public List<string> Tags { get; set; }
        public int TimesOpened { get; set; }
    }
}
