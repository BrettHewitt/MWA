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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;
using RobynsWhiskerTracker.ModelInterface.Whisker;

namespace RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Amplitude
{
    public interface ISingleWhiskerAmplitude : IModelObjectBase
    {
        IWhisker Whisker
        {
            get;
            set;
        }

        double MaxAmplitude
        {
            get;
        }

        double MinAngle
        {
            get;
        }

        double MaxAngle
        {
            get;
        }

        double CurrentAmplitude
        {
            get;
        }

        double CurrentMinAngle
        {
            get;
        }

        double CurrentMaxAngle
        {
            get;
        }

        double AverageAmplitude
        {
            get;
        }

        double RMS
        {
            get;
        }

        double[] Signal
        {
            get;
            set;
        }

        int CurrentFrame
        {
            get;
        }

        int[] SignalPeaks
        {
            get;
        }

        int[] SignalValleys
        {
            get;
        }

        List<IProtractionRetractionBase> ProtractionRetractionData
        {
            get;
            set;
        }

        Dictionary<int, IProtractionRetractionBase> ProtractionRetractionDictionary
        {
            get;
            set;
        }

        void UpdateFrameNumber(int indexNumber);
    }
}
