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

using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;

namespace RobynsWhiskerTracker.Model.WhiskerPoint
{
    internal class WhiskerPoint : ModelObjectBase, IWhiskerPoint
    {
        private double m_XRatio;
        private double m_YRatio;
        private int m_PointId;

        private IWhisker m_Parent;

        public double XRatio
        {
            get
            {
                return m_XRatio;
            }
            set
            {
                if (Equals(m_XRatio, value))
                {
                    return;
                }

                m_XRatio = value;

                MarkAsDirty();
            }
        }

        public double YRatio
        {
            get
            {
                return m_YRatio;
            }
            set
            {
                if (Equals(m_YRatio, value))
                {
                    return;
                }

                m_YRatio = value;

                MarkAsDirty();
            }
        }

        public int PointId
        {
            get
            {
                return m_PointId;
            }
            set
            {
                if (Equals(m_PointId, value))
                {
                    return;
                }

                m_PointId = value;

                MarkAsDirty();
            }
        }

        public IWhisker Parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                if (Equals(m_Parent, value))
                {
                    return;
                }

                m_Parent = value;

                MarkAsDirty();
            }
        }

        public override bool Equals(object obj)
        {
            IWhiskerPoint other = obj as IWhiskerPoint;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(IWhiskerPoint other)
        {
            return Parent.Equals(other.Parent) && PointId == other.PointId;
        }

        public override int GetHashCode()
        {
            return Parent.GetHashCode() ^ PointId;
        }

        public IWhiskerPoint Clone()
        {
            WhiskerPoint whiskerPoint = new WhiskerPoint();

            whiskerPoint.PointId = PointId;
            whiskerPoint.Parent = Parent;
            whiskerPoint.XRatio = XRatio;
            whiskerPoint.YRatio = YRatio;

            return whiskerPoint;
        }
    }
}
