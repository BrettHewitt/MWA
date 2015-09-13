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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency.FrequencyTypes;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Frequency
{
    internal class SingleWhiskerFrequency : ModelObjectBase, ISingleWhiskerFrequency
    {
        private double m_Frequency;
        private IWhisker m_Whisker;
        private double[] m_Signal;
        private IFrequencyTypeBase m_FrequencyType;
        private double m_FrameRate;
        private double m_FrameInterval;
        private Dictionary<double, double> m_ExtraData; 

        public double Frequency
        {
            get
            {
                return m_Frequency;
            }
            private set
            {
                if (Equals(m_Frequency, value))
                {
                    return;
                }

                m_Frequency = value;

                MarkAsDirty();
            }
        }

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

                MarkAsDirty();
            }
        }

        public double[] Signal
        {
            get
            {
                return m_Signal;
            }
            set
            {
                m_Signal = value;

                CalculateFrequency();

                MarkAsDirty();
            }
        }

        public int WhiskerId
        {
            get
            {
                return Whisker.WhiskerId;
            }
        }

        public IFrequencyTypeBase FrequencyType
        {
            get
            {
                return m_FrequencyType;
            }
            set
            {
                if (Equals(m_FrequencyType, value))
                {
                    return;
                }

                m_FrequencyType = value;

                CalculateFrequency();

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

                CalculateFrequency();

                MarkAsDirty();
            }
        }

        public double FrameInterval
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

                CalculateFrequency();

                MarkAsDirty();
            }
        }

        public Dictionary<double, double> ExtraData
        {
            get
            {
                return m_ExtraData;
            }
            private set
            {
                if (ReferenceEquals(m_ExtraData, value))
                {
                    return;
                }

                m_ExtraData = value;

                MarkAsDirty();
            }
        }

        private void CalculateFrequency()
        {
            if (FrequencyType == null || Signal == null || FrameRate == 0 || FrameInterval == 0)
            {
                return;
            }

            Frequency = FrequencyType.CalculateFrequency(Signal, FrameRate, FrameInterval);
            ExtraData = FrequencyType.GetExtraData();
        }
    }
}
