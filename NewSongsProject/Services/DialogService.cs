using NewSongsProject.Models;
using NewSongsProject.ViewModels;
using NewSongsProject.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewSongsProject.Services
{
    public static class DialogService
    {
        public static string FilePath { get; set; }

        public static AdditionalTrackInfo ShowTrackPropertiesDlg(AdditionalTrackInfo trackInfo, List<TrackCategory> trackCategories, List<TrackListItem> allTracks)
        {
            var dlgVM = new TrackPropertiesDlgVM(trackInfo, trackCategories, allTracks);
            var dlg = new TrackPropertiesDlg();
            dlg.DataContext = dlgVM;
            if ((bool)dlg.ShowDialog())
            {
                return new AdditionalTrackInfo()
                {
                    TrackPath = $"{Path.GetDirectoryName(trackInfo.TrackPath)}\\{dlgVM.Caption}.cwp",
                    FullName = dlgVM.FullName,
                    Category = dlgVM.Category,
                    VocalType = dlgVM.VocalType,
                    IsLounge = dlgVM.IsLounge,
                    Tags = dlgVM.Tags.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToList(),
                    TimesOpened = dlgVM.TimesOpened
                };
            }
            else
            {
                return null;
            }
        }

        public static void ShowAppSettingsDlg(AppSettings appSettings, List<TrackCategory> trackCategories)
        {
            var dlgVM = new AppSettingsDlgVM(appSettings, trackCategories.ToList());
            var dlg = new AppSettingsDlg();
            dlg.DataContext = dlgVM;
            if (dlg.ShowDialog() == true)
            {
               // a
            }
        }

        public static bool ShowOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Hollywood Playlist| *.hpl";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public static bool ShowSaveFileDialog()
        {
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.Filter = "Hollywood Playlist| *.hpl";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public static void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }
    }
}
