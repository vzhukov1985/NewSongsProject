using NewSongsProject.Models;
using NewSongsProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace NewSongsProject.ViewModels
{
    public class AppSettingsDlgVM: BaseVM
    {
        private List<TrackCategory> _categories;
        public List<TrackCategory> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        public RelayCommand ShowSelectColorDialogCmd { get; set; }

        public AppSettingsDlgVM(AppSettings appSettings, List<TrackCategory> trackCategories)
        {
            Categories = trackCategories;

            ShowSelectColorDialogCmd = new RelayCommand((cat) => ChangeCategoryColor((TrackCategory)cat));
        }

        private void ChangeCategoryColor(TrackCategory category)
        {

            using (ColorDialog clrDialog = new ColorDialog())
            {
                if (clrDialog.ShowDialog() == DialogResult.OK)
                {
                    category.Color = Color.FromArgb(clrDialog.Color.A, clrDialog.Color.R, clrDialog.Color.G, clrDialog.Color.B);
                    Categories = Categories.ToList();
                }
            }
        }


        public AppSettingsDlgVM()
        {
        }
    }
}
