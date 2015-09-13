/*
Manual Whisker Annotator - A program to manually annotate whiskers and analyse them
Copyright (C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RobynsWhiskerTracker.Behaviours
{
    public static class ListViewKeepItemSelectedBehaviour
    {
        public static readonly DependencyProperty DisableDeselectProperty =
            DependencyProperty.RegisterAttached("DisableDeselect",
            typeof(bool), typeof(ListViewKeepItemSelectedBehaviour),
            new UIPropertyMetadata(false, OnDisableDeselectChanged));

        public static bool GetDisableDeselect(DependencyObject source)
        {
            return (bool)source.GetValue(DisableDeselectProperty);
        }

        public static void SetDisableDeselect(DependencyObject source, bool value)
        {
            source.SetValue(DisableDeselectProperty, value);
        }

        private static void OnDisableDeselectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = sender as ListView;

            if (listView == null)
            {
                return;
            }

            bool disable = (e.NewValue is bool) && (bool)e.NewValue;

            if (disable)
            {
                listView.SelectionChanged += ListVIewOnSelectionChanged;
            }
            else
            {
                listView.SelectionChanged -= ListVIewOnSelectionChanged;
            }
        }

        private static void ListVIewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (ListView)sender;

            if (listView.SelectedItem == null)
            {
                if (e.RemovedItems.Count > 0)
                {
                    object itemToReselect = e.RemovedItems[0];
                    if (listView.Items.Contains(itemToReselect))
                    {
                        listView.SelectedItem = itemToReselect;
                    }
                }
            }
        }
    }
}
