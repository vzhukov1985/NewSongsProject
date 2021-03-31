using System;
namespace SPInfo.Models
{
    public class GlobalStates:IGlobalStates
    {
        public bool IsTrackListPageShowing { get; set; }

        public GlobalStates()
        {
            IsTrackListPageShowing = false;
        }
    }
}
