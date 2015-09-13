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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using RobynsWhiskerTracker.ModelInterface.ClipSettings;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.ClipSettings
{
    internal class ClipSettings : ModelObjectBase, IClipSettings
    {
        private string m_ClipFilePath;
        private int m_StartFrame;
        private int m_EndFrame;
        private int m_FrameInterval;
        private int m_NumberOfWhiskers;
        private int m_NumberOfPointsPerWhisker;
        private bool m_IncludeNosePoint;
        private bool m_IncludeOrientationPoint;
        private int m_NumberOfGenericPoints;
        private IWhiskerClipSettings[] m_Whiskers;

        public string ClipFilePath
        {
            get
            {
                return m_ClipFilePath;
            }
            set
            {
                if (Equals(m_ClipFilePath, value))
                {
                    return;
                }

                m_ClipFilePath = value;

                MarkAsDirty();
            }
        }

        public int StartFrame
        {
            get
            {
                return m_StartFrame;
            }
            set
            {
                if (Equals(m_StartFrame, value))
                {
                    return;
                }

                m_StartFrame = value;

                MarkAsDirty();
            }
        }

        public int EndFrame
        {
            get
            {
                return m_EndFrame;
            }
            set
            {
                if (Equals(m_EndFrame, value))
                {
                    return;
                }

                m_EndFrame = value;

                MarkAsDirty();
            }
        }

        public int FrameInterval
        {
            get
            {
                return m_FrameInterval;
            }
            set
            {
                if (Equals(m_FrameInterval, value))
                {
                    return;
                }

                m_FrameInterval = value;

                MarkAsDirty();
            }
        }

        public int NumberOfWhiskers
        {
            get
            {
                return m_NumberOfWhiskers;
            }
            set
            {
                if (Equals(m_NumberOfWhiskers, value))
                {
                    return;
                }

                m_NumberOfWhiskers = value;

                MarkAsDirty();
            }
        }
        public int NumberOfPointsPerWhisker
        {
            get
            {
                return m_NumberOfPointsPerWhisker;
            }
            set
            {
                if (Equals(m_NumberOfPointsPerWhisker, value))
                {
                    return;
                }

                m_NumberOfPointsPerWhisker = value;

                MarkAsDirty();
            }
        }

        public bool IncludeNosePoint
        {
            get
            {
                return m_IncludeNosePoint;
            }
            set
            {
                if (Equals(m_IncludeNosePoint, value))
                {
                    return;
                }

                m_IncludeNosePoint = value;

                if (!value)
                {
                    IncludeOrientationPoint = false;
                }

                MarkAsDirty();
            }
        }

        public bool IncludeOrientationPoint
        {
            get
            {
                return m_IncludeOrientationPoint;
            }
            set
            {
                if (Equals(m_IncludeOrientationPoint, value))
                {
                    return;
                }

                m_IncludeOrientationPoint = value;

                MarkAsDirty();
            }
        }

        public int NumberOfGenericPoints
        {
            get
            {
                return m_NumberOfGenericPoints;
            }
            set
            {
                if (Equals(m_NumberOfGenericPoints, value))
                {
                    return;
                }

                m_NumberOfGenericPoints = value;

                MarkAsDirty();
            }
        }

        public int TotalNumberOfPoints
        {
            get
            {
                return Whiskers.Sum(whisker => whisker.NumberOfPoints);
            }
        }

        public IWhiskerClipSettings[] Whiskers
        {
            get
            {
                return m_Whiskers;
            }
            set
            {
                m_Whiskers = value;
            }
        }

        public IWhisker[] CreateEmptyWhiskers(IMouseFrame mouseFrame)
        {
            return Whiskers.Select(x => x.CreateWhisker(mouseFrame)).ToArray();
        }
    }
}
