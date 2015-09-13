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
using RobynsWhiskerTracker.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngle
{
    internal class WhiskerAngle : MethodBase<IWhiskerAngleFrame>, IWhiskerAngle
    {
        public WhiskerAngle() : base("Whisker Angle")
        {

        }

        public void UpdateTValue(double tValue)
        {
            foreach (var frame in Frames)
            {
                foreach (var angle in frame.Targets)
                {
                    angle.TValue = tValue;
                }
            }
        }

        protected override IWhiskerAngleFrame[] CreateData(IMouseFrame[] frames)
        {
            int frameCount = frames.Length;

            IWhiskerAngleFrame[] data = new IWhiskerAngleFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                IWhiskerAngleFrame frameData = ModelResolver.Resolve<IWhiskerAngleFrame>();
                frameData.LoadData(frames[i]);

                data[frames[i].IndexNumber] = frameData;
            }

            return data;
        }


        public void UpdateAngleType(IAngleTypeBase angleType)
        {
            foreach (var frame in Frames)
            {
                frame.UpdateAngleType(angleType);
            }
        }

        public override object[][] ExportData()
        {
            if (Frames == null)
            {
                return null;
            }

            if (Frames.Length == 0)
            {
                return null;
            }

            List<object[]> data = new List<object[]>();

            data.Add(Frames[0].Targets.Select(target => target.Whisker.WhiskerName).Cast<object>().ToArray());
            data.AddRange(Frames.Select(frame => frame.ExportData()));

            return data.ToArray();
        }

        public override object[][] ExportMeanData()
        {
            return null;
        }
    }
}
