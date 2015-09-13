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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Velocity;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using System.Linq;
using System.Windows;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Velocity
{
    internal class SingleWhiskerVelocity : ModelObjectBase, ISingleWhiskerVelocity
    {
        private IWhisker m_Whisker;
        private IWhiskerPoint m_VelocityPoint;
        private ISingleWhiskerVelocity m_PreviousFrame;
        private ISingleWhiskerVelocity m_NosePoint;
        private IFrameRateSettings m_FrameRateSettings;
        private IUnitSettings m_UnitSettings;

        private Vector m_Velocity;
        private bool m_IsHeadPoint;
        private bool m_CompensateForHeadMovement;

        private double m_VideoWidth;
        private double m_VideoHeight;

        public IWhisker Whisker
        {
            get
            {
                return m_Whisker;
            }
            set
            {
                if (Equals(m_Whisker, value))
                {
                    return;
                }

                m_Whisker = value;

                if (Whisker != null)
                {
                    m_VideoWidth = Whisker.Parent.OriginalWidth;
                    m_VideoHeight = Whisker.Parent.OriginalHeight;

                    VelocityPoint = Whisker.WhiskerPoints.First();

                    if (Whisker.WhiskerId == -1)
                    {
                        CompensateForHeadMovement = false;
                        IsHeadPoint = true;
                    }
                }

                CalculateVelocity();

                MarkAsDirty();
            }
        }

        public IWhiskerPoint VelocityPoint
        {
            get
            {
                return m_VelocityPoint;
            }
            private set
            {
                if (ReferenceEquals(m_VelocityPoint, value))
                {
                    return;
                }

                m_VelocityPoint = value;

                MarkAsDirty();
            }
        }

        public ISingleWhiskerVelocity PreviousFrame
        {
            get
            {
                return m_PreviousFrame;
            }
            set
            {
                if (Equals(m_PreviousFrame, value))
                {
                    return;
                }

                m_PreviousFrame = value;

                CalculateVelocity();

                MarkAsDirty();
            }
        }

        public ISingleWhiskerVelocity NosePoint
        {
            get
            {
                return m_NosePoint;
            }
            set
            {
                if (Equals(m_NosePoint, value))
                {
                    return;
                }

                m_NosePoint = value;

                if (m_NosePoint != null)
                {
                    CompensateForHeadMovement = true;
                }

                MarkAsDirty();
            }
        }

        public Vector Velocity
        {
            get
            {
                return m_Velocity;
            }
            private set
            {
                if (Equals(m_Velocity, value))
                {
                    return;
                }

                m_Velocity = value;

                MarkAsDirty();
            }
        }

        public string DisplayVelocity
        {
            get
            {
                return Velocity.Length + " " + UnitSettings.UnitsName + "/s";
            }
        }

        public IFrameRateSettings FrameRateSettings
        {
            get
            {
                return m_FrameRateSettings;
            }
            set
            {
                if (Equals(m_FrameRateSettings, value))
                {
                    return;
                }

                m_FrameRateSettings = value;

                CalculateVelocity();

                MarkAsDirty();
            }
        }

        public IUnitSettings UnitSettings
        {
            get
            {
                return m_UnitSettings;
            }
            set
            {
                if (Equals(m_UnitSettings, value))
                {
                    return;
                }

                m_UnitSettings = value;

                CalculateVelocity();

                MarkAsDirty();
            }
        }

        public bool IsHeadPoint
        {
            get
            {
                return m_IsHeadPoint;
            }
            private set
            {
                if (Equals(m_IsHeadPoint, value))
                {
                    return;
                }

                m_IsHeadPoint = value;

                MarkAsDirty();
            }
        }

        public bool CompensateForHeadMovement
        {
            get
            {
                return m_CompensateForHeadMovement;
            }
            set
            {
                if (Equals(m_CompensateForHeadMovement, value))
                {
                    return;
                }

                if (NosePoint == null)
                {
                    return;
                }

                m_CompensateForHeadMovement = value;

                CalculateVelocity();

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

        private void CalculateVelocity()
        {
            if (VelocityPoint == null || PreviousFrame == null || FrameRateSettings == null || UnitSettings == null)
            {
                return;
            }

            IWhiskerPoint previousWhisker = PreviousFrame.VelocityPoint;
            Point currentPoint = new Point(VelocityPoint.XRatio*VideoWidth, VelocityPoint.YRatio*VideoHeight);
            Point previousPoint = new Point(previousWhisker.XRatio*VideoWidth, previousWhisker.YRatio*VideoHeight);

            Vector distance = new Vector(currentPoint.X - previousPoint.X, currentPoint.Y - previousPoint.Y) * FrameRateSettings.OriginalFrameRate;
            distance *= UnitSettings.UnitsPerPixel;

            if (CompensateForHeadMovement)
            {
                distance -= NosePoint.Velocity;
            }

            Velocity = distance;
        }
    }
}
