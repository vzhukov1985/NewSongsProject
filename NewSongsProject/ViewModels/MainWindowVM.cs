using NewSongsProject.Models;
using NewSongsProject.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NewSongsProject.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        private int _mainWindowX;
        public int MainWindowX
        {
            get { return _mainWindowX; }
            set
            {
                _mainWindowX = value;
                OnPropertyChanged("MainWindowX");
            }
        }

        private int _mainWindowY;
        public int MainWindowY
        {
            get { return _mainWindowY; }
            set
            {
                _mainWindowY = value;
                OnPropertyChanged("MainWindowY");
            }
        }


        private int _mainWindowHeight;
        public int MainWindowHeight
        {
            get { return _mainWindowHeight; }
            set
            {
                _mainWindowHeight = value;
                OnPropertyChanged("MainWindowHeight");
            }
        }

        private int _mainWindowWidth;
        public int MainWindowWidth
        {
            get { return _mainWindowWidth; }
            set
            {
                _mainWindowWidth = value;
                OnPropertyChanged("MainWindowWidth");
            }
        }


        private List<TrackListItem> _trackList;
        public List<TrackListItem> TrackList
        {
            get { return _trackList; }
            set
            {
                _trackList = value;
                OnPropertyChanged("TrackList");
            }
        }

        private TrackListItem _selectedTrackListItem;
        public TrackListItem SelectedTrackListItem
        {
            get { return _selectedTrackListItem; }
            set
            {
                _selectedTrackListItem = value;
                OnPropertyChanged("SelectedTrackListItem");
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        private bool _preformanceMode;
        public bool PerformanceMode
        {
            get { return _preformanceMode; }
            set
            {
                _preformanceMode = value;
                OnPropertyChanged("PerformanceMode");
            }
        }

        private string _openedTrack;
        public string OpenedTrack
        {
            get { return _openedTrack; }
            set
            {
                if (PerformanceMode && _openedTrack.Equals("Нет") && !value.Equals("Нет"))
                {
                    PlayStop();
                }

                if (_openedTrack == null || !_openedTrack.Equals(value))
                {
                    _openedTrack = value;

                    OnPropertyChanged("OpenedTrack");
                }
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged("IsPlaying");
                }
            }
        }

        private int _trackListFontSize;
        public int TrackListFontSize
        {
            get { return _trackListFontSize; }
            set
            {
                _trackListFontSize = value;
                OnPropertyChanged("TrackListFontSize");
            }
        }

        private TrackFilter _categoriesFilter;
        public TrackFilter CategoriesFilter
        {
            get { return _categoriesFilter; }
            set
            {
                _categoriesFilter = value;
                OnPropertyChanged("CategoriesFilter");
            }
        }

            private TrackFilter _VocalsFilter;
            public TrackFilter VocalsFilter
            {
                get { return _VocalsFilter; }
                set
                {
                    _VocalsFilter = value;
                    OnPropertyChanged("VocalsFilter");
                }
            }

        private bool _loungeFilter;
        public bool LoungeFilter
        {
            get { return _loungeFilter; }
            set
            {
                _loungeFilter = value;
                if (string.IsNullOrEmpty(SearchText)) ProcessSearchAsync();
                OnPropertyChanged("LoungeFilter");
            }
        }




        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        static extern int GetMenuString(IntPtr hMenu, uint uIDItem, [Out] StringBuilder lpString, int nMaxCount, uint uFlag);

        [DllImport("user32.dll")]
        static extern IntPtr GetSubMenu(IntPtr hMenu, uint nPos);

        [DllImport("user32.dll")]
        static extern uint GetMenuItemID(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, UInt32 wMsg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        AppSettings appSettings;
        private string currentPath;
        private List<TrackListItem> allTracksList;
        private List<AdditionalTrackInfo> additionalTrackInfos;
        public List<TrackCategory> CategoriesList { get; set; }

        private IntPtr mainWndHandle;


        private Timer tmrOpenedTrack;
        private Timer tmrFocus;
        private int tmrFocusCounter;

        private Timer tmrPlayStatusChecker;
        private RECT oldHUDRect, newHUDRect;

        private Timer tmrAlwaysSelectionChecker;

        private FileSystemWatcher dirWatcher;

        private SemaphoreSlim searchSmph;

        public RelayCommand SelectFirstTrackCmd { get; set; }
        public RelayCommand SelectLastTrackCmd { get; set; }
        public RelayCommand ProcessTrackListItemCmd { get; set; }
        public RelayCommand UpFolderCmd { get; set; }
        public RelayCommand SaveSettingsCmd { get; set; }
        public RelayCommand AddSymbolSearchCmd { get; set; }
        public RelayCommand RemoveSymbolSearchCmd { get; set; }
        public RelayCommand ClearSearchCmd { get; set; }
        public RelayCommand PlayStopCmd { get; set; }
        public RelayCommand ShowTrackPropertiesCmd { get; set; }
        public RelayCommand ShowAppSettingsCmd { get; set; }
        public RelayCommand AlterCategoryFilterCmd { get; set; }
        public RelayCommand AlterVocalsFilterCmd { get; set; }

        public MainWindowVM()
        {
            searchSmph = new SemaphoreSlim(1);
            additionalTrackInfos = new List<AdditionalTrackInfo>();
            LoadAppSettings();

            CategoriesFilter = new TrackFilter()
            {
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
            };
            foreach(var i in CategoriesFilter)
            {
                i.PropertyChanged += (s,e) => ProcessSearchAsync();
            }

            VocalsFilter = new TrackFilter() { true, true, true };
            foreach (var i in VocalsFilter)
            {
                i.PropertyChanged += (s, e) => ProcessSearchAsync();
            }


            SelectFirstTrackCmd = new RelayCommand(_ => SelectedTrackListItem = TrackList.FirstOrDefault(), _ => TrackList.IndexOf(SelectedTrackListItem) > 0);
            SelectLastTrackCmd = new RelayCommand(_ => SelectedTrackListItem = TrackList.LastOrDefault(), _ => TrackList.IndexOf(SelectedTrackListItem) < TrackList.Count - 1);
            ProcessTrackListItemCmd = new RelayCommand(_ => ProcessTrackListItem(), _ => SelectedTrackListItem != null);
            UpFolderCmd = new RelayCommand(_ => ChangeDirectoryAsync(Directory.GetParent(currentPath).FullName), _ => Directory.GetParent(currentPath) != null);
            SaveSettingsCmd = new RelayCommand(_ => SaveAppSettings());
            AddSymbolSearchCmd = new RelayCommand((symbol) => { SearchText += (string)symbol; ProcessSearchAsync(); });
            RemoveSymbolSearchCmd = new RelayCommand(_ => { SearchText = SearchText.Substring(0, SearchText.Length - 1); ProcessSearchAsync(); }, _ => SearchText.Length > 0);
            ClearSearchCmd = new RelayCommand(_ => { SearchText = ""; ProcessSearchAsync(); });
            PlayStopCmd = new RelayCommand(_ => PlayStop());
            ShowTrackPropertiesCmd = new RelayCommand(_ => EditTrackProperties(), _ => SelectedTrackListItem != null && SelectedTrackListItem.IsDirectory == false);
            ShowAppSettingsCmd = new RelayCommand(_ => ShowAppSettings(), _ => PerformanceMode == false);
            AlterCategoryFilterCmd = new RelayCommand((ind) => AlterCategoryFilter(int.Parse((string)ind)));
            AlterVocalsFilterCmd = new RelayCommand((ind) => AlterVocalsFilter((int)ind));

            tmrOpenedTrack = new Timer(_ => GetOpenedTrackName(), null, 0, 200);
            tmrPlayStatusChecker = new Timer(_ => PlayStatusCheck(), null, 0, 200);
            tmrAlwaysSelectionChecker = new Timer(_ => { if (TrackList != null && TrackList.Count > 0 && SelectedTrackListItem == null) SelectedTrackListItem = TrackList[0]; }, null, 0, 100);

            dirWatcher = new FileSystemWatcher(currentPath) { NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName, IncludeSubdirectories = false };
            dirWatcher.Created += (s, e) => DirContentsChangedAsync(e.FullPath);
            dirWatcher.Deleted += (s, e) => DirContentsChangedAsync(e.FullPath);
            dirWatcher.Renamed += (s, e) => DirContentsChangedAsync(e.FullPath);
            dirWatcher.EnableRaisingEvents = true;

            ChangeDirectoryAsync(currentPath);
        }

        private void AlterCategoryFilter(int category)
        {
            if (category > -1)
            {
                if (CategoriesFilter.FilteredList.Count == 10)
                {
                    for (int i = 0; i < CategoriesFilter.Count; i++)
                    {
                        CategoriesFilter[i] = i == category ? true : false;
                    }
                }
                else
                {
                    CategoriesFilter[category] = !CategoriesFilter[category];
                }
            }
            else
            {
                for (int i = 0; i < CategoriesFilter.Count; i++)
                {
                    CategoriesFilter[i] = true;
                }
            }

            if (CategoriesFilter.FilteredList.Count == 0)
            {
                AlterCategoryFilter(-1);
                return;
            }

            if (string.IsNullOrEmpty(SearchText)) ProcessSearchAsync();
        }
       
        private void AlterVocalsFilter(int vocalsType)
        {
            if (vocalsType > -1)
            {
                if (VocalsFilter.FilteredList.Count == 3)
                {
                    for (int i = 0; i < VocalsFilter.Count; i++)
                    {
                        VocalsFilter[i] = i == vocalsType ? true : false;
                    }
                }
                else
                {
                    VocalsFilter[vocalsType] = !VocalsFilter[vocalsType];
                }
            }
            else
            {
                for (int i = 0; i < VocalsFilter.Count; i++)
                {
                    VocalsFilter[i] = true;
                }
            }

            if (VocalsFilter.FilteredList.Count == 0)
            {
                AlterVocalsFilter(-1);
                return;
            }

            if (string.IsNullOrEmpty(SearchText)) ProcessSearchAsync();
        }


        private void ShowAppSettings()
        {
            DialogService.ShowAppSettingsDlg(appSettings, CategoriesList);
        }

        private async void DirContentsChangedAsync(string path)
        {
            await Task.Run(() =>
            {
                if (Path.GetFileName(path).Equals("MixScenes") || (!File.GetAttributes(path).HasFlag(FileAttributes.Directory) && !Path.GetExtension(path).Equals(".cwp"))) return;

                var selFilePath = SelectedTrackListItem.FullPath;
                RefreshTrackList();
                SelectedTrackListItem = allTracksList.FirstOrDefault(t => t.FullPath == selFilePath);
            });
        }

        private void EditTrackProperties()
        {
            var trackInfo = additionalTrackInfos.FirstOrDefault(t => t.TrackPath == SelectedTrackListItem.FullPath);
            if (trackInfo == null)
            {
                trackInfo = new AdditionalTrackInfo()
                {
                    TrackPath = SelectedTrackListItem.FullPath,
                    Category = 0,
                    TimesOpened = 0
                };
            }

            var updatedInfo = DialogService.ShowTrackPropertiesDlg(trackInfo, CategoriesList, allTracksList);

            if (updatedInfo == null)
                return;

            var existingTrackInfoId = additionalTrackInfos.FindIndex(t => t.TrackPath == SelectedTrackListItem.FullPath);

            if (existingTrackInfoId != -1)
            {
                additionalTrackInfos[existingTrackInfoId] = updatedInfo;
            }
            else
            {
                additionalTrackInfos.Add(updatedInfo);
            }

            var oldSelectedTrackListItemPath = SelectedTrackListItem.FullPath;

            if (!oldSelectedTrackListItemPath.Equals(updatedInfo.TrackPath))
            {
                dirWatcher.EnableRaisingEvents = false;
                File.Move(oldSelectedTrackListItemPath, updatedInfo.TrackPath);
                dirWatcher.EnableRaisingEvents = true;
                SelectedTrackListItem = TrackList.FirstOrDefault(t => t.FullPath == updatedInfo.TrackPath);
            }

            RefreshTrackList();
            ProcessSearchAsync(updatedInfo.TrackPath);
            SaveAppSettings();
        }

        private void GetOpenedTrackName()
        {
            var wndHandle = FindWindow("Cakewalk Core", null);

            if (wndHandle == IntPtr.Zero)
            {
                OpenedTrack = "Нет";
                return;
            }

            var length = GetWindowTextLength(wndHandle) + 1;
            var title = new StringBuilder(length);
            GetWindowText(wndHandle, title, length);

            if (title.ToString().Equals("Cakewalk"))
            {
                OpenedTrack = "Нет";
                return;
            }

            var fullCaption = title.ToString();


            try
            {
                OpenedTrack = fullCaption.Substring(fullCaption.IndexOf('[') + 1, fullCaption.IndexOf(" - Track") - 1 - fullCaption.IndexOf('['));
            }
            catch
            {
                OpenedTrack = "Ошибка";
            }
        }

        private void PlayStatusCheck()
        {
            var cursorHandle = FindWindow(null, "HUDNowTime");
            if (cursorHandle == IntPtr.Zero)
            {
                IsPlaying = false;
                return;
            }

            GetWindowRect(cursorHandle, out oldHUDRect);
            Thread.Sleep(150);
            GetWindowRect(cursorHandle, out newHUDRect);
            if (newHUDRect.Left != oldHUDRect.Left)
            {
                IsPlaying = true;
            }
            else
            {
                IsPlaying = false;
            }
        }

        private async void ChangeDirectoryAsync(string path)
        {
            dirWatcher.Path = path;
            currentPath = path;
            await Task.Run(() => RefreshTrackList());
            SearchText = "";
            ProcessSearchAsync();
        }

        private void RefreshTrackList()
        {
            try
            {
                var items = Directory.GetDirectories(currentPath).Where(d => !Path.GetFileName(d).Equals("MixScenes") && Path.GetFileName(d) != "Audio").Select(d => new TrackListItem()
                {
                    Caption = Path.GetFileName(d),
                    IsDirectory = true,
                    FullPath = d
                })
                    .OrderBy(d => d.Caption)
                    .ToList();

                items.AddRange(Directory.GetFiles(currentPath, "*.cwp").Select(f => new TrackListItem()
                {
                    Caption = Path.GetFileNameWithoutExtension(f),
                    IsDirectory = false,
                    FullPath = f
                }));

                foreach (var item in items)
                {
                    var existingInfo = additionalTrackInfos.FirstOrDefault(i => i.TrackPath == item.FullPath);
                    if (existingInfo != null)
                    {
                        item.Category = existingInfo.Category;
                        item.Tags = existingInfo.Tags;
                        item.TimesOpened = existingInfo.TimesOpened;
                        item.VocalType = existingInfo.VocalType;
                        item.IsLounge = existingInfo.IsLounge;
                    }
                }

                allTracksList = items.OrderByDescending(t => t.IsDirectory).ThenByDescending(t => t.TimesOpened).ThenBy(t =>t.Caption).ToList();
            }
            catch
            {
                return;
            }
        }

        private void ProcessTrackListItem()
        {
            if (SelectedTrackListItem == null)
                return;

            if (SelectedTrackListItem.IsDirectory)
            {
                ChangeDirectoryAsync(SelectedTrackListItem.FullPath);
            }
            else
            {
                if (Path.GetExtension(SelectedTrackListItem.FullPath) == ".cwp")
                {
                    OpenCwpProject(SelectedTrackListItem.FullPath);
                }
            }

            var SelectedItemFullPath = SelectedTrackListItem.FullPath;
            SearchText = "";
            SelectedTrackListItem = allTracksList.Where(t => t.FullPath == SelectedItemFullPath).FirstOrDefault();
        }

        private void OpenCwpProject(string path)
        {
            if (IsPlaying)
                PlayStop();
            if (PerformanceMode)
            {
                var existingTrackInfoId = additionalTrackInfos.FindIndex(t => t.TrackPath == SelectedTrackListItem.FullPath);
                if (existingTrackInfoId == -1)
                {
                    additionalTrackInfos.Add(new AdditionalTrackInfo()
                    {
                        TrackPath = SelectedTrackListItem.FullPath,
                        TimesOpened = 1
                    });
                }
                else
                {
                    additionalTrackInfos[existingTrackInfoId].TimesOpened++;
                }
            }

            var wndHandle = FindWindow("Cakewalk Core", null);
            if (wndHandle != IntPtr.Zero)
            {

                var mnuHandle = GetMenu(wndHandle);

                for (uint i = 0; i < GetMenuItemCount(mnuHandle); i++)
                {
                    StringBuilder menuString = new StringBuilder();
                    GetMenuString(mnuHandle, i, menuString, 255, 0x00000400);
                    if (menuString.Equals(new StringBuilder("&File")))
                    {
                        var subMenuHandle = GetSubMenu(mnuHandle, i);
                        for (uint j = 0; j < GetMenuItemCount(subMenuHandle); j++)
                        {
                            StringBuilder subMenuString = new StringBuilder();
                            GetMenuString(subMenuHandle, j, subMenuString, 255, 0x00000400);
                            if (subMenuString.Equals(new StringBuilder("&Close")))
                            {
                                PostMessage(wndHandle, 0x0111, (int)GetMenuItemID((IntPtr)subMenuHandle, (int)j), 0);

                                if (PerformanceMode)
                                {
                                    Thread.Sleep(50);
                                    var dlgHandle = FindWindow("#32770", "Cakewalk");
                                    var btHandle = FindWindowEx(dlgHandle, IntPtr.Zero, "Button", "&Нет");
                                    PostMessage(btHandle, 0x0201, 0x0001, 0);
                                    PostMessage(btHandle, 0x0202, 0x0001, 0);
                                }
                            }
                        }
                    }
                }
            }

            System.Diagnostics.Process.Start(path);

            tmrFocusCounter = 0;
            tmrFocus = new Timer((tmr) => { SetForegroundWindow(mainWndHandle); if (tmrFocusCounter > 10) tmrFocus.Change(Timeout.Infinite, Timeout.Infinite); tmrFocusCounter++; }, 0, 0, 100);
        }

        private async void ProcessSearchAsync(string pathItemToSelect = null)
        {
            await searchSmph.WaitAsync();
            await Task.Run(() =>
            {
                try
                {
                    if (allTracksList != null)
                    {
                        TrackList = string.IsNullOrEmpty(SearchText) ? allTracksList.Where(t => (CategoriesFilter.FilteredList.Contains(t.Category) &&
                                                                                                VocalsFilter.FilteredList.Contains((int)t.VocalType) &&
                                                                                                (LoungeFilter == true ? t.IsLounge == true : true)) || t.IsDirectory == true).ToList() : allTracksList.FindAll(t => t.Caption.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);
                        if (!string.IsNullOrEmpty(pathItemToSelect))
                            Application.Current.Dispatcher.Invoke(new Action(() => SelectedTrackListItem = TrackList.FirstOrDefault(t => t.FullPath == pathItemToSelect)));
                    }
                }
                catch
                {
                    return;
                }
            });
            searchSmph.Release();
            searchSmph = new SemaphoreSlim(1);
        }

        private void LoadAppSettings()
        {
            appSettings = new AppSettings();
            string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.json";
            if (File.Exists(settingsFilePath))
            {
                appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(settingsFilePath));
            }

            if (Directory.Exists(appSettings.InitialPath))
            {
                currentPath = appSettings.InitialPath;
            }
            else
            {
                currentPath = "D:\\";
            }
            MainWindowX = appSettings.MainWindowX;
            MainWindowY = appSettings.MainWindowY;
            MainWindowHeight = appSettings.MainWindowHeight;
            MainWindowWidth = appSettings.MainWindowWidth;
            TrackListFontSize = appSettings.TrackListFontSize;

            string trackCategoriesPath = AppDomain.CurrentDomain.BaseDirectory + "\\Categories.json";
            if (File.Exists(trackCategoriesPath))
            {
                CategoriesList = JsonConvert.DeserializeObject<List<TrackCategory>>(File.ReadAllText(trackCategoriesPath));
            }
            else
            {
                CategoriesList = new List<TrackCategory>()
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

            string tracksInfoPath = AppDomain.CurrentDomain.BaseDirectory + "\\TracksInfo.json";
            if (File.Exists(tracksInfoPath))
            {
                additionalTrackInfos = JsonConvert.DeserializeObject<List<AdditionalTrackInfo>>(File.ReadAllText(tracksInfoPath));
            }
        }

        private void SaveAppSettings()
        {
            appSettings.InitialPath = currentPath;
            appSettings.MainWindowX = MainWindowX;
            appSettings.MainWindowY = MainWindowY;
            appSettings.MainWindowHeight = MainWindowHeight;
            appSettings.MainWindowWidth = MainWindowWidth;
            appSettings.TrackListFontSize = TrackListFontSize;
            var fileData = JsonConvert.SerializeObject(appSettings);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Settings.json", fileData);
            fileData = JsonConvert.SerializeObject(CategoriesList);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Categories.json", fileData);
            fileData = JsonConvert.SerializeObject(additionalTrackInfos);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\TracksInfo.json", fileData);
        }

        public void SetMainWindowHandle(IntPtr mainWndHandle)
        {
            this.mainWndHandle = mainWndHandle;
        }

        private void PlayStop()
        {
            var wndHandle = FindWindow("Cakewalk Core", null);
            if (wndHandle == IntPtr.Zero)
                return;

            SetForegroundWindow(wndHandle);
            Thread.Sleep(20);
            keybd_event(0x20, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);
            SetForegroundWindow(mainWndHandle);
        }
    }
}
