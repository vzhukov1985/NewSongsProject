using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Models
{
    public class AppSettings
    {
        public string InitialPath { get; set; }
        public int MainWindowX { get; set; }
        public int MainWindowY { get; set; }
        public int MainWindowHeight { get; set; }
        public int MainWindowWidth { get; set; }
        public double MainWindowOpacity { get; set; }
        public bool IsMainWindowMaximized { get; set; }
        public bool AreTracksColored { get; set; }

        public int TrackListFontSize { get; set; }

        public List<TrackCategory> TrackCategories { get; set; }

        public AppSettings()
        {
            InitialPath = "D:\\";
            MainWindowX = 0;
            MainWindowY = 0;
            MainWindowHeight = 450;
            MainWindowWidth = 800;
            TrackListFontSize = 14;
            MainWindowOpacity = 1;
            IsMainWindowMaximized = false;
            AreTracksColored = true;

            TrackCategories = new List<TrackCategory>()
                {
                    new TrackCategory() {Name = "Без категории", IsChangeable = false},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                    new TrackCategory() {Name = "Нет"},
                };
        }
    }
}
