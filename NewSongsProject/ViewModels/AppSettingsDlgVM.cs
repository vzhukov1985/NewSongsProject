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

        private string _cakewalkPath;
        public string CakewalkPath
        {
            get { return _cakewalkPath; }
            set
            {
                _cakewalkPath = value;
                OnPropertyChanged("CakewalkPath");
            }
        }

        private string _projectsPath;
        public string ProjectsPath
        {
            get { return _projectsPath; }
            set
            {
                _projectsPath = value;
                OnPropertyChanged("ProjectsPath");
            }
        }


        private int _fontSize;
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        private int _opacity;
        public int Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                OnPropertyChanged("Opacity");
            }
        }

        private bool _isWindowSizeResetted;
        public bool IsWindowSizeResetted
        {
            get { return _isWindowSizeResetted; }
            set
            {
                _isWindowSizeResetted = value;
                OnPropertyChanged("IsWindowSizeResetted");
            }
        }


        public RelayCommand ShowSelectColorDialogCmd { get; set; }
        public RelayCommand BrowseCakewalkCmd { get; set; }
        public RelayCommand BrowseProjectsPathCmd { get; set; }
        public RelayCommand ResetWindowSizeCmd { get; set; }

        public AppSettingsDlgVM(AppSettings appSettings)
        {
            CakewalkPath = appSettings.CakewalkPath;
            ProjectsPath = appSettings.ProjectsPath;
            FontSize = appSettings.TrackListFontSize;
            Opacity = (int)(appSettings.MainWindowOpacity * 100);
            IsWindowSizeResetted = false;

            Categories = new List<TrackCategory>();
            foreach (var cat in appSettings.TrackCategories)
            {
                Categories.Add((TrackCategory)cat.Clone());
            }

            ShowSelectColorDialogCmd = new RelayCommand((cat) => ChangeCategoryColor((TrackCategory)cat));
            BrowseCakewalkCmd = new RelayCommand(_ => BrowseCakewalk());
            BrowseProjectsPathCmd = new RelayCommand(_ => BrowseProjectsPath());
            ResetWindowSizeCmd = new RelayCommand(_ => ResetWindowSize());
        }

        private void ResetWindowSize()
        {
            IsWindowSizeResetted = true;
        }

        private void BrowseCakewalk()
        {
            using (OpenFileDialog cwDlg = new OpenFileDialog() { CheckFileExists = true, DefaultExt = ".exe", Filter = "Cakewalk Application Executable (*.exe)|*.exe"})
            {
                if (cwDlg.ShowDialog() == DialogResult.OK)
                {
                    CakewalkPath = cwDlg.FileName;
                }
            }
        }

        private void BrowseProjectsPath()
        {
            using (var dlgFolder = new FolderBrowserDialog())
            {
                if (dlgFolder.ShowDialog() == DialogResult.OK)
                {
                    ProjectsPath = dlgFolder.SelectedPath;
                }
            }
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
