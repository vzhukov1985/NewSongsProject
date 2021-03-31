using System;

namespace SPInfo.Models
{
    public interface ISettings
    {
        public string IP { get; set; }
        public bool ShowFullTrackName { get; set; }
        public bool ShowTrackKey { get; set; }
        public bool ReceiveTrackList { get; set; }
        public int TrackListShowTime { get; set; }
    }
}
