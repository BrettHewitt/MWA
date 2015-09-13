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
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Frequency
{
    internal class WhiskerFrequencyFrame : AnalyserFrame<ISingleWhiskerFrequency>, IWhiskerFrequencyFrame
    {
        public WhiskerFrequencyFrame()
        {
            Name = "Frequency";
        }

        public void LoadData(IMouseFrame frame)
        {
            TargetFrame = frame;

            int numberOfWhiskers = TargetFrame.Whiskers.Length;

            for (int i = 0; i < numberOfWhiskers; i++)
            {
                IWhisker whisker = TargetFrame.Whiskers[i];

                ISingleWhiskerFrequency singleWhiskerFrequency = ModelResolver.Resolve<ISingleWhiskerFrequency>();
                singleWhiskerFrequency.Whisker = whisker;

                Targets.Add(singleWhiskerFrequency);
            }
        }        

        public override object[] ExportData()
        {
            return null;
        }
    }
}
