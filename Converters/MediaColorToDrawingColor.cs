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
using System.Windows.Data;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace RobynsWhiskerTracker.Converters
{
    public class MediaColorToDrawingColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is DColor))
            {
                return null;
            }

            DColor originalColor = (DColor)value;

            return MColor.FromArgb(originalColor.A, originalColor.R, originalColor.G, originalColor.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is MColor))
            {
                return null;
            }

            MColor originalColor = (MColor)value;

            return DColor.FromArgb(originalColor.A, originalColor.R, originalColor.G, originalColor.B);
        }
    }
}
