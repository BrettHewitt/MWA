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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Curvature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Curvature
{
    internal class WhiskerCurveFrame : AnalyserFrame<ISingleWhiskerCurve>, IWhiskerCurveFrame
    {
        public WhiskerCurveFrame()
        {
            Name = "Curvature";
        }

        public void LoadData(IMouseFrame frame)
        {
            TargetFrame = frame;
            foreach (IWhisker whisker in TargetFrame.Whiskers)
            {
                if (whisker.IsGenericPoint)
                {
                    continue;
                }

                ISingleWhiskerCurve singleWhiskerCurve = ModelResolver.Resolve<ISingleWhiskerCurve>();
                singleWhiskerCurve.Whisker = whisker;
                Targets.Add(singleWhiskerCurve);
            }
        }

        public override object[] ExportData()
        {
            int length = Targets.Count;
            object[] data = new object[length];

            for (int i = 0; i < length; i++)
            {
                data[i] = Targets[i].Curvature;
            }

            return data;
        }
    }
}
