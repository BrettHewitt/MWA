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
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Whisker
{
    internal class Whisker : ModelObjectBase, IWhisker
    {
        private int m_WhiskerId;
        private IWhiskerPoint[] m_WhiskerPoints;
        private string m_WhiskerName;
        private bool m_IsGenericPoint;
        private IMouseFrame m_Parent;
        
        public int WhiskerId
        {
            get
            {
                return m_WhiskerId;
            }
            set
            {
                if (Equals(m_WhiskerId, value))
                {
                    return;
                }

                m_WhiskerId = value;

                MarkAsDirty();
            }
        }

        public IWhiskerPoint[] WhiskerPoints
        {
            get
            {
                return m_WhiskerPoints;
            }
            set
            {
                if (ReferenceEquals(m_WhiskerPoints, value))
                {
                    return;
                }

                m_WhiskerPoints = value;

                MarkAsDirty();
            }
        }

        public string WhiskerName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_WhiskerName))
                {
                    return WhiskerId.ToString();
                }

                return m_WhiskerName;
            }
            set
            {
                if (Equals(m_WhiskerName, value))
                {
                    return;
                }

                m_WhiskerName = value;

                MarkAsDirty();
            }
        }

        public bool IsGenericPoint
        {
            get
            {
                return m_IsGenericPoint;
            }
            set
            {
                if (Equals(m_IsGenericPoint, value))
                {
                    return;
                }

                m_IsGenericPoint = value;

                MarkAsDirty();
            }
        }

        public IMouseFrame Parent
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

        public void Initialise(int numberOfPoints)
        {
            WhiskerPoints = new IWhiskerPoint[numberOfPoints];
        }

        public bool WhiskerReady()
        {
            if (WhiskerPoints == null)
            {
                return false;
            }

            if (WhiskerPoints.Any(x => x == null))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            IWhisker whisker = obj as IWhisker;

            if (whisker == null)
            {
                return false;
            }

            return Equals(whisker);
        }

        public bool Equals(IWhisker whisker)
        {
            return Parent.Equals(whisker.Parent) && WhiskerId == whisker.WhiskerId;
        }

        public override int GetHashCode()
        {
            return Parent.GetHashCode() ^ WhiskerId;
        }

        public IWhisker Clone()
        {
            Whisker whisker = new Whisker();

            whisker.WhiskerId = WhiskerId;
            whisker.Parent = Parent;
            whisker.WhiskerName = WhiskerName;
            whisker.IsGenericPoint = IsGenericPoint;
            whisker.WhiskerPoints = new IWhiskerPoint[WhiskerPoints.Length];

            return whisker;
        }
    }
}
