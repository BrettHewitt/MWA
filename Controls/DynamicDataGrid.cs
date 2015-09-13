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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobynsWhiskerTracker.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RobynsWhiskerTracker.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RobynsWhiskerTracker.Controls;assembly=RobynsWhiskerTracker.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DynamicDataGrid/>
    ///
    /// </summary>
    public class DynamicDataGrid : DataGrid
    {
        static DynamicDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicDataGrid), new FrameworkPropertyMetadata(typeof(DynamicDataGrid)));
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Columns.Clear();

            int maxColumns = 0;

            IEnumerable<object[,]> values = newValue.Cast<object[,]>();

            if (values == null)
            {
                return;
            }

            foreach (object[,] value in values)
            {
                int itemMax = value.GetLength(1);

                if (itemMax > maxColumns)
                {
                    maxColumns = itemMax;
                }
            }

            for (int i = 0; i < maxColumns; i++)
            {
                Columns.Add(new DataGridTextColumn());
                //ItemContainerGenerator.ContainerFromIndex()
            }

            int rowCounter = 0;
            foreach (object[,] value in values)
            {
                int valueRowLength = value.GetLength(0);
                int valueColumnLength = value.GetLength(1);

                for (int row = 0; row < valueRowLength; row++)
                {
                    object[] currentRow = new object[valueColumnLength];

                    for (int column = 0; column < valueColumnLength; column++)
                    {
                        currentRow[column] = value[row, column];
                    }

                    Items.Add(currentRow);
                }

                rowCounter += value.GetLength(0);
            }

            Console.WriteLine(maxColumns);
            
            //base.OnItemsSourceChanged(oldValue, newValue);
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }
    }
}
