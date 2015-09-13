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
using System.Windows;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.NoseDisplacement;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.NoseDisplacement
{
    internal class NoseDisplacementFrame : ModelObjectBase, INoseDisplacementFrame
    {
        private IWhisker m_Nose;
        private IWhiskerPoint m_NosePoint;
        private Point m_NoseLocation;

        public Point NoseLocation
        {
            get
            {
                return m_NoseLocation;
            }
            private set
            {
                if (Equals(m_NoseLocation, value))
                {
                    return;
                }

                m_NoseLocation = value;

                MarkAsDirty();
            }
        }

        public IWhisker Nose
        {
            get
            {
                return m_Nose;
            }
            set
            {
                if (Equals(m_Nose, value))
                {
                    return;
                }

                m_Nose = value;

                if (m_Nose != null)
                {
                    NosePoint = m_Nose.WhiskerPoints[0];
                }

                MarkAsDirty();
            }
        }

        public IWhiskerPoint NosePoint
        {
            get
            {
                return m_NosePoint;
            }
            private set
            {
                if (Equals(m_NosePoint, value))
                {
                    return;
                }

                m_NosePoint = value;

                if (m_NosePoint != null)
                {
                    NoseLocation = new Point(m_NosePoint.XRatio * Nose.Parent.OriginalWidth, m_NosePoint.YRatio * Nose.Parent.OriginalHeight);
                }

                MarkAsDirty();
            }
        }
    }
}
