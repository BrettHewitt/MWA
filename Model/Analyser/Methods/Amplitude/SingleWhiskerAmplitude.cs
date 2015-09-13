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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.Services.Maths;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Amplitude
{
    internal class SingleWhiskerAmplitude : ModelObjectBase, ISingleWhiskerAmplitude
    {
        private IWhisker m_Whisker;
        private double m_MaxAmplitude;
        private double m_MinAngle;
        private double m_MaxAngle;
        private double m_CurrentAmplitude;
        private double m_CurrentMaxAngle;
        private double m_CurrentMinAngle;
        private double m_AverageAmplitude;
        private double m_RMS;

        private double[] m_Signal;
        private int m_CurrentFrame;

        private int[] m_SignalPeaks;
        private int[] m_SignalValleys;

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

        public double CurrentAmplitude
        {
            get
            {
                return m_CurrentAmplitude;
            }
            private set
            {
                if (Equals(m_CurrentAmplitude, value))
                {
                    return;
                }

                m_CurrentAmplitude = value;

                MarkAsDirty();
            }
        }

        public double CurrentMinAngle
        {
            get
            {
                return m_CurrentMinAngle;
            }
            private set
            {
                if (Equals(m_CurrentMinAngle, value))
                {
                    return;
                }

                m_CurrentMinAngle = value;

                MarkAsDirty();
            }
        }

        public double CurrentMaxAngle
        {
            get
            {
                return m_CurrentMaxAngle;
            }
            private set
            {
                if (Equals(m_CurrentMaxAngle, value))
                {
                    return;
                }

                m_CurrentMaxAngle = value;

                MarkAsDirty();
            }
        }

        public double AverageAmplitude
        {
            get
            {
                return m_AverageAmplitude;
            }
            private set
            {
                if (Equals(m_AverageAmplitude, value))
                {
                    return;
                }

                m_AverageAmplitude = value;

                MarkAsDirty();
            }
        }

        public double RMS
        {
            get
            {
                return m_RMS;
            }
            private set
            {
                if (Equals(m_RMS, value))
                {
                    return;
                }

                m_RMS = value;

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
                if (ReferenceEquals(m_Signal, value))
                {
                    return;
                }

                m_Signal = value;

                CalculateData();

                MarkAsDirty();
            }
        }

        public int CurrentFrame
        {
            get
            {
                return m_CurrentFrame;
            }
            private set
            {
                if (Equals(m_CurrentFrame, value))
                {
                    return;
                }

                m_CurrentFrame = value;

                CalculateCurrentFrameData();

                MarkAsDirty();
            }
        }

        public int[] SignalPeaks
        {
            get
            {
                return m_SignalPeaks;
            }
            private set
            {
                if (ReferenceEquals(m_SignalPeaks, value))
                {
                    return;
                }

                m_SignalPeaks = value;

                MarkAsDirty();
            }
        }

        public int[] SignalValleys
        {
            get
            {
                return m_SignalValleys;
            }
            private set
            {
                if (ReferenceEquals(m_SignalValleys, value))
                {
                    return;
                }

                m_SignalValleys = value;

                MarkAsDirty();
            }
        }

        private List<IProtractionRetractionBase> m_ProtractionRetractionData;

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

        public Dictionary<int, IProtractionRetractionBase> ProtractionRetractionDictionary
        {
            get;
            set;
        }

        private void CalculateData()
        {
            MinAngle = Signal.Min();
            MaxAngle = Signal.Max();
            MaxAmplitude = Math.Abs(MaxAngle - MinAngle);
            RMS = SmoothingFunctions.GetRMS(Signal);

            if (ProtractionRetractionData == null)
            {
                return;
            }

            AverageAmplitude = ProtractionRetractionData.Where(x => x.Amplitude != 0).Average(protractionRetraction => protractionRetraction.Amplitude);

            double[] smoothedSignal = SmoothingFunctions.BoxCarSmooth(Signal);
            int[][] peaksandValleys = SmoothingFunctions.FindPeaksAndValleys(smoothedSignal);
            int[] signalPeaks = peaksandValleys[0];
            int[] signalValleys = peaksandValleys[1];

            SignalPeaks = signalPeaks.Select(peak => SmoothingFunctions.FindClosestPeak(peak, Signal)).ToArray();
            SignalValleys = signalValleys.Select(valley => SmoothingFunctions.FindClosestValley(valley, Signal)).ToArray();
        }

        public void UpdateFrameNumber(int indexNumber)
        {
            CurrentFrame = indexNumber;
        }

        private void CalculateCurrentFrameData()
        {
            //Need to find previous peak/valley and next valley/peak
            int closestPeak = int.MaxValue;
            int cloestPeakDelta = int.MaxValue;
            int closestValley = int.MaxValue;
            int closestValleyDelta = int.MaxValue;

            foreach (int peak in SignalPeaks)
            {
                int delta = Math.Abs(CurrentFrame - peak);
                if (delta < cloestPeakDelta)
                {
                    cloestPeakDelta = delta;
                    closestPeak = peak;
                }
            }

            foreach (int valley in SignalValleys)
            {
                int delta = Math.Abs(CurrentFrame - valley);
                if (delta < closestValleyDelta)
                {
                    closestValleyDelta = delta;
                    closestValley = valley;
                }
            }

            //Make sure current frame is between closest valley and closest peak
            int peakDelta = Math.Abs(CurrentFrame - closestPeak);
            int valleyDelta = Math.Abs(CurrentFrame - closestValley);

            bool closerToPeak = peakDelta < valleyDelta;
            int value = closerToPeak ? closestPeak : closestValley;
            bool before = CurrentFrame < value;

            int nextValue;

            if (closerToPeak)
            {
                if (before)
                {
                    //Search for previous valley
                    nextValue = GetPreviousValley(CurrentFrame);
                }
                else
                {
                    //Search for next valley
                    nextValue = GetNextValley(CurrentFrame);
                }
            }
            else
            {
                if (before)
                {
                    //Search for previous peak
                    nextValue = GetPreviousPeak(CurrentFrame);
                }
                else
                {
                    //Search for next peak
                    nextValue = GetNextPeak(CurrentFrame);
                }
            }

            if (nextValue == -1)
            {
                ZeroData();
                return;
            }

            if (closerToPeak)
            {
                CurrentMinAngle = Signal[nextValue];
                CurrentMaxAngle = Signal[value];
            }
            else
            {
                CurrentMinAngle = Signal[value];
                CurrentMaxAngle = Signal[nextValue];
            }
            
            CurrentAmplitude = Math.Abs(CurrentMaxAngle - CurrentMinAngle);
        }

        private void ZeroData()
        {
            CurrentAmplitude = 0;
            CurrentMaxAngle = 0;
            CurrentMinAngle = 0;
        }

        private int GetNextValley(int currentFrame)
        {
            for (int i = 0; i < SignalValleys.Length; i++)
            {
                if (SignalValleys[i] > currentFrame)
                {
                    return SignalValleys[i];
                }
            }

            return -1;
        }

        private int GetPreviousValley(int currentFrame)
        {
            for (int i = SignalValleys.Length - 1; i >= 0; i--)
            {
                if (SignalValleys[i] < currentFrame)
                {
                    return SignalValleys[i];
                }
            }

            return -1;
        }

        private int GetNextPeak(int currentFrame)
        {
            for (int i = 0; i < SignalPeaks.Length; i++)
            {
                if (SignalPeaks[i] > currentFrame)
                {
                    return SignalPeaks[i];
                }
            }

            return -1;
        }

        private int GetPreviousPeak(int currentFrame)
        {
            for (int i = SignalPeaks.Length - 1; i >= 0; i--)
            {
                if (SignalPeaks[i] < currentFrame)
                {
                    return SignalPeaks[i];
                }
            }

            return -1;
        }
    }
}
