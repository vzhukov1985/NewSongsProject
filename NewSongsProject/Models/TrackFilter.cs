using NewSongsProject.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Models
{
    public class TrackFilter: ObservableCollection<Wrapper<bool>>
    {
        public List<int> FilteredList
        {
            get
            {
                List<int> result = new List<int>();
                int ind = 0;
                foreach(var i in this)
                {
                    if (i) result.Add(ind);
                    ind++;
                }
                return result;
            }
        }
    }
}
