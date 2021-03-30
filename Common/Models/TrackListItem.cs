using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TrackListItem
    {
        public string Caption { get; set; }
        public bool IsDirectory { get; set; }
        public string FullPath { get; set; }
        public string FullName { get; set; }
        public int Category { get; set; }
        public VocalType VocalType { get; set; }
        public bool IsLounge { get; set; }
        public string Key { get; set; }
        public double Tempo { get; set; }
        public List<string> Tags { get; set; }
        public int TimesOpened { get; set; }

        public TrackListItem()
        {
            Tags = new List<string>();
        }

        public TrackListItem(TrackListItem source)
        {
            Caption = source.Caption;
            IsDirectory = source.IsDirectory;
            FullPath = source.FullPath;
            Category = source.Category;
            VocalType = source.VocalType;
            IsLounge = source.IsLounge;
            Tags = source.Tags;
            TimesOpened = source.TimesOpened;
            Tempo = 0;
        }
    }
}
