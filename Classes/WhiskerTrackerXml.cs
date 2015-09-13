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
using System.Xml;
using System.Xml.Serialization;
using RobynsWhiskerTracker.Model.ClipSettings;
using RobynsWhiskerTracker.Model.MouseFrame;
using RobynsWhiskerTracker.Model.Settings;

namespace RobynsWhiskerTracker.Classes
{
    public class WhiskerTrackerXml
    {
        [XmlElement(ElementName = "ClipSettings")]
        public ClipSettingsXml ClipSettings
        {
            get;
            set;
        }

        [XmlArray(ElementName = "Frames")]
        [XmlArrayItem(ElementName = "Frame")]
        public MouseFrameXml[] Frames
        {
            get;
            set;
        }

        [XmlElement(ElementName="UnitSettings")]
        public UnitSettingsXml UnitSettings
        {
            get;
            set;
        }

        [XmlElement(ElementName = "FrameRateSettings")]
        public FrameRateSettingsXml FrameRateSettings
        {
            get;
            set;
        }

        public WhiskerTrackerXml()
        {
            
        }

        public WhiskerTrackerXml(ClipSettingsXml clipSettings, MouseFrameXml[] frames, UnitSettingsXml unitSettings, FrameRateSettingsXml frameRateSettings)
        {
            ClipSettings = clipSettings;
            Frames = frames;
            UnitSettings = unitSettings;
            FrameRateSettings = frameRateSettings;
        }
    }
}
