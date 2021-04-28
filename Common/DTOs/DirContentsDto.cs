using System;
using System.Collections.Generic;
using Common.Models;

namespace Common.DTOs
{
    public class DirContentsDto
    {
        public List<TrackListItem> TrackListItems { get; set; }
        public bool IsTopDir { get; set; }
        public string UpDirPath { get; set; }
    }
}
