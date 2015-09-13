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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.HeadOrientation;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.HeadOrientation
{
    internal class SingleHeadOrientation : ModelObjectBase, ISingleHeadOrientation
    {
        private IWhisker m_NoseWhisker;
        private IWhisker m_OrientationWhisker;
        private double m_VideoWidth;
        private double m_VideoHeight;

        private double m_Orientation;

        public IWhisker NoseWhisker
        {
            get
            {
                return m_NoseWhisker;
            }
            set
            {
                if (Equals(m_NoseWhisker, value))
                {
                    return;
                }

                m_NoseWhisker = value;

                m_VideoWidth = NoseWhisker.Parent.OriginalWidth;
                m_VideoHeight = NoseWhisker.Parent.OriginalHeight;

                MarkAsDirty();

                CalculateOrientation();
            }
        }

        public IWhisker OrientationWhisker
        {
            get
            {
                return m_OrientationWhisker;
            }
            set
            {
                if (Equals(m_OrientationWhisker, value))
                {
                    return;
                }

                m_OrientationWhisker = value;

                MarkAsDirty();

                CalculateOrientation();
            }
        }

        public double Orientation
        {
            get
            {
                return m_Orientation;
            }
            private set
            {
                if (Equals(m_Orientation, value))
                {
                    return;
                }

                m_Orientation = value;

                MarkAsDirty();
            }
        }

        protected double VideoWidth
        {
            get
            {
                return m_VideoWidth;
            }
        }

        protected double VideoHeight
        {
            get
            {
                return m_VideoHeight;
            }
        }

        private void CalculateOrientation()
        {
            if (NoseWhisker == null || OrientationWhisker == null)
            {
                return;
            }

            Point nosePoint = new Point(NoseWhisker.WhiskerPoints[0].XRatio * VideoWidth, NoseWhisker.WhiskerPoints[0].YRatio * VideoHeight);
            Point orientationPoint = new Point(OrientationWhisker.WhiskerPoints[0].XRatio * VideoWidth, OrientationWhisker.WhiskerPoints[0].YRatio * VideoHeight);

            Vector orientation = new Vector(orientationPoint.X - nosePoint.X, orientationPoint.Y - nosePoint.Y);
            Vector vertical = new Vector(0, 1);

            Orientation = Vector.AngleBetween(vertical, orientation);
        }
    }
}
