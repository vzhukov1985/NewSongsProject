using NewSongsProject.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewSongsProject.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr _windowHandle;
        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var list = TrackList;
            switch (e.Key)
            {

                case Key.Down:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        var dc = ((MainWindowVM)DataContext);
                        if (dc.SelectNextPlaylistItemCmd.CanExecute(null))
                        {
                            dc.SelectNextPlaylistItemCmd.Execute(null);
                        }
                    }
                    else
                    {
                        if (!list.Items.MoveCurrentToNext())
                        {
                            list.Items.MoveCurrentToLast();
                        }
                    }
                    e.Handled = true;
                    break;

                case Key.Up:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        var dc = ((MainWindowVM)DataContext);
                        if (dc.SelectPrevPlaylistItemCmd.CanExecute(null))
                        {
                            dc.SelectPrevPlaylistItemCmd.Execute(null);
                        }
                    }
                    else
                    {
                        if (!list.Items.MoveCurrentToPrevious())
                        {
                            list.Items.MoveCurrentToFirst();
                        }
                    }
                    e.Handled = true;
                    break;

                case Key.Space:
                    ((MainWindowVM)DataContext).PlayStopCmd.Execute(null);
                    e.Handled = true;
                    break;

                case Key.End:
                    var cmd = ((MainWindowVM)DataContext).SelectLastTrackCmd;
                    if (cmd.CanExecute(null))
                        cmd.Execute(null);
                    e.Handled = true;
                    break;

                case Key.F1:
                    this.Activate();
                    break;

                case Key.F2:
                    var wndHandle = FindWindow("Cakewalk Core", null);

                    if (wndHandle != IntPtr.Zero)
                    {
                        SetForegroundWindow(wndHandle);
                        return;
                    }
                    break;


                default:
                    break;
            }

        }

        private void TrackListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                var list = sender as ListView;

                if (list.SelectedIndex != -1)
                {
                    ListViewItem item = list.ItemContainerGenerator.ContainerFromIndex(list.SelectedIndex) as ListViewItem;
                    list.Focus();
                    if (item != null)
                    {
                        item.Focus();
                    }
                }
            }
        }

        private void MainWnd_TextInput(object sender, TextCompositionEventArgs e)
        {
            char[] searchKeys = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
                                  'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm',
                                  'й', 'ц', 'у', 'к', 'е', 'н', 'г', 'ш', 'щ', 'з', 'х', 'ъ', 'ф', 'ы', 'в', 'а', 'п', 'р', 'о', 'л', 'д', 'ж', 'э', 'я', 'ч', 'с', 'м', 'и', 'т', 'ь', 'б', 'ю'};


            var dc = (MainWindowVM)DataContext;

            if (e.Text == "." || e.Text == ",")
            {
                dc.AlterCategoryFilterCmd.Execute("-1");
                dc.AlterVocalsFilterCmd.Execute(-1);
                dc.LoungeFilter = false;
                e.Handled = true;
            }

            if (e.Text == "/")
            {
                dc.AlterVocalsFilterCmd.Execute(0);
                e.Handled = true;
            }

            if (e.Text == "*")
            {
                dc.AlterVocalsFilterCmd.Execute(1);
                e.Handled = true;
            }

            if (e.Text == "-")
            {
                dc.AlterVocalsFilterCmd.Execute(2);
                e.Handled = true;
            }

            if (e.Text == "+")
            {
                dc.AlterVocalsFilterCmd.Execute(-1);
                e.Handled = true;
            }

            if (e.Text == "\\")
            {
                dc.LoungeFilter = !dc.LoungeFilter;
            }

            if (!string.IsNullOrEmpty(e.Text) && searchKeys.Contains(e.Text[0]))
            {
                ((MainWindowVM)DataContext).AddSymbolSearchCmd.Execute(e.Text);
                e.Handled = true;
            }
        }

        private void MainWnd_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainWindowVM)DataContext).SetMainWindowHandle(new WindowInteropHelper(MainWnd).Handle);
        }

        private void MainWnd_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = GetScrollViewer(TrackList) as ScrollViewer;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }

        private void WndClose_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void WndMaximize_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void WndMinimize_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void Header_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                this.DragMove();
            }

            if (e.ClickCount == 2)
            {
                WndMaximize_PreviewMouseLeftButtonDown(sender, e);
            }
        }

        private void MainWnd_SourceInitialized(object sender, EventArgs e)
        {
            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, 0, 0x70); //F1
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == 0x70)
                            {
                                this.Activate();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void MainWnd_Closed(object sender, EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            //base.OnClosed(e);
        }

        private void MainWnd_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainGrid.Margin = new Thickness(7);
            }
            else
            {
                MainGrid.Margin = new Thickness(0);
            }
        }
    }
}
