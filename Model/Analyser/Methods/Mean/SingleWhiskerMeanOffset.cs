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
using MathNet.Numerics.Statistics;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Mean;
using RobynsWhiskerTracker.ModelInterface.Whisker;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Mean
{
    internal class SingleWhiskerMeanOffset : ModelObjectBase, ISingleWhiskerMeanOffset
    {
        private double m_MeanOffset;
        private IWhisker m_Whisker;
        private double[] m_Signal;

        public double MeanOffset
        {
            get
            {
                return m_MeanOffset;
            }
            private set
            {
                if (Equals(m_MeanOffset, value))
                {
                    return;
                }

                m_MeanOffset = value;

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

        public int WhiskerId
        {
            get
            {
                return Whisker.WhiskerId;
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

                CalculateMeanOffset();

                MarkAsDirty();
            }
        }

        private void CalculateMeanOffset()
        {
            if (Signal == null)
            {
                return;
            }

            if (Signal.Length == 0)
            {
                return;
            }

            MeanOffset = Signal.Mean();
        }
    }
}
