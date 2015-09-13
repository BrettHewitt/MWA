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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.ProtractionRetraction
{
    internal abstract class ProtractionRetractionBase : ModelObjectBase, IProtractionRetractionBase
    {
        private double m_MinAngle;
        private double m_MaxAngle;
        private double m_DeltaTime;
        private double m_MeanAngularVelocity;

        public abstract string Name
        {
            get;
        }

        public double MinAngle
        {
            get
            {
                return m_MinAngle;
            }
            private set
            {
                if (Equals(m_MinAngle, value))
                {
                    return;
                }

                m_MinAngle = value;

                MarkAsDirty();
            }
        }

        public double MaxAngle
        {
            get
            {
                return m_MaxAngle;
            }
            private set
            {
                if (Equals(m_MaxAngle, value))
                {
                    return;
                }

                m_MaxAngle = value;

                MarkAsDirty();
            }
        }

        public double DeltaTime
        {
            get
            {
                return m_DeltaTime;
            }
            private set
            {
                if (Equals(m_DeltaTime, value))
                {
                    return;
                }

                m_DeltaTime = value;

                MarkAsDirty();
            }
        }

        public double MeanAngularVelocity
        {
            get
            {
                return m_MeanAngularVelocity;
            }
        }

        public double Amplitude
        {
            get
            {
                return Math.Abs(MaxAngle - MinAngle);
            }
        }

        public virtual void UpdateData(double minAngle, double maxAngle, double deltaTime)
        {
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            DeltaTime = deltaTime;
            m_MeanAngularVelocity = CalculateMean();
        }

        protected abstract double CalculateMean();
    }
}
