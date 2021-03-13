using NewSongsProject.Models;
using NewSongsProject.ViewModels;
using NewSongsProject.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Services
{
    public static class DialogService
    {
        public static AdditionalTrackInfo ShowTrackPropertiesDlg(AdditionalTrackInfo trackInfo, List<TrackCategory> trackCategories)
        {
            var dlgVM = new TrackPropertiesDlgVM(trackInfo, trackCategories);
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
    }
}
