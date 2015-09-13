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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Amplitude;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Amplitude
{
    internal class WhiskerAmplitude : ModelObjectBase, IWhiskerAmplitude
    {
        private IWhiskerAngle m_WhiskerAngle;
        private double m_TValue;
        private string m_Name;
        private ISingleWhiskerAmplitude[] m_Whiskers;

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

        public ISingleWhiskerAmplitude[] Whiskers
        {
            get
            {
                return m_Whiskers;
            }
            set
            {
                if (ReferenceEquals(m_Whiskers, value))
                {
                    return;
                }

                m_Whiskers = value;

                MarkAsDirty();
            }
        }

        private IWhiskerProtractionRetraction ProtractionRetraction
        {
            get;
            set;
        }

        public WhiskerAmplitude()
        {
            Name = "Whisker Amplitude";
            WhiskerAngle = ModelResolver.Resolve<IWhiskerAngle>();
            ProtractionRetraction = ModelResolver.Resolve<IWhiskerProtractionRetraction>();
        }

        public void LoadData(IMouseFrame[] frames)
        {
            //Load Whisker Angle Data
            WhiskerAngle.LoadData(frames);
            ProtractionRetraction.LoadData(frames);

            //Get whiskers
            List<ISingleWhiskerAmplitude> whiskers = new List<ISingleWhiskerAmplitude>();
            foreach (IWhisker whisker in frames[0].Whiskers)
            {
                if (whisker.IsGenericPoint)
                {
                    continue;
                }

                ISingleWhiskerAmplitude singleWhisker = ModelResolver.Resolve<ISingleWhiskerAmplitude>();
                singleWhisker.Whisker = whisker;
                ISingleWhiskerProtractionRetraction proRetWhisker = ProtractionRetraction.Whiskers.Single(x => x.WhiskerId == whisker.WhiskerId);
                singleWhisker.ProtractionRetractionData = proRetWhisker.ProtractionRetractionData;
                singleWhisker.ProtractionRetractionDictionary = proRetWhisker.ProtractionRetractionDictionary;
                whiskers.Add(singleWhisker);
            }

            Whiskers = whiskers.ToArray();

            UpdateTValue();
        }

        private void UpdateTValue()
        {
            WhiskerAngle.UpdateTValue(TValue);

            for (int j = 0; j < Whiskers.Length; j++)
            {
                double[] currentSignal = new double[WhiskerAngle.Frames.Length];
                for (int i = 0; i < WhiskerAngle.Frames.Length; i++)
                {
                    ISingleWhiskerAngle whiskerAngle = WhiskerAngle.Frames[i].Targets.Single(x => x.Whisker.WhiskerId == Whiskers[j].Whisker.WhiskerId);
                    currentSignal[i] = whiskerAngle.Angle;
                }

                Whiskers[j].Signal = currentSignal;
            }
        }

        public void UpdateAngleType(IAngleTypeBase angleType)
        {
            WhiskerAngle.UpdateAngleType(angleType);
            UpdateWhiskers();
        }

        private void UpdateWhiskers()
        {
            for (int j = 0; j < Whiskers.Length; j++)
            {
                double[] whiskerAngles = new double[WhiskerAngle.Frames.Length];
                for (int i = 0; i < WhiskerAngle.Frames.Length; i++)
                {
                    whiskerAngles[i] = WhiskerAngle.Frames[i].Targets.Single(x => x.Whisker.WhiskerId == Whiskers[j].Whisker.WhiskerId).Angle;
                }

                Whiskers[j].Signal = whiskerAngles;
            }
        }

        public object[][] ExportData()
        {
            object[][] data = new object[WhiskerAngle.Frames.Length + 6][];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new object[(Whiskers.Length*5) + 1];

                if (i < 5)
                {
                    continue;
                }

                if (i == 5)
                {
                    data[5][0] = "Frame";
                    continue;
                }

                data[i][0] = i - 5;
            }

            for (int i = 0; i < Whiskers.Length; i++)
            {
                data[0][1 + (i*5)] = Whiskers[i].Whisker.WhiskerName;

                data[1][1 + (i*5)] = "Max Angle: ";
                data[1][2 + (i*5)] = Whiskers[i].MaxAngle;

                data[2][1 + (i*5)] = "Minimum Angle: ";
                data[2][2 + (i*5)] = Whiskers[i].MinAngle;

                data[3][1 + (i*5)] = "Max Amplitude";
                data[3][2 + (i*5)] = Whiskers[i].MaxAmplitude;

                data[4][1 + (i*5)] = "RMS";
                data[4][2 + (i*5)] = Whiskers[i].RMS;

                data[5][1 + (i*5)] = "Current Max Angle";
                data[5][2 + (i*5)] = "Current Min Angle";
                data[5][3 + (i*5)] = "Current Amplitude";
                

                for (int j = 6; j < data.Length; j++)
                {
                    Whiskers[i].UpdateFrameNumber(j - 6);
                    data[j][1 + (i * 5)] = Whiskers[i].CurrentMaxAngle;
                    data[j][2 + (i * 5)] = Whiskers[i].CurrentMinAngle;
                    data[j][3 + (i * 5)] = Whiskers[i].CurrentAmplitude;
                }
            }

            return data;
        }

        public object[][] ExportMeanData(out int rowCount, out int columnCount)
        {
            rowCount = 6;
            columnCount = Whiskers.Length * 3;
            object[][] data = new object[rowCount][];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new object[columnCount];
            }

            data[0][0] = "Amplitude";
            for (int i = 0; i < Whiskers.Length; i++)
            {
                data[1][0 + (i * 3)] = Whiskers[i].Whisker.WhiskerName;

                data[2][0 + (i * 3)] = "Max Angle: ";
                data[2][1 + (i * 3)] = Whiskers[i].MaxAngle;

                data[3][0 + (i * 3)] = "Minimum Angle: ";
                data[3][1 + (i * 3)] = Whiskers[i].MinAngle;

                data[4][0 + (i * 3)] = "Max Amplitude";
                data[4][1 + (i * 3)] = Whiskers[i].MaxAmplitude;

                data[5][0 + (i * 3)] = "RMS";
                data[5][1 + (i * 3)] = Whiskers[i].RMS;
            }

            return data;
        }
    }
}
