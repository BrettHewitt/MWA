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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngle
{
    internal class WhiskerAngleFrame : AnalyserFrame<ISingleWhiskerAngle>, IWhiskerAngleFrame
    {
        public WhiskerAngleFrame()
        {
            Name = "Whisker Angle";
        }

        public void LoadData(IMouseFrame frame)
        {
            TargetFrame = frame;

            IAngleTypeBase angleType;
            IWhisker noseWhisker = TargetFrame.Whiskers.FirstOrDefault(x => x.WhiskerId == -1);
            IWhisker orientationWhisker = TargetFrame.Whiskers.FirstOrDefault(x => x.WhiskerId == 0);

            if (noseWhisker == null || orientationWhisker == null)
            {
                //can't generate centerline, use vertical
                angleType = ModelResolver.Resolve<IVertical>();
            }
            else
            {
                ICenterLine centerLine = ModelResolver.Resolve<ICenterLine>();
                centerLine.NosePoint = noseWhisker.WhiskerPoints[0];
                centerLine.OrientationPoint = orientationWhisker.WhiskerPoints[0];
                angleType = centerLine;
            }
            

            foreach (IWhisker whisker in TargetFrame.Whiskers)
            {
                if (whisker.IsGenericPoint)
                {
                    continue;
                }

                ISingleWhiskerAngle singleWhiskerCurve = ModelResolver.Resolve<ISingleWhiskerAngle>();
                singleWhiskerCurve.Whisker = whisker;
                singleWhiskerCurve.AngleType = angleType;
                Targets.Add(singleWhiskerCurve);
            }
        }

        public void UpdateAngleType(IAngleTypeBase angleType)
        {
            IAngleTypeBase actualAngleType = angleType.GetInstance();
            actualAngleType.LoadData(TargetFrame);
            
            foreach (ISingleWhiskerAngle whisker in Targets)
            {
                whisker.AngleType = actualAngleType;
            }
        }

        public override object[] ExportData()
        {
            int length = Targets.Count;
            object[] data = new object[length];

            for (int i = 0; i < length; i++)
            {
                data[i] = Targets[i].Angle;
            }

            return data;
        }
    }
}
