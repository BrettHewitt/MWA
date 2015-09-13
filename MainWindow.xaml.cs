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

using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace RobynsWhiskerTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            IMouseClickedViewModel viewModel = DataContext as IMouseClickedViewModel;

            if (viewModel != null)
            {
                viewModel.MouseClicked(sender, e);
            }
        }

        private void Image_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            IResize viewModel = DataContext as IResize;

            if (viewModel != null)
            {
                viewModel.SizeChanged(sender, e);
            }
        }
    }
}
