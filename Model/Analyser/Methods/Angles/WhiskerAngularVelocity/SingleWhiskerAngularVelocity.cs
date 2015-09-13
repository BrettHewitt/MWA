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
using RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngularVelocity;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngularVelocity
{
    internal class SingleWhiskerAngularVelocity : SingleWhiskerAngle, ISingleWhiskerAngularVelocity
    {
        private ISingleWhiskerAngularVelocity m_PreviousFrame;
        private double m_AngularVelocity;
        private double m_FrameRate;

        public override double TValue
        {
            get
            {
                return m_TValue;
            }
            set
            {
                if (Equals(m_TValue, value))
                {
                    return;
                }

                m_TValue = value;

                CalculateAngle();
                CalculateAngularVelocity();

                MarkAsDirty();
            }
        }

        public ISingleWhiskerAngularVelocity PreviousFrame
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

                CalculateAngularVelocity();

                MarkAsDirty();
            }
        }

        public double AngularVelocity
        {
            get
            {
                return m_AngularVelocity;
            }
            private set
            {
                if (Equals(m_AngularVelocity, value))
                {
                    return;
                }

                m_AngularVelocity = value;

                MarkAsDirty();
            }
        }

        public double FrameRate
        {
            get
            {
                return m_FrameRate;
            }
            set
            {
                if (Equals(m_FrameRate, value))
                {
                    return;
                }

                m_FrameRate = value;

                CalculateAngularVelocity();

                MarkAsDirty();
            }
        }

        private void CalculateAngularVelocity()
        {
            //Make sure we have the necessary info to caluclate it
            if (PreviousFrame == null || FrameRate == 0)
            {
                return;
            }

            double angle1 = Angle;
            double angle2 = PreviousFrame.Angle;
            double delta = angle1 - angle2;

            AngularVelocity = delta * FrameRate;
        }

        public void UpdateAngleType(IAngleTypeBase angleType)
        {
            AngleType = angleType;
            CalculateAngularVelocity();
        }
    }
}