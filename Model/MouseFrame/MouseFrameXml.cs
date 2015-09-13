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

using System.Linq;
using System.Xml.Serialization;
using RobynsWhiskerTracker.Classes;
using RobynsWhiskerTracker.Model.Whisker;
using RobynsWhiskerTracker.Model.WhiskerPoint;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.ViewModel;

namespace RobynsWhiskerTracker.Model.MouseFrame
{
    public class MouseFrameXml
    {
        [XmlElement(ElementName = "FrameNumber")]
        public int FrameNumber
        {
            get;
            set;
        }

        [XmlElement(ElementName = "IndexNumber")]
        public int IndexNumber
        {
            get;
            set;
        }

        [XmlArray(ElementName = "Whiskers")]
        [XmlArrayItem(ElementName = "Whisker")]
        public WhiskerXml[] Whiskers
        {
            get;
            set;
        }

        public MouseFrameXml()
        {
            
        }

        public MouseFrameXml(IMouseFrame mouseFrame)
        {
            FrameNumber = mouseFrame.FrameNumber;
            IndexNumber = mouseFrame.IndexNumber;
            Whiskers = mouseFrame.Whiskers.Select(x => new WhiskerXml(x)).ToArray();
        }

        public IMouseFrame GetMouseFrame()
        {
            MouseFrame mouseFrame = new MouseFrame();
            mouseFrame.FrameNumber = FrameNumber;
            mouseFrame.IndexNumber = IndexNumber;
            mouseFrame.Whiskers = Whiskers.Select(x => x.GetWhisker(mouseFrame)).ToArray();
            mouseFrame.DataLoadComplete();

            return mouseFrame;
        }
    }
}
