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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency.FrequencyTypes;
using RobynsWhiskerTracker.Services.Maths;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Frequency.FrequencyTypes
{
    internal class DiscreteFourierTransform : FrequencyTypeBase, IDiscreteFourierTransform
    {
        private Dictionary<double, double> FourierGraph
        {
            get;
            set;
        }

        public DiscreteFourierTransform() : base("Discrete Fourier Transform")
        {
        }

        public override double CalculateFrequency(double[] signal, double frameRate, double frameInterval)
        {
            double bestFrequency;

            FourierGraph = BrettFFT.BrettDFT(signal, out bestFrequency, 1, 40);

            return bestFrequency;
        }

        public override string[] GetExtraDataNames()
        {
            return new string[]
            {
                "DFT", "Frequency (Hz)", "Power"
            };
        }

        public override Dictionary<double, double> GetExtraData()
        {
            return FourierGraph;
        }
    }
}
