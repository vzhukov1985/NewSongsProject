﻿using NewSongsProject.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public MainWindow()
        {
            DataContext = new MainWindowVM();
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
    }
}
