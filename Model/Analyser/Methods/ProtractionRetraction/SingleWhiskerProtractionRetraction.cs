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

using MathNet.Numerics.Statistics;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.Services.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.ProtractionRetraction
{
    internal class SingleWhiskerProtractionRetraction : ModelObjectBase, ISingleWhiskerProtractionRetraction
    {
        private double[] m_AngleSignal;
        private double[] m_AngularVelocitySignal;
        private IWhisker m_Whisker;
        private double m_MeanProtractionVelocity;
        private double m_MeanRetractionVelocity;
        private double m_MaxAmplitude;
        private List<IProtractionRetractionBase> m_ProtractionRetractionData;

        public Dictionary<int, IProtractionRetractionBase> ProtractionRetractionDictionary
        {
            get;
            set;
        }

        public double[] AngleSignal
        {
            get
            {
                return m_AngleSignal;
            }
            set
            {
                if (ReferenceEquals(m_AngleSignal, value))
                {
                    return;
                }

                m_AngleSignal = value;

                CreateAngularVelocitySignal();
                MarkAsDirty();
            }
        }

        public double[] AngularVelocitySignal
        {
            get
            {
                return m_AngularVelocitySignal;
            }
            set
            {
                if (ReferenceEquals(m_AngularVelocitySignal, value))
                {
                    return;
                }

                m_AngularVelocitySignal = value;

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

        public double MeanProtractionVelocity
        {
            get
            {
                return m_MeanProtractionVelocity;
            }
            private set
            {
                if (Equals(m_MeanProtractionVelocity, value))
                {
                    return;
                }

                m_MeanProtractionVelocity = value;

                MarkAsDirty();
            }
        }

        public double MeanRetractionVelocity
        {
            get
            {
                return m_MeanRetractionVelocity;
            }
            private set
            {
                if (Equals(m_MeanRetractionVelocity, value))
                {
                    return;
                }

                m_MeanRetractionVelocity = value;

                MarkAsDirty();
            }
        }

        public double MaxAmplitude
        {
            get
            {
                return m_MaxAmplitude;
            }
            private set
            {
                if (Equals(m_MaxAmplitude, value))
                {
                    return;
                }

                m_MaxAmplitude = value;

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

        public List<IProtractionRetractionBase> ProtractionRetractionData
        {
            get
            {
                return m_ProtractionRetractionData;
            }
            set
            {
                if (ReferenceEquals(m_ProtractionRetractionData, value))
                {
                    return;
                }

                m_ProtractionRetractionData = value;

                MarkAsDirty();
            }
        }

        private void CreateAngularVelocitySignal()
        {
            if (AngleSignal == null || AngleSignal.Length == 0)
            {
                return;
            }

            bool error = false;

            double[] smoothedAngleSignal = SmoothingFunctions.BoxCarSmooth(AngleSignal);

            if (smoothedAngleSignal == null || smoothedAngleSignal.Length == 0)
            {
                return;
            }

            int[][] peaksAndValleys = SmoothingFunctions.FindPeaksAndValleys(smoothedAngleSignal);
            int[] peaks = peaksAndValleys[0];
            int[] valleys = peaksAndValleys[1];

            if (peaks.Length == 0 || valleys.Length == 0)
            {
                return;
            }

            int[] rawPeaks = new int[peaks.Length];
            for (int i = 0; i < peaks.Length; i++)
            {
                int closestPeak = SmoothingFunctions.FindClosestPeak(peaks[i], AngleSignal);

                if (rawPeaks.Contains(closestPeak))
                {
                    ErrorOccured("The signal is too noisy, accurate results can not be guaranteed");
                    error = true;
                }

                rawPeaks[i] = closestPeak;
            }

            int[] rawValleys = new int[valleys.Length];
            for (int i = 0; i < valleys.Length; i++)
            {
                int closestValley = SmoothingFunctions.FindClosestValley(valleys[i], AngleSignal);

                if (rawValleys.Contains(closestValley))
                {
                    ErrorOccured("The signal is too noisy, accurate results can not be guaranteed");
                    error = true;
                }

                rawValleys[i] = closestValley;
            }

            List<IProtractionRetractionBase> data = new List<IProtractionRetractionBase>();

            int peakCounter = 0;
            int valleyCounter = 0;
            int currentPeak = rawPeaks[peakCounter];
            int currentValley = rawValleys[valleyCounter];
            bool protract = currentPeak > currentValley;
            Dictionary<int, IProtractionRetractionBase> protractionRetractionDictionary = new Dictionary<int, IProtractionRetractionBase>();
            while (true)
            {
                if (protract)
                {
                    double deltaTime = Math.Abs(currentPeak - currentValley) / GlobalSettings.GlobalSettings.FrameRateSettings.OriginalFrameRate;
                    IProtractionRetractionBase protraction = ModelResolver.Resolve<IProtractionData>();
                    protraction.UpdateData(AngleSignal[currentValley], AngleSignal[currentPeak], deltaTime);
                    data.Add(protraction);

                    for (int i = currentValley; i < currentPeak; i++)
                    {
                        if (!protractionRetractionDictionary.ContainsKey(i))
                        {
                            protractionRetractionDictionary.Add(i, protraction);
                        }
                        else
                        {
                            ErrorOccured("The signal is too noisy, accurate results can not be guaranteed");
                            error = true;
                        }
                    }

                    valleyCounter++;

                    if (valleyCounter >= rawValleys.Length)
                    {
                        break;
                    }

                    currentValley = rawValleys[valleyCounter];

                    protract = false;
                }
                else
                {
                    double deltaTime = Math.Abs(currentPeak - currentValley) / GlobalSettings.GlobalSettings.FrameRateSettings.OriginalFrameRate;
                    IProtractionRetractionBase retraction = ModelResolver.Resolve<IRetractionData>();
                    retraction.UpdateData(AngleSignal[currentValley], AngleSignal[currentPeak], deltaTime);
                    data.Add(retraction);

                    for (int i = currentPeak; i < currentValley; i++)
                    {
                        if (!protractionRetractionDictionary.ContainsKey(i))
                        {
                            protractionRetractionDictionary.Add(i, retraction);
                        }
                        else
                        {
                            ErrorOccured("The signal is too noisy, accurate results can not be guaranteed");
                            error = true;
                        }
                    }

                    peakCounter++;

                    if (peakCounter >= rawPeaks.Length)
                    {
                        break;
                    }

                    currentPeak = rawPeaks[peakCounter];

                    protract = true;
                }
            }

            ProtractionRetractionData = data;
            ProtractionRetractionDictionary = protractionRetractionDictionary;

            MeanProtractionVelocity = ProtractionRetractionData.Where(x => x is IProtractionData).Select(x => x.MeanAngularVelocity).Mean();
            MeanRetractionVelocity = ProtractionRetractionData.Where(x => x is IRetractionData).Select(x => x.MeanAngularVelocity).Mean();

            double maxAngle = ProtractionRetractionData.Select(x => x.MaxAngle).Max();
            double minAngle = ProtractionRetractionData.Select(x => x.MinAngle).Min();

            MaxAmplitude = Math.Abs(maxAngle - minAngle);

            if (!error)
            {
                ModelObjectState = ModelObjectState.New;
            }
        }

        public IProtractionRetractionBase GetCurrentProtractionRetraction(int frame)
        {
            if (ProtractionRetractionDictionary == null)
            {
                return null;
            }

            if (ProtractionRetractionDictionary.ContainsKey(frame))
            {
                return ProtractionRetractionDictionary[frame];
            }

            return null;
        }
    }
}
