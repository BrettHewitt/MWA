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

using System.Xml.Serialization;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.WhiskerPoint
{
    public class WhiskerPointXml
    {
        [XmlElement(ElementName = "PointID")]
        public int PointId
        {
            get;
            set;
        }

        [XmlElement(ElementName = "XRatio")]
        public double XRatio
        {
            get;
            set;
        }

        [XmlElement(ElementName = "YRatio")]
        public double YRatio
        {
            get;
            set;
        }

        public WhiskerPointXml()
        {
            
        }

        public WhiskerPointXml(IWhiskerPoint whiskerPoint)
        {
            PointId = whiskerPoint.PointId;
            XRatio = whiskerPoint.XRatio;
            YRatio = whiskerPoint.YRatio;
        }

        public IWhiskerPoint CreateWhiskerPoint(IWhisker parent)
        {
            WhiskerPoint whiskerPoint = new WhiskerPoint();
            whiskerPoint.Parent = parent;
            whiskerPoint.PointId = PointId;
            whiskerPoint.XRatio = XRatio;
            whiskerPoint.YRatio = YRatio;
            whiskerPoint.DataLoadComplete();

            return whiskerPoint;
        }
    }
}
