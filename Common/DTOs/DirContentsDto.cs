using System;
using System.Collections.Generic;
using Common.Models;

namespace Common.DTOs
{
    public class DirContentsDto
    {
        List<TrackListItem> TrackListItems { get; set; }
        bool IsTopDir { get; set; }
        string UpDirPath { get; set; }
    }
}
