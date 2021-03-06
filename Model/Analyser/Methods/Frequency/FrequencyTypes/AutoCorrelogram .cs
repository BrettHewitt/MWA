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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency.FrequencyTypes;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Frequency.FrequencyTypes
{
    internal class AutoCorrelogram : FrequencyTypeBase, IAutoCorrelogram 
    {
        public Dictionary<double, double> SmoothedSignal
        {
            get;
            set;
        }

        public AutoCorrelogram() : base("Autocorrelogram")
        {
        }

        public override double CalculateFrequency(double[] signal, double frameRate, double frameInterval)
        {
            //Smooth signal using box-car filter
            double[] smoothedSignal = BoxCarFilter(signal);

            //Create Dictionary
            SmoothedSignal = new Dictionary<double, double>();
            for (int i = 0; i < smoothedSignal.Length; i++)
            {
                SmoothedSignal.Add(i, smoothedSignal[i]);
            }

            //Find peaks from smoothed signal
            int[] peaks = FindPeaks(smoothedSignal);

            if (peaks.Length < 2)
            {
                return 0;
            }

            //Find peaks from original signal
            int[] rawPeaks = new int[peaks.Length];

            for (int i = 0; i < peaks.Length; i++)
            {
                rawPeaks[i] = FindClosestPeak(peaks[i], signal);
            }

            //Calculate average frames between peaks
            int peakCounter = 0;
            double cumulativePeak = 0;

            for (int i = 1; i < rawPeaks.Length; i++)
            {
                cumulativePeak += rawPeaks[i] - rawPeaks[i - 1];
                peakCounter++;
            }

            double averageFramesBetweenPeak = cumulativePeak / peakCounter;

            return (frameRate / frameInterval) / averageFramesBetweenPeak;
        }

        public override Dictionary<double, double> GetExtraData()
        {
            return SmoothedSignal;
        }

        public override string[] GetExtraDataNames()
        {
            return new string[]
            {
                "Smoothed Signal", "Frame", "Angle"
            };
        }

        private int[] FindPeaks(double[] smoothedSignal, int range = 2)
        {
            List<int> peaks = new List<int>();

            for (int i = range; i < smoothedSignal.Length - range; i++)
            {
                double currentValue = smoothedSignal[i];

                bool isPeak = true;

                for (int j = -range; j <= range; j++)
                {
                    if (j == 0)
                    {
                        continue;
                    }

                    double testValue = smoothedSignal[i - j];

                    if (testValue > currentValue)
                    {
                        isPeak = false;
                        break;
                    }
                }

                if (isPeak)
                {
                    peaks.Add(i);
                }
            }

            return peaks.ToArray();
        }

        private double[] BoxCarFilter(double[] signal)
        {
            double[] filter = new double[] { 0.2, 0.2, 0.2, 0.2, 0.2 };
            double[] result = new double[signal.Length];
            double[] borderedSignal = new double[signal.Length + 4];

            for (int i = -2; i < signal.Length + 2; i++)
            {
                if (i < 0)
                {
                    borderedSignal[i + 2] = signal[0];
                }
                else if (i >= signal.Length)
                {
                    borderedSignal[i + 2] = signal.Last();
                }
                else
                {
                    borderedSignal[i + 2] = signal[i];
                }
            }

            for (int i = 2; i < borderedSignal.Length - 2; i++)
            {
                //Start at 2, finish at length - 2
                result[i - 2] = (borderedSignal[i - 2] * filter[0]) + (borderedSignal[i - 1] * filter[1]) + (borderedSignal[i] * filter[2]) + (borderedSignal[i + 1] * filter[3]) + (borderedSignal[i + 2] * filter[4]);
            }

            return result;
        }

        private int FindClosestPeak(int index, double[] signal)
        {
            double currentValue = signal[index];

            //Go left or right?
            double leftValue;
            double rightValue;

            if (index - 1 < 0)
            {
                rightValue = signal[index + 1];

                if (currentValue > rightValue)
                {
                    //This is a peak already
                    return index;
                }
                else
                {
                    //Scan right
                    return ScanRightForPeak(index, signal);
                }
            }
            else if (index + 1 >= signal.Length)
            {
                leftValue = signal[index - 1];

                if (currentValue > leftValue)
                {
                    //This is a peak already
                    return index;
                }
                else
                {
                    //Scan left
                    return ScanLeftForPeak(index, signal);
                }
            }
            else
            {
                leftValue = signal[index - 1];
                rightValue = signal[index + 1];

                if (currentValue < leftValue && currentValue < rightValue)
                {
                    //We're at a valley, look both ways maybe?
                    int leftIndex = ScanLeftForPeak(index, signal);
                    int rightIndex = ScanRightForPeak(index, signal);

                    int leftDelta = Math.Abs(index - leftIndex);
                    int rightDelta = Math.Abs(index - rightIndex);

                    if (leftDelta <= rightDelta)
                    {
                        return leftIndex;
                    }
                    else
                    {
                        return rightIndex;
                    }
                }
                else if (currentValue < leftValue)
                {
                    //Go left
                    return ScanLeftForPeak(index, signal);
                }
                else if (currentValue < rightValue)
                {
                    //Go right
                    return ScanRightForPeak(index, signal);
                }
                else
                {
                    //We're already at a peak
                    return index;
                }
            }

            return 0;
        }

        private int ScanLeftForPeak(int index, double[] signal)
        {
            int testIndex = index;
            double currentValue = signal[testIndex];
            double testValue = signal[testIndex - 1];

            while (currentValue < testValue)
            {
                testIndex--;

                if (testIndex <= 0)
                {
                    return testIndex;
                }

                currentValue = signal[testIndex];
                testValue = signal[testIndex - 1];
            }

            return testIndex;
        }

        private int ScanRightForPeak(int index, double[] signal)
        {
            int testIndex = index;
            double currentValue = signal[testIndex];
            double testValue = signal[testIndex + 1];

            while (currentValue < testValue)
            {
                testIndex++;

                if (testIndex >= signal.Length - 1)
                {
                    return testIndex;
                }

                currentValue = signal[testIndex];
                testValue = signal[testIndex + 1];
            }

            return testIndex;
        }
    }
}
