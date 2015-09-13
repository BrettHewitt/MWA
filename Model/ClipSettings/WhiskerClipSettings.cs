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
using RobynsWhiskerTracker.ModelInterface.ClipSettings;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.ClipSettings
{
    internal class WhiskerClipSettings : ModelObjectBase, IWhiskerClipSettings
    {
        private int m_WhiskerId;
        private int m_NumberOfPoints;
        private string m_WhiskerName;
        private bool m_IsGenericPoint;

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

        public int NumberOfPoints
        {
            get
            {
                return m_NumberOfPoints;
            }
            set
            {
                if (Equals(m_NumberOfPoints, value))
                {
                    return;
                }

                m_NumberOfPoints = value;

                MarkAsDirty();
            }
        }

        public string WhiskerName
        {
            get
            {
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

        public IWhisker CreateWhisker(IMouseFrame mouseFrame)
        {
            IWhisker whisker = ModelResolver.Resolve<IWhisker>();

            whisker.Parent = mouseFrame;
            whisker.WhiskerId = WhiskerId;

            IWhiskerPoint[] whiskerPoints = new IWhiskerPoint[NumberOfPoints];
            whisker.WhiskerPoints = new IWhiskerPoint[NumberOfPoints];
            for (int i = 0; i < NumberOfPoints; i++)
            {
                IWhiskerPoint whiskerPoint = ModelResolver.Resolve<IWhiskerPoint>();
                whiskerPoint.Parent = whisker;
                whiskerPoint.PointId = i + 1;
                whiskerPoints[i] = whiskerPoint;
            }

            whisker.WhiskerPoints = whiskerPoints;
            whisker.IsGenericPoint = IsGenericPoint;
            whisker.WhiskerName = WhiskerName;

            return whisker;
        }
    }
}
