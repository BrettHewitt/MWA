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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Mean;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Mean
{
    internal class WhiskerMeanOffset : ModelObjectBase, IWhiskerMeanOffset
    {
        private ISingleWhiskerMeanOffset[] m_Whiskers;
        private IWhiskerAngle m_WhiskerAngle;
        private double m_TValue;
        private string m_Name;

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (Equals(m_Name, value))
                {
                    return;
                }

                m_Name = value;

                MarkAsDirty();
            }
        }

        public ISingleWhiskerMeanOffset[] Whiskers
        {
            get
            {
                return m_Whiskers;
            }
            private set
            {
                if (ReferenceEquals(m_Whiskers, value))
                {
                    return;
                }

                m_Whiskers = value;

                MarkAsDirty();
            }
        }

        public IWhiskerAngle WhiskerAngle
        {
            get
            {
                return m_WhiskerAngle;
            }
            private set
            {
                if (Equals(m_WhiskerAngle, value))
                {
                    return;
                }

                m_WhiskerAngle = value;

                MarkAsDirty();
            }
        }

        public double TValue
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

                UpdateTValue();

                MarkAsDirty();
            }
        }

        public WhiskerMeanOffset()
        {
            Name = "Mean Offset";
            WhiskerAngle = ModelResolver.Resolve<IWhiskerAngle>();
        }

        public void LoadData(IMouseFrame[] frames)
        {
            //Load Whisker Angle Data
            WhiskerAngle.LoadData(frames);

            List<ISingleWhiskerMeanOffset> whiskers = new List<ISingleWhiskerMeanOffset>();
            foreach (IWhisker whisker in frames[0].Whiskers)
            {
                if (whisker.IsGenericPoint)
                {
                    continue;
                }

                ISingleWhiskerMeanOffset singleWhisker = ModelResolver.Resolve<ISingleWhiskerMeanOffset>();
                singleWhisker.Whisker = whisker;
                whiskers.Add(singleWhisker);
            }

            Whiskers = whiskers.ToArray();

            UpdateTValue();
        }

        private void UpdateTValue()
        {
            if (WhiskerAngle == null)
            {
                return;
            }

            WhiskerAngle.UpdateTValue(TValue);

            for (int j = 0; j < Whiskers.Length; j++)
            {
                double[] whiskerAngles = new double[WhiskerAngle.Frames.Length];
                for (int i = 0; i < WhiskerAngle.Frames.Length; i++)
                {
                    ISingleWhiskerAngle whiskerAngle = WhiskerAngle.Frames[i].Targets.Single(x => x.Whisker.WhiskerId == Whiskers[j].WhiskerId);
                    whiskerAngles[i] = whiskerAngle.Angle;
                }

                Whiskers[j].Signal = whiskerAngles;
            }
        }

        public object[][] ExportData()
        {
            List<object[]> data = new List<object[]>();

            data.Add(Whiskers.Select(target => target.Whisker.WhiskerName).Cast<object>().ToArray());
            data.Add(Whiskers.Select(whisker => whisker.MeanOffset).Cast<object>().ToArray());

            return data.ToArray();
        }

        public object[][] ExportMeanData()
        {
            List<object[]> data = new List<object[]>();
            data.Add(new object[]{"Mean Offset"});
            data.Add(Whiskers.Select(target => target.Whisker.WhiskerName).Cast<object>().ToArray());
            data.Add(Whiskers.Select(whisker => whisker.MeanOffset).Cast<object>().ToArray());

            return data.ToArray();
        }
    }
}
