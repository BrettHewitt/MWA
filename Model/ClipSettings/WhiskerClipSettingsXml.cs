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
using System.Xml.Serialization;
using RobynsWhiskerTracker.ModelInterface.ClipSettings;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.ClipSettings
{
    public class WhiskerClipSettingsXml
    {
        [XmlElement(ElementName="WhiskerId")]
        public int WhiskerId
        {
            get;
            set;
        }

        [XmlElement(ElementName = "NumberOfPoints")]
        public int NumberOfPoints
        {
            get;
            set;
        }

        [XmlElement(ElementName = "WhiskerName")]
        public string WhiskerName
        {
            get;
            set;
        }

        [XmlElement(ElementName = "IsGenericPoint")]
        public bool IsGenericPoint
        {
            get;
            set;
        }

        public WhiskerClipSettingsXml()
        {
            
        }

        public WhiskerClipSettingsXml(IWhiskerClipSettings whiskerClipSettings)
        {
            WhiskerId = whiskerClipSettings.WhiskerId;
            WhiskerName = whiskerClipSettings.WhiskerName;
            IsGenericPoint = whiskerClipSettings.IsGenericPoint;
            NumberOfPoints = whiskerClipSettings.NumberOfPoints;
        }

        public IWhiskerClipSettings CreateWhiskerClipSettings()
        {
            WhiskerClipSettings whiskerClipSettings = new WhiskerClipSettings();

            whiskerClipSettings.WhiskerId = WhiskerId;
            whiskerClipSettings.WhiskerName = WhiskerName;
            whiskerClipSettings.NumberOfPoints = NumberOfPoints;
            whiskerClipSettings.IsGenericPoint = IsGenericPoint;
            whiskerClipSettings.DataLoadComplete();

            return whiskerClipSettings;
        }
    }
}
