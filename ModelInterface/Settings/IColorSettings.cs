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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.ModelInterface.Settings
{
    public interface IColorSettings : ISettingsBase
    {
        Color NoseColor
        {
            get;
            set;
        }

        Color OrientationColor
        {
            get;
            set;
        }

        Color Whisker1Color
        {
            get;
            set;
        }

        Color Whisker2Color
        {
            get;
            set;
        }

        Color Whisker3Color
        {
            get;
            set;
        }

        Color Whisker4Color
        {
            get;
            set;
        }

        Color Whisker5Color
        {
            get;
            set;
        }

        Color Whisker6Color
        {
            get;
            set;
        }

        Color Whisker7Color
        {
            get;
            set;
        }

        Color Whisker8Color
        {
            get;
            set;
        }

        Color Whisker9Color
        {
            get;
            set;
        }

        Color Whisker10Color
        {
            get;
            set;
        }

        Color GetColorFromId(int id);
        void ReturnToDefault();
    }
}
