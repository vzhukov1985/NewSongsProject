using NewSongsProject.Models;
using NewSongsProject.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.ViewModels
{
    public class TrackPropertiesDlgVM:BaseVM
    {
        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged("Caption");
            }
        }

        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged("FullName");
            }
        }

        private int _category;
        public int Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged("Category");
            }
        }

        private string _tags;
        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged("Tags");
            }
        }

        private int _timesOpened;
        public int TimesOpened
        {
            get { return _timesOpened; }
            set
            {
                _timesOpened = value;
                OnPropertyChanged("TimesOpened");
            }
        }

        private bool _isMaleVocalSelected;
        public bool IsMaleVocalSelected
        {
            get { return _isMaleVocalSelected; }
            set
            {
                _isMaleVocalSelected = value;
                if (value)
                    _vocalType = VocalType.Male;

                OnPropertyChanged("IsMaleVocalSelected");
            }
        }

        private bool _isFemaleVocalSelected;
        public bool IsFemaleVocalSelected
        {
            get { return _isFemaleVocalSelected; }
            set
            {
                _isFemaleVocalSelected = value;
                if (value)
                    _vocalType = VocalType.Female;
                OnPropertyChanged("IsFemaleVocalSelected");
            }
        }

        private bool _isDuetVocalSelected;
        public bool IsDuetVocalSelected
        {
            get { return _isDuetVocalSelected; }
            set
            {
                _isDuetVocalSelected = value;
                if (value)
                    _vocalType = VocalType.Duet;
                OnPropertyChanged("IsDuetVocalSelected");
            }
        }

        private bool _isLounge;
        public bool IsLounge
        {
            get { return _isLounge; }
            set
            {
                _isLounge = value;
                OnPropertyChanged("IsLounge");
            }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        private double _tempo;
        public double Tempo
        {
            get { return _tempo; }
            set
            {
                _tempo = value;
                OnPropertyChanged("Tempo");
            }
        }



        private VocalType _vocalType;
        public VocalType VocalType
        {
            get { return _vocalType; }
            set
            {
                _vocalType = value;
                switch (VocalType)
                {
                    case VocalType.Male: IsMaleVocalSelected = true; break;
                    case VocalType.Female: IsFemaleVocalSelected = true; break;
                    case VocalType.Duet: IsDuetVocalSelected = true; break;
                    default: break;
                }
                OnPropertyChanged("VocalType");
            }
        }


        public List<TrackCategory> CategoriesList { get; set; }

        private string TrackPath;
        private string oldCaption;
        public bool AreChangesWereMade { get; set; }
        private List<TrackListItem> allTracks;

        public RelayCommand ResetTimesOpenedCmd { get; set; }
        public RelayCommand FileNameCheckerCmd { get; set; }

        public TrackPropertiesDlgVM(AdditionalTrackInfo trackInfo, List<TrackCategory> trackCategories, List<TrackListItem> allTracks)
        {
            TrackPath = trackInfo.TrackPath;
            Caption = Path.GetFileNameWithoutExtension(trackInfo.TrackPath);
            FullName = trackInfo.FullName;
            Category = trackInfo.Category;
            VocalType = trackInfo.VocalType;
            IsLounge = trackInfo.IsLounge;
            Key = trackInfo.Key;
            Tempo = trackInfo.Tempo;
            Tags = string.Join(" ", trackInfo.Tags.ToArray());
            TimesOpened = trackInfo.TimesOpened;

            CategoriesList = trackCategories;

            this.allTracks = allTracks;
            oldCaption = Caption;

            ResetTimesOpenedCmd = new RelayCommand(_ => TimesOpened = 0);
            FileNameCheckerCmd = new RelayCommand(_ => { }, _ => !string.IsNullOrEmpty(Caption) && (allTracks.FirstOrDefault(t => t.Caption == Caption) == null || Caption == oldCaption));
        }

        public TrackPropertiesDlgVM()
        {

        }
    }
}
