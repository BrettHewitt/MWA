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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngularVelocity;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.ProtractionRetraction
{
    internal class WhiskerProtractionRetraction : ModelObjectBase, IWhiskerProtractionRetraction
    {
        private ISingleWhiskerProtractionRetraction[] m_Whiskers;
        private IWhiskerAngle m_WhiskerAngle;
        //private IWhiskerAngularVelocity m_WhiskerAngularVelocity;
        private double m_TValue;
        private string m_Name;
        //private double[] m_AngleSignal;

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

        public ISingleWhiskerProtractionRetraction[] Whiskers
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

        //public IWhiskerAngularVelocity WhiskerAngularVelocity
        //{
        //    get
        //    {
        //        return m_WhiskerAngularVelocity;
        //    }
        //    private set
        //    {
        //        if (Equals(m_WhiskerAngularVelocity, value))
        //        {
        //            return;
        //        }

        //        m_WhiskerAngularVelocity = value;

        //        MarkAsDirty();
        //    }
        //}

        public IWhiskerAngle WhiskerAngle
        {
            get
            {
                return m_WhiskerAngle;
            }
            set
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

        public WhiskerProtractionRetraction()
        {
            Name = "Mean Offset";
            WhiskerAngle = ModelResolver.Resolve<IWhiskerAngle>();
            //WhiskerAngularVelocity = ModelResolver.Resolve<IWhiskerAngularVelocity>();
        }

        public void LoadData(IMouseFrame[] frames)
        {
            //Load Whisker Angle Data
            WhiskerAngle.LoadData(frames);
            WhiskerAngle.UpdateTValue(0);
            //WhiskerAngularVelocity.LoadData(frames);

            List<ISingleWhiskerProtractionRetraction> whiskers = new List<ISingleWhiskerProtractionRetraction>();
            foreach (IWhisker whisker in frames[0].Whiskers)
            {
                if (whisker.IsGenericPoint)
                {
                    continue;
                }

                ISingleWhiskerProtractionRetraction singleWhisker = ModelResolver.Resolve<ISingleWhiskerProtractionRetraction>();
                singleWhisker.Whisker = whisker;

                double[] signal = new double[frames.Length];
                for (int i = 0; i < frames.Length; i++)
                {
                    signal[i] = WhiskerAngle.Frames[i].Targets.Single(x => x.Whisker.WhiskerId == whisker.WhiskerId).Angle;
                }

                singleWhisker.AngleSignal = signal;

                whiskers.Add(singleWhisker);
            }

            Whiskers = whiskers.ToArray();

            UpdateTValue();
        }

        public void UpdateTValue()
        {
            if (WhiskerAngle == null)
            {
                return;
            }

            WhiskerAngle.UpdateTValue(TValue);
            //WhiskerAngularVelocity.UpdateTValue(TValue);

            UpdateWhiskers();
        }

        private void UpdateWhiskers()
        {
            for (int j = 0; j < Whiskers.Length; j++)
            {
                double[] whiskerAngles = new double[WhiskerAngle.Frames.Length];
                for (int i = 0; i < WhiskerAngle.Frames.Length; i++)
                {
                    whiskerAngles[i] = WhiskerAngle.Frames[i].Targets.Single(x => x.Whisker.WhiskerId == Whiskers[j].WhiskerId).Angle;
                }

                Whiskers[j].AngleSignal = whiskerAngles;
            }
        }

        public object[][] ExportData()
        {
            object[][] data = new object[WhiskerAngle.Frames.Length + 5][];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new object[(Whiskers.Length * 5) + 1];

                if (i < 4)
                {
                    continue;
                }

                if (i == 4)
                {
                    data[4][0] = "Frame";
                    continue;
                }

                data[i][0] = i - 4;
            }

            for (int i = 0; i < Whiskers.Length; i++)
            {
                data[0][1 + (i * 5)] = Whiskers[i].Whisker.WhiskerName;

                data[1][1 + (i * 5)] = "Mean Protraction Velocity: ";
                data[1][2 + (i * 5)] = Whiskers[i].MeanProtractionVelocity;

                data[2][1 + (i * 5)] = "Mean Retraction Velocity: ";
                data[2][2 + (i * 5)] = Whiskers[i].MeanRetractionVelocity;

                data[3][1 + (i * 5)] = "Max Amplitude";
                data[3][2 + (i * 5)] = Whiskers[i].MaxAmplitude;

                data[4][1 + (i * 5)] = "Action";
                data[4][2 + (i * 5)] = "Current Velocity";
                data[4][3 + (i * 5)] = "Current Amplitude";
                data[4][4 + (i * 5)] = "Max Angle";
                data[4][5 + (i * 5)] = "Min Angle";

                for (int j = 5; j < data.Length + 5; j++)
                {
                    IProtractionRetractionBase protractRetract = Whiskers[i].GetCurrentProtractionRetraction(j - 4);

                    if (protractRetract == null)
                    {
                        continue;
                    }

                    data[j][1 + (i * 5)] = protractRetract.Name;
                    data[j][2 + (i * 5)] = protractRetract.MeanAngularVelocity;
                    data[j][3 + (i * 5)] = protractRetract.Amplitude;
                    data[j][4 + (i * 5)] = protractRetract.MaxAngle;
                    data[j][5 + (i * 5)] = protractRetract.MinAngle;
                }
            }

            return data;
        }

        public object[][] ExportMeanData(out int rowCount, out int columnCount)
        {
            rowCount = 5;
            columnCount = Whiskers.Length*3;
            object[][] data = new object[5][];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new object[Whiskers.Length * 3];
            }

            data[0][0] = "Protraction/Retraction";

            for (int i = 0; i < Whiskers.Length; i++)
            {
                data[1][0 + (i*3)] = Whiskers[i].Whisker.WhiskerName;

                data[2][0 + (i*3)] = "Mean Protraction Velocity: ";
                data[2][1 + (i*3)] = Whiskers[i].MeanProtractionVelocity;

                data[3][0 + (i*3)] = "Mean Retraction Velocity: ";
                data[3][1 + (i*3)] = Whiskers[i].MeanRetractionVelocity;

                data[4][0 + (i*3)] = "Max Amplitude";
                data[4][1 + (i*3)] = Whiskers[i].MaxAmplitude;
            }

            return data;
        }

        public void UpdateAngleType(IAngleTypeBase angleType)
        {
            WhiskerAngle.UpdateAngleType(angleType);
            UpdateWhiskers();
        }
    }
}
