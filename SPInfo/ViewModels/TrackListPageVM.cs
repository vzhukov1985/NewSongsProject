using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;
using SPInfo.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SPInfo.ViewModels
{
    public class TrackListPageVM: BaseVM
    {
        public List<TrackListItem> TrackList { get; set; }

        private TrackListItem selectedTrack;
        public TrackListItem SelectedTrack
        {
            get
            {
                return selectedTrack;
            }
            set
            {
                if (value != null)
                {
                    selectedTrack = value;
                    PageService.GoBack();
                }
            }
        }

        public TrackListPageVM()
        {

        }

        public TrackListPageVM(List<TrackListItem> tracklist)
        {
            SelectedTrack = null;
            TrackList = tracklist;
        }
    }
}
