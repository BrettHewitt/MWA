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

using RobynsWhiskerTracker.ModelInterface.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RobynsWhiskerTracker.Model.Settings
{
    public class FrameRateSettingsXml
    {
        [XmlElement(ElementName = "OriginalFrameRate")]
        public double OriginalFrameRate
        {
            get;
            set;
        }

        [XmlElement(ElementName = "CurrentFrameRate")]
        public double CurrentFrameRate
        {
            get;
            set;
        }

        [XmlElement(ElementName = "ModifierRatio")]
        public double ModifierRatio
        {
            get;
            set;
        }

        public FrameRateSettingsXml()
        {

        }

        public FrameRateSettingsXml(IFrameRateSettings settings)
        {
            CurrentFrameRate = settings.CurrentFrameRate;
            OriginalFrameRate = settings.OriginalFrameRate;            
        }

        public IFrameRateSettings GetSettings()
        {
            IFrameRateSettings settings = new FrameRateSettings();
            settings.CurrentFrameRate = CurrentFrameRate;
            settings.OriginalFrameRate = OriginalFrameRate;

            return settings;
        }
    }
}
